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
        Description = "API for managing articles"
    });
    
    var xmlFile = "Articles.API.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        var retryCount = 0;
        const int maxRetries = 10;
        
        while (retryCount < maxRetries)
        {
            try
            {
                context.Database.Migrate();
                break;
            }
            catch (Exception ex)
            {
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    throw;
                }
                
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogWarning(ex, "Attempt {RetryCount}/{MaxRetries} to connect to the database failed. Retrying in 5 seconds...", retryCount, maxRetries);
                
                Thread.Sleep(5000);
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration.");
    }
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Articles API V1");
    c.RoutePrefix = string.Empty;
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
