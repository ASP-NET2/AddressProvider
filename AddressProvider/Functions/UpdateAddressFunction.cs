using AddressLibrary.Data.Context;
using AddressProvider.Models;
using AddressProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace AddressProvider.Functions
{
    public class UpdateAddressFunction
    {
        private readonly ILogger<UpdateAddressFunction> _logger;
        private readonly UpdateService _service;

        public UpdateAddressFunction(ILogger<UpdateAddressFunction> logger, UpdateService service)
        {
            _logger = logger;
            _service = service;
        }

        [Function("UpdateAddressFunction")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route ="addresses/{addressId}")] HttpRequest req, int addressId)
        {
            _logger.LogInformation($"Recieved request to update address Id {addressId}");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var updatedAddress = JsonConvert.DeserializeObject<UpdateAddressModel>(requestBody);

                if (updatedAddress == null)
                {
                    return new BadRequestObjectResult("Invalid data");
                }

                var result = await _service.UpdateAddressAsync(addressId, updatedAddress);

                if (result == null)
                {
                    return new NotFoundObjectResult("The address that you were looking for could not be found");
                }

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the entity changes.");
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception details.");
                }
                return new BadRequestObjectResult(new { error = ex.Message, innerException = ex.InnerException?.Message });
            }
        }
    }
}
