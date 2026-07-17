using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ------------ CONFIGURAÇÃO UNIFICADA DO OPENTELEMETRY ------------
string applicationName = builder.Configuration["ApplicationName"] ?? "dashboardTelemetria";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName: applicationName))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddSource(applicationName)
            .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources"); // Escuta as atividades do Mongo

        // Captura o Host e a Porta do Jaeger baseado no seu JSON original
        var jaegerHost = builder.Configuration["Jaeger:Host"] ?? "http://localhost";
        var jaegerPort = builder.Configuration["Jaeger:Port"] ?? "4317";
        var baseUri = jaegerHost.StartsWith("http") ? $"{jaegerHost}:{jaegerPort}" : $"http://{jaegerHost}:{jaegerPort}";

        tracing.AddOtlpExporter(opt =>
        {
            opt.Endpoint = new Uri(baseUri);
            opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        });
    })
    .WithMetrics(metrics =>
    {
        metrics.AddMeter(applicationName);
        metrics.AddAspNetCoreInstrumentation(); // Coleta métricas de requisições HTTP (Prometheus)
        metrics.AddPrometheusExporter();        // Habilitado pelo novo pacote instalado!
    });

// ------------ HEALTH CHECKS ------------
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API OK"))
    .AddMongoDb(
        sp =>
        {
            var mongoSection = builder.Configuration.GetSection("MongoDb");
            var cs = mongoSection["ConnectionString"] ?? "mongodb://host.docker.internal:27017";

            var mongoUrl = new MongoUrl(cs);
            var clientSettings = MongoClientSettings.FromUrl(mongoUrl);

            // Vincula o Driver do MongoDB com a fonte de monitoramento do OpenTelemetry
            clientSettings.ClusterConfigurator = cb => cb.Subscribe(new MongoDB.Driver.Core.Extensions.DiagnosticSources.DiagnosticsActivityEventSubscriber());

            return new MongoClient(clientSettings);
        },
        name: "mongodb",
        timeout: TimeSpan.FromSeconds(5),
        tags: new[] { "db", "mongo" }
    );

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "Minha API v1");
    });
}

app.UseCors("AllowAll");

// Cria e expõe a rota automática http://localhost:<sua-porta>/metrics para o Prometheus ler
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();