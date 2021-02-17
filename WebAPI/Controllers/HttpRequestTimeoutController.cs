using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HttpRequestTimeoutController : ControllerBase
    {
        private readonly ILogger<HttpRequestTimeoutController> _logger;
        private readonly CircuitBreakerSimulationHelper _circuitBreaker;
        private static int _called;

        public HttpRequestTimeoutController(ILogger<HttpRequestTimeoutController> logger, CircuitBreakerSimulationHelper circuitBreaker)
        {
            _logger = logger;
            _circuitBreaker = circuitBreaker;
        }

        [HttpGet]
        public IStatusCodeActionResult HttpRequestTimeout()
        {
            Console.WriteLine($"Called: {++_called}");

            if (_circuitBreaker.Closed())
            {
                _logger.LogInformation("Return OK");
                return Ok(Task.FromResult(42));
            }

            _logger.LogError("408");
            return StatusCode(408);
        }
    }
}
