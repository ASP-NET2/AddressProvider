using AddressLibrary.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AddressProvider.Functions
{
    public class DeleteAddressFunction
    {
        private readonly ILogger<DeleteAddressFunction> _logger;
        private readonly DataContext _context;

        public DeleteAddressFunction(ILogger<DeleteAddressFunction> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("DeleteAddressFunction")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route ="addresses/{addressId}")] HttpRequest req, int addressId)
        {
            _logger.LogInformation($"Recieved request to delete address ID: {addressId}");

            try
            {
                var existingAddress = await _context.Addresses.FindAsync(addressId);
                if(existingAddress == null)
                {
                    return new NotFoundObjectResult("The address is not found");
                }

                _context.Addresses.Remove(existingAddress);
                await _context.SaveChangesAsync();

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
