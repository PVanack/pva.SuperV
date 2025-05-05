var builder = DistributedApplication.CreateBuilder(args);

var tdEngine = builder.AddTdEngine("tdEngine");

var api = builder.AddProject<Projects.pva_SuperV_Api>("api")
    .WithReference(tdEngine).WaitFor(tdEngine);
var frontend = builder.AddProject<Projects.pva_SuperV_Blazor>("frontend")
    .WithReference(api).WaitFor(api);
await builder.Build().RunAsync();
