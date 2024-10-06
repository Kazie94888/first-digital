using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Backoffice.Api;
using SmartCoinOS.Extensions;
using SmartCoinOS.Infrastructure.DependencyInjection;
using SmartCoinOS.Infrastructure.Extensions;
using SmartCoinOS.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddLogging();
builder.AddConfiguration();
builder.AddJsonOptions();
builder.Services.AddDatabase(builder.Configuration);
builder.AddBlockchainSettings();

builder.AddInfrastructure();
builder.AddDomainServices();
builder.AddDomainEventHandlers();

builder.AddHttpClients();
builder.AddAwsServices();
builder.AddWebhooks();
builder.AddMonitoring();

builder.AddApplication();
builder.Services.AddEndpointsApiExplorer();
builder.AddApiDocs();
builder.AddValidationSchemaProvider();

builder.AddCors();
builder.AddAuthentication();
builder.AddAuthorization();
builder.AddEndpoints();
builder.AddErrorHandlers();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseOpenApi();
    app.UseReDoc();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(o =>
    {
        /* empty delegate: https://github.com/dotnet/aspnetcore/issues/52622 */
    });
}

app.UseCors();
app.UseOutputCache();
app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();

await using (var scope = app.Services.CreateAsyncScope())
{
    await using var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
}

await app.RunAsync();
