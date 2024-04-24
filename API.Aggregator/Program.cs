using API.Aggregator.Interfaces;
using API.Aggregator.Services;
using API_Aggregator.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Register services used by the application
builder.Services.AddControllers();                 // Enables controllers for handling API requests
builder.Services.AddMemoryCache();                 // Adds in-memory caching functionality (optional)
builder.Services.AddEndpointsApiExplorer();         // Enables API Explorer for metadata generation
builder.Services.AddHttpClient();
builder.Services.AddTransient<IOpenWeatherMapService, OpenWeatherMapService>();  // Registers OpenWeatherMap service with transient lifetime
builder.Services.AddTransient<IIpGeolocationService, IpGeolocationService>();  // Registers IpGeolocation service with transient lifetime
builder.Services.AddTransient<INewsService, NewsService>();                    // Registers News service with transient lifetime
builder.Services.AddSwaggerGen(c =>                // Configures Swagger generation for API documentation
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Aggregation Service", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline for processing requests
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Enables Swagger UI for interactive API documentation (during development)
}

app.UseHttpsRedirection();   // Redirects HTTP requests to HTTPS for security (optional)
app.UseStaticFiles();        // Enables serving static files (if applicable)
app.UseAuthorization();      // Enables authorization checks for API endpoints (optional)

// Maps controllers to the HTTP request pipeline for handling incoming requests
app.MapControllers();

app.Run();                     // Starts the web application and listens for incoming requests
