using Microsoft.Extensions.Logging;
using ScratchConsole.FluentHttp;
using ScratchConsole.FluentHttp.Result;
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

            return await _client
                .Get<Car>(url)
                .WithExceptionHandler(ex => { })
                .SendAsync(token);
        }

        public async Task<RestResult<Car, ProblemDetails>> FindCarAsync(string carId, CancellationToken token = default)
        {
            var url = $"car?name={carId}";

            var result = await _client
                .Get<Car>(url)
                .WithProblemDetails<ProblemDetails>(400, 500)
                .WithExceptionHandler(ex => { })
                .SendAsync(token);

            return result;
        }

        public async Task<RestResult<UpdateCarResponse, ProblemDetails>> UpdateCar(UpdateCarRequest update, CancellationToken token = default)
        {
            var url = "/car/123";

            var result = await _client
                .Post<UpdateCarRequest, UpdateCarResponse>(url, update)
                .WithProblemDetails<ProblemDetails>(400, 500)
                .WithExceptionHandler(ex => { })
                .SendAsync(token);

            return result;
        }

        public async Task<RestUnitResult<ProblemDetails>> CreateCar(CarCreate createCar, CancellationToken token = default)
        {
            var url = "/car";

            var result = await _client
                .Post(url, createCar)
                .WithProblemDetails<ProblemDetails>(400, 500)
                .WithExceptionHandler(ex => { /* log this */ })
                .SendAsync(token);

            return result;
        }

        public async Task<RestResult> StartCar(CarCreate createCar, CancellationToken token = default)
        {
            var url = "/car";

            var result = await _client
                .Post(url, createCar)
                .WithExceptionHandler(ex => { })
                .SendAsync(token);

            return result;
        }
    }
}