using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientFacingController : ControllerBase
    {
        private readonly ILogger<ClientFacingController> _logger;
        private readonly ISimulateHttpRequestTimeout _simulator;

        public ClientFacingController(ILogger<ClientFacingController> logger,ISimulateHttpRequestTimeout simulator)
        {
            _logger = logger;
            _simulator = simulator;
        }

        public class OutgoingResponse
        {
            public string Message { get; }
            public HttpStatusCode HttpStatusCode { get; }

            public OutgoingResponse(string message, HttpStatusCode httpStatusCode)
            {
                Message = message;
                HttpStatusCode = httpStatusCode;
            }
        }

        [HttpGet("simulator")]
        public async Task<OutgoingResponse> GetSimulation()
        {
            var simulation = await _simulator.Get();
            if (simulation.StatusCode == HttpStatusCode.RequestTimeout)
            {
                return new OutgoingResponse("Can't serve your request now", HttpStatusCode.InternalServerError);
            }
            
            return new OutgoingResponse(await simulation.Content.ReadAsStringAsync(), simulation.StatusCode );
        }
    }
}
