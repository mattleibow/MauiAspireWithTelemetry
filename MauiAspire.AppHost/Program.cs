var builder = DistributedApplication.CreateBuilder(args);

var webapi = builder.AddProject<Projects.SampleWebApi>("samplewebapi");

builder.AddProject<Projects.MauiAspire>("mauiaspire")
    .WithReference(webapi);

builder.Build().Run();
