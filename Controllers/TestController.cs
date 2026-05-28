using Microsoft.AspNetCore.Mvc;

namespace TravellioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("working");
}