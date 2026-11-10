var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.EventProcessor_Worker>("worker");

builder.Build().Run();
