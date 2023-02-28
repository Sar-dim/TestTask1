using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Workers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.SetBasePath(Directory.GetCurrentDirectory());
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<MailSQLLoggerWorker>();
    })
    .Build();

await host.RunAsync();
