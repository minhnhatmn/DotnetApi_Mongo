using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Host.UseMetricsWebTracking()
            .UseMetrics(options =>
            {
                options.EndpointOptions = endpointOptions =>
                {
                    endpointOptions.MetricsEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
                    endpointOptions.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
                    endpointOptions.EnvironmentInfoEndpointEnabled = false;
                };
            });
// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200");
                      });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });
builder.Services.AddMetrics();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(options =>
{
    options.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();



});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
