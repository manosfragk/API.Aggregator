using API.Aggregator.Interfaces;
using API.Aggregator.Services;
using API_Aggregator.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient<IOpenWeatherMapService, OpenWeatherMapService>();
builder.Services.AddHttpClient<IIpGeolocationService, IpGeolocationService>();
builder.Services.AddHttpClient<INewsService, NewsService>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Aggregation Service", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();  //app.Run();
