using AddressLibrary.Data.Context;
using AddressProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace AddressProvider.Functions
{
    public class UpdateAddressFunction(ILogger<UpdateAddressFunction> logger, DataContext context)
    {
        private readonly ILogger<UpdateAddressFunction> _logger = logger;
        private readonly DataContext _context = context;

        [Function("UpdateAddressFunction")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route ="addresses/{addressId}")] HttpRequest req, int addressId)
        {
            _logger.LogInformation($"Recieved request to update address Id {addressId}");

            try
            {
                var existingAddress = await _context.Addresses.FirstOrDefaultAsync(x => x.AddressId == addressId);

                if(existingAddress == null)
                {
                    _logger.LogWarning($"No address found with ID: {addressId}");
                    return new NotFoundObjectResult("The address that you where looking for could not be found");
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var updatedAddress = JsonConvert.DeserializeObject<UpdateAddressModel>(requestBody);

                if(updatedAddress == null)
                {
                    return new BadRequestObjectResult("Invalid data");
                }

                existingAddress.AddressTitle = updatedAddress.AddressTitle;
                existingAddress.AddressLine_1 = updatedAddress.AddressLine_1;
                existingAddress.PostalCode = updatedAddress.PostalCode;
                existingAddress.City = updatedAddress.City;

               _context.Addresses.Update(existingAddress);
                _context.SaveChanges();

                return new OkObjectResult(updatedAddress);

            }catch (Exception ex)
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
