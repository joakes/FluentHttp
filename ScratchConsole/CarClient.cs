using Microsoft.Extensions.Logging;
using ScratchConsole.HttpExtensions;
using ScratchConsole.Models;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ScratchConsole.Test")]
namespace ScratchConsole
{
    internal class CarClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<CarClient> _logger;

        public CarClient(HttpClient client, ILogger<CarClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<HttpResultWithResponseProblemDetails<Car, ProblemDetails>> FindCarAsync(string carId, CancellationToken token = default)
        {
            var url = $"car?name={carId}";

            var result = await _client
                .Get<Car>(url)
                .WithProblemDetails<ProblemDetails>(400, 500)
                .SendAsync(_logger, token);
            
            return result;
        }

        public async Task<HttpResultWithResponseProblemDetails<CarUpdateResponse, ProblemDetails>> UpdateCar(CarUpdate update, CancellationToken token = default)
        {
            var url = "/car/123";

            var result = await _client
                .Post<CarUpdate, CarUpdateResponse>(url, update)
                .WithProblemDetails<ProblemDetails>(400, 500)
                .SendAsync(_logger, token);

            return result;
        }

        public async Task<HttpResultWithProblemDetails<ProblemDetails>> CreateCar(CarCreate createCar, CancellationToken token = default)
        {
            var url = "/car";

            var result = await _client
                .Post(url, createCar)
                .WithProblemDetails<ProblemDetails>(400, 500)
                .SendAsync(_logger, token);

            return result;
        }
    }
}