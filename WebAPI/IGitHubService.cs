using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebAPI
{
    public interface ISimulateHttpRequestTimeout
    {
        Task<HttpResponseMessage> Get();
    }

    public class Simulator : ISimulateHttpRequestTimeout
    {
        private readonly ILogger<Simulator> _logger;
        private readonly HttpClient _client;

        public Simulator(HttpClient client, ILogger<Simulator> logger)
        {
            _logger = logger;

            _client = client;
            _client.BaseAddress = new Uri("https://localhost:5001/");
            // GitHub API versioning
            _client.DefaultRequestHeaders.Add("Accept",
                "application/vnd.github.v3+json");
            // GitHub requires a user-agent
            _client.DefaultRequestHeaders.Add("User-Agent",
                "HttpClientFactory-Sample");

            _client = client;
        }

        public async Task<HttpResponseMessage> Get()
        {
            var response = await _client.GetAsync("/api/HttpRequestTimeout");
            _logger.LogWarning($"Github get response code: {response.StatusCode}");
            return response;
        }
    }
}
