using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartCoinOS.SmartCoinTool.Common;
using SmartCoinOS.SmartCoinTool.DataSeed;
using Spectre.Console;
using Spectre.Console.Cli;


Host.CreateApplicationBuilder(args);

var registrar = new TypeRegistrar(new ServiceCollection());
var cmdApp = new CommandApp(registrar);

cmdApp.Configure(x =>
{
    x.AddCommand<DataSeedCommand>("seed")
        .WithDescription("Generate seed data");

    x.SetApplicationName("smart-coin");

    var interceptor = new CancellationInterceptor();
    Console.CancelKeyPress += interceptor.OnCancel;
    x.SetInterceptor(new CancellationInterceptor());
});

try
{
    return await cmdApp.RunAsync(args);
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
    throw;
}
