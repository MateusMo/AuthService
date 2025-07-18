using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [HttpGet]
    [Route("")]
    public IActionResult Root()
    {
        return Ok(new { 
            message = "AuthService API está funcionando!", 
            endpoints = new[] {
                "/swagger",
                "/api/Funcionario",
                "/api/Login/authenticate",
                "/api/Login/healthy"
            },
            timestamp = DateTime.UtcNow 
        });
    }
    
    [HttpGet]
    [Route("health")]
    public IActionResult Health()
    {
        return Ok(new { 
            status = "Healthy", 
            service = "AuthService",
            timestamp = DateTime.UtcNow 
        });
    }
}