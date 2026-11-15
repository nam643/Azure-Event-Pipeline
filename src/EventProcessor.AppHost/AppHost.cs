var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator();

var blobs = storage.AddBlobContainer("blobs"); // resource blob

builder.AddProject<Projects.EventProcessor_Worker>("worker")
                                                .WithReference(blobs)
                                                .WaitFor(blobs);

builder.Build().Run();