using DoaMais.StockService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<StockWorker>();

var host = builder.Build();
host.Run();
