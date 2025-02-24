using MauiServiceDefaults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Collections;

namespace MauiAspire
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            
            builder.Configuration.AddInMemoryCollection(GetEnvVars());

            builder.AddServiceDefaults();

            builder.Services.AddHttpClient<WeatherApiClient>(client =>
            {
                // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
                // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
                client.BaseAddress = new("https+http://samplewebapi");
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            
            var app = builder.Build();

            // TODO can we move this to Extensions?
            app.Services.GetService<MeterProvider>();
            app.Services.GetService<TracerProvider>();
            app.Services.GetService<LoggerProvider>();

            return app;
        }

        static List<KeyValuePair<string, string?>> GetEnvVars()
        {
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();

            // For Android we read the environment variables from a text file that is written to the device/emulator
            if (OperatingSystem.IsAndroid())
            {
                var envVarLines = File.ReadAllLines("/data/local/tmp/ide-launchenv.txt");

                environmentVariables = envVarLines
                    .Select(line => line.Split('=', 2))
                    .ToDictionary(parts => parts[0], parts => parts[1]);
            }

            var variablesToInclude = new HashSet<string>
            {
                "ASPNETCORE_ENVIRONMENT",
                "ASPNETCORE_URLS",
                "DOTNET_ENVIRONMENT",
                "DOTNET_LAUNCH_PROFILE",
                "DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION"
            };

            var prefixesToRemove = new List<string>
            {
                "ASPNETCORE_",
                "DOTNET_",
            };

            List<KeyValuePair<string, string?>> settings = new();
            foreach (object variableNameObject in environmentVariables.Keys)
            {
                string variableName = (string)variableNameObject;
                if (variablesToInclude.Contains(variableName) || variableName.StartsWith("OTEL_") || variableName.StartsWith("LOGGING__CONSOLE") || variableName.StartsWith("services__"))
                {
                    string value = (string)environmentVariables[variableName]!;

                    // Normalize the key, matching the logic here:
                    // https://github.dev/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Configuration.EnvironmentVariables/src/EnvironmentVariablesConfigurationProvider.cs
                    variableName = variableName.Replace("__", ":");

                    // For defined prefixes, add the variable with the prefix removed, matching the logic
                    // in EnvironmentVariablesConfigurationProvider.cs. Also add the variable with the
                    // prefix intact, which matches the normal HostApplicationBuilder behavior, where
                    // there's an EnvironmentVariablesConfigurationProvider added with and another one
                    // without the prefix set.
                    foreach (var prefix in prefixesToRemove)
                    {
                        if (variableName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                        {
                            settings.Add(new KeyValuePair<string, string?>(variableName, value));
                            variableName = variableName.Substring(prefix.Length);
                            break;
                        }
                    }

                    settings.Add(new KeyValuePair<string, string?>(variableName, value));
                }
            }

            return settings;
        }
    }
}
