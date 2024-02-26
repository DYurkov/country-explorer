using CountryExplorer.Infrastructure;
using CountryExplorer.Services;
using Microsoft.Net.Http.Headers;
using Serilog;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICountryCache, CountryCache>();
builder.Services.AddHttpClient<ICountryService, CountryService>();

builder.Services.AddSpaStaticFiles(z => z.RootPath = "Client/public");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.MapWhen(x => !x.Request.Path.Value.StartsWith("/api"), builder =>
{
    builder.UseSpaStaticFiles();
    builder.UseSpa(
        spa =>
        {
            spa.Options.SourcePath = "Client";

            spa.Options.DefaultPageStaticFileOptions =
                new StaticFileOptions
                {
                    // Do not cache implicit `/index.html`
                    OnPrepareResponse =
                        z =>
                        {
                            var headers = z.Context.Response.GetTypedHeaders();

                            headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
                        }
                };

            Log.Logger.Warning("Application was started with react dev server proxy");
            spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
        });
});

app.Run();
