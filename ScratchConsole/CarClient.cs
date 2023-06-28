using Microsoft.Extensions.Logging;
using ScratchConsole.FluidHttp.Result;
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

        public async Task<RestResult<Car>> FindCarNoProblemAsync(string carId, CancellationToken token = default)
        {
            var url = $"car?name={carId}";
            return await _client.Get<Car>(url).SendAsync(token);
        }

        public async Task<RestResult<Car, ProblemDetails>> FindCarAsync(string carId, CancellationToken token = default)
        {
            var url = $"car?name={carId}";

            var result = await _client
                .Get<Car>(url)
                .WithProblemDetails<ProblemDetails>(400, 500)
                .SendAsync(token);

            return result;
        }

        public async Task<RestResult<CarUpdateResponse, ProblemDetails>> UpdateCar(CarUpdate update, CancellationToken token = default)
        {
            var url = "/car/123";

            var result = await _client
                .Post<CarUpdate, CarUpdateResponse>(url, update)
                .WithProblemDetails<ProblemDetails>(400, 500)
                .SendAsync(token);

            return result;
        }

        public async Task<RestUnitResult<ProblemDetails>> CreateCar(CarCreate createCar, CancellationToken token = default)
        {
            var url = "/car";

            var result = await _client
                .Post(url, createCar)
                .WithProblemDetails<ProblemDetails>(400, 500)
                .SendAsync(token);

            return result;
        }

        public async Task<RestResult> StartCar(CarCreate createCar, CancellationToken token = default)
        {
            var url = "/car";

            return await 
                TryHttp(() => 
                    _client
                        .Post(url, createCar)
                        .SendAsync(token));
        }

        private static async Task<TResult> TryHttp<TResult>(
            Func<Task<TResult>> func, 
            Func<TResult> defaultValueOnFailure = default)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                return defaultValueOnFailure();
            }
        }
    }
}