using AddressLibrary.Data.Context;
using AddressProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AddressProvider.Functions
{
    public class DeleteAddressFunction
    {
        private readonly ILogger<DeleteAddressFunction> _logger;
        private readonly DeleteService _deleteService;

        public DeleteAddressFunction(ILogger<DeleteAddressFunction> logger, DeleteService deleteService)
        {
            _logger = logger;
            _deleteService = deleteService;
        }

        [Function("DeleteAddressFunction")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route ="addresses/{addressId}")] HttpRequest req, int addressId)
        {
            _logger.LogInformation($"Recieved request to delete address ID: {addressId}");

            try
            {
                var existingAddress = await _deleteService.DeleteAddressAsync(addressId);
                if(!existingAddress)
                {
                    return new NotFoundObjectResult("The address is not found");
                }

                return new OkObjectResult("The address deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
