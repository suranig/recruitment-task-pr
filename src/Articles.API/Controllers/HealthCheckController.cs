using Microsoft.AspNetCore.Mvc;

namespace Articles.API.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    [Route("/health")]
    public IActionResult Get()
    {
        return Ok(new { Status = "Healthy" });
    }
} 