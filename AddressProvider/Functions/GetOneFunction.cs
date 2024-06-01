using AddressLibrary.Data.Context;
using AddressProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddressProvider.Functions
{
    public class GetOneFunction
    {
        private readonly ILogger<GetOneFunction> _logger;
        private readonly GetOneService _service;

        public GetOneFunction(ILogger<GetOneFunction> logger, GetOneService service)
        {
            _logger = logger;
            _service = service;
        }

        [Function("GetOneFunction")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route ="addresses/{addressId}")] HttpRequest req, int addressId)
        {
            _logger.LogInformation($"Recieved Request for this addressId{addressId}");
            try
            {
                var address = await _service.GetAddressByIdAsync(addressId);

                if (address == null)
                {
                    return new NotFoundObjectResult("Address not found");
                }

                return new OkObjectResult(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
