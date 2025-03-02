using Articles.Application;
using Articles.Infrastructure;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Articles.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Articles API", 
        Version = "v1",
        Description = "API do zarządzania artykułami"
    });
    
    var xmlFile = "Articles.API.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Zastosuj migracje przy starcie aplikacji
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Wystąpił błąd podczas migracji bazy danych.");
    }
}

// Włącz Swagger dla wszystkich środowisk
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Articles API V1");
    c.RoutePrefix = string.Empty; // Ustawia Swagger UI jako stronę główną
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

namespace Articles.API
{
    public partial class Program { }
}
