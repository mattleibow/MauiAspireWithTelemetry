# .NET MAUI <3 .NET Aspire

This repository serves as a proof-of-concept for what a .NET MAUI integration with .NET Aspire would look like.

You can either pull down this repository which should be a self-contained sample that you can play with. Additionally, you can try to apply this to an existing .NET MAUI project that you may want to try this on. See the steps for that below.

If you want to learn more, please check out [this video](https://youtu.be/RCIM6qagZ-U).

## Prerequisites

You will need to run all of the below with Visual Studio 17.13+ on Windows for the time being.

## Applying to your existing .NET MAUI project

* Make sure you are using `Microsoft.Maui.Controls` .NET 10 preview 4+
* Add .NET Aspire App Host project to your solution
* Add .NET MAUI app project reference to .NET Aspire App Host project, see [here](https://github.com/jfversluis/MauiAspire/blob/main/MauiAspire.AppHost/MauiAspire.AppHost.csproj#L19) in this sample project.
* Add your .NET MAUI app to the .NET Aspire App Host [**Program.cs**](https://github.com/jfversluis/MauiAspire/blob/main/MauiAspire.AppHost/Program.cs) and add any references to services that you want to consume in your .NET MAUI app. For example:
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
  * This is a new template in .NET 10 preview 4+ (`dotnet new maui-aspire-servicedefaults`), or take it from: https://github.com/jfversluis/MauiAspire/tree/main/MauiApp.ServiceDefaults
* Add a [reference](https://github.com/jfversluis/MauiAspire/blob/main/SampleMauiApp/SampleMauiApp.csproj#L77) to the .NET MAUI ServiceDefaults project in your .NET MAUI app
* Go into your [**MauiProgram.cs**](https://github.com/jfversluis/MauiAspire/blob/main/MauiAspire/MauiProgram.cs#L19) in your .NET MAUI project and add `builder.AddServiceDefaults();` also make sure to add `using Microsoft.Extensions.Hosting;` in the top of your file.
* Start registering your services/HttpClient instances, for example (of course make sure that your service discovery endpoints match, in this case `webapi` must match how your Web API is registered in the .NET Aspire AppHost project), see [here](https://github.com/jfversluis/MauiAspire/blob/main/SampleMauiApp/MauiProgram.cs#L21-L26) in this sample project:
  ```csharp
  builder.Services.AddHttpClient<WeatherApiClient>(client =>
  {
      // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
      // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
      client.BaseAddress = new("https+http://webapi");
  });
  ```
* Go into the .NET Aspire AppHost project's **launchSettings.json** file and note the port number for [`DOTNET_DASHBOARD_OTLP_ENDPOINT_URL`](https://github.com/jfversluis/MauiAspire/blob/main/MauiAspire.AppHost/Properties/launchSettings.json#L13) (for example 21104)
* Go into the Web API [**launchSettings.json**](https://github.com/jfversluis/MauiAspire/blob/main/SampleWebApi/Properties/launchSettings.json#L17) file and note the port number of the application. (for example 7291)
  * In this example I assume we're only using the https one. The http endpoint should work as well, you will need to note this port too and add it to the Dev Tunnel command a little later.
* Make sure Dev Tunnels is installed: https://learn.microsoft.com/azure/developer/dev-tunnels/get-started
* Manually start a Dev Tunnel, for example: `devtunnel host --allow-anonymous --protocol https -p 21104 -p 7291` note the Dev Tunnel ID. For example, if the resulting URL is `https://lgtm1234-7291.euw.devtunnels.ms`, the ID is `lgtm1234`
* Make sure to click the link and open each endpoint (shown in the console output) in the browser. You will need to manually accept once for each endpoint, to allow traffic to go through the tunnel.
* With the Dev Tunnel ID go back to the **launchSettings.json** file of the AppHost project and add a new entry under `EnvironmentVariables`: add [`DEVTUNNEL_ID`](https://github.com/jfversluis/MauiAspire/blob/main/MauiAspire.AppHost/Properties/launchSettings.json#L14) with the ID you just got from the Dev Tunnel:
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