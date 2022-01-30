using System.Threading.Tasks;
using Bev.RedSignal.Api.Models;
using Bev.RedSignal.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bev.RedSignal.Api.Controllers
{
    [ApiController]
    [Route("[controller]/token")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IJwtGenerator _jwtGenerator;

        public UserController(ILogger<UserController> logger,
                              IJwtGenerator jwtGenerator)
        {
            _logger = logger;
            _jwtGenerator = jwtGenerator;
        }

        [HttpPost]
        public IActionResult Login([FromBody] Auth auth)
        {
            _logger.LogInformation("Loging in...");

            if (string.IsNullOrWhiteSpace(auth.Username))
            {
                return BadRequest();
            }

            var token = _jwtGenerator.Generate(auth.Username);

            return Ok(new { token });
        }

    }
}