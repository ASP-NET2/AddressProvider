using AddressLibrary.Data.Context;
using AddressLibrary.Data.Entities;
using AddressLibrary.Migrations;
using AddressProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddressProvider.Functions
{
    public class GetAddressFunction(ILogger<GetAddressFunction> logger, DataContext context)
    {
        private readonly ILogger<GetAddressFunction> _logger = logger;
        private readonly DataContext _context = context;

        [Function("GetAddressFunction")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "user/{accountId}/addresses")] HttpRequest req, int accountId)
        {
            _logger.LogInformation($"Received request for addresses of user ID: {accountId}");
            try
            {
               var addresses = await _context.Addresses
                    .Where(a => a.AccountId == accountId)
                .ToListAsync();

                if(addresses == null || !addresses.Any())
                {
                    _logger.LogError($"No addresses found for this user id{accountId}");
                    return new OkObjectResult(new List<AddressEntity>());
                }

                return new OkObjectResult(addresses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
       
    }

}
    

