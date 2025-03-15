* Get the .NET MAUI artifacts from: https://github.com/dotnet/maui/pull/24365
  * There is a version also included already in this repository, so you can skip this step if you're going to look at this project. If you want to apply it to your own project, get the artifacts from here or the above PR.
* Create a new solution with the .NET Aspire AppHost project and whatever you desire or add the Aspire AppHost project to your existing .NET MAUI project/solution
* Add a .NET MAUI app with the version from above
  * Skip this is you're applying this to an existing project 
* Go into MAUI app csproj and change `<TargetFrameworks>` to `<TargetFramework>` (no trailing s) and pick the framework you want
  * This is due to a bug in the tooling, if this were to be an actual project in the future, this will be resolved
* Still in the csproj add (without this, service discovery will still work, all the rest will **not** work):
  ```xml
  <PropertyGroup Condition="($([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android' or $([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'ios') and '$(Configuration)' == 'Debug'">
    <HttpActivityPropagationSupport>true</HttpActivityPropagationSupport>
  </PropertyGroup>
  ```
  
* Still in the csproj add (without this metrics will not work for Android, the rest **does** work):
  ```xml
  <!-- Needed for enabling metrics on Android -->
  <PropertyGroup Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android' and '$(Configuration)' == 'Debug'">
      <MetricsSupport>true</MetricsSupport>
  </PropertyGroup>
  ```
  
* Add .NET MAUI app project reference to .NET Aspire HostApp project
* Add your .NET MAUI app to the .NET Aspire AppHost **Program.cs** and add any references to services that you want to consume in your .NET MAUI app. For example:
  ```csharp
  // This was probably already there
  var builder = DistributedApplication.CreateBuilder(args);

  // This is a service you want to call from your .NET MAUI app and/or other projects
  var webapi = builder.AddProject<Projects.SampleWebApi>("webapi");

  // This registers your .NET MAUI app and adds a reference to the Web API you want to call
  builder.AddProject<Projects.MauiAspire>("mauiapp")
      .WithReference(webapi);

  // This was probably already there
  builder.Build().Run();
  ```
* Add .NET MAUI ServiceDefaults project to the solution
  * This is a new template in the artifacts above, or take it from: https://github.com/jfversluis/MauiAspire/tree/main/MauiApp.ServiceDefaults
* Add a reference to the .NET MAUI ServiceDefaults project in your .NET MAUI app
* Go into your **MauiProgram.cs** in your .NET MAUI project and add `builder.AddServiceDefaults();` also make sure to add `using Microsoft.Extensions.Hosting;` in the top of your file.
* Start registering your services/HttpClient instances, for example (of course make sure that your service discovery endpoints match, in this case `webapi` must match how your Web API is registered in the .NET Aspire AppHost project):
  ```csharp
  builder.Services.AddHttpClient<WeatherApiClient>(client =>
  {
      // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
      // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
      client.BaseAddress = new("https+http://webapi");
  });
  ```
* Go into the .NET Aspire AppHost project's **launchSettings.json** file and note the port number for `DOTNET_RESOURCE_SERVICE_ENDPOINT_URL` (for example 21104)
* Go into the Web API **launchSettings.json** file and note the port number of the application. (for example 1337)
* Make sure Dev Tunnels is installed: https://learn.microsoft.com/azure/developer/dev-tunnels/get-started
* Manually start a Dev Tunnel, for example: `devtunnel host --allow-anonymous --protocol https -p 21104 -p 1337` note the Dev Tunnel ID. For example, if the resulting URL is `https://lgtm1234-7291.euw.devtunnels.ms`, the ID is `lgtm1234`
* Make sure to click the link and open each endpoint (shown in the console output) in the browser. You will need to manually accept once, to allow traffic to go through the tunnel.
* With the Dev Tunnel ID go back to the **launchSettings.json** file of the AppHost project and add a new entry under `EnvironmentVariables`: add `DEVTUNNEL_ID` with the ID you just got from the Dev Tunnel:
  ```json 
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "DOTNET_ENVIRONMENT": "Development",
    "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL": "https://localhost:21014",
    "DOTNET_RESOURCE_SERVICE_ENDPOINT_URL": "https://localhost:22279",
    "DEVTUNNEL_ID": "lgtm1234"
  }
  ```
* Consume your registered service/HttpClient in your .NET MAUI app
* Set the AppHost project as the startup project, click the run button and all should light up and you should see all data in the .NET Aspire dashboard!
