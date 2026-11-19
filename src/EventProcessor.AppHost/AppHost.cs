var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator();

var blobs = storage.AddBlobContainer("blobs", "batch-summaries"); // resource blob


builder.AddProject<Projects.EventProcessor_Worker>("worker")
                                                .WithReference(blobs)
                                                .WaitFor(blobs);

builder.AddProject<Projects.EventProcessor_StatsApi>("statsapi")
                                                .WithReference(blobs)
                                                .WaitFor(blobs);

builder.Build().Run();