using AddressLibrary.Data.Context;
using AddressLibrary.Data.Entities;
using AddressLibrary.Migrations;
using AddressProvider.Models;
using AddressProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddressProvider.Functions
{
    public class GetAddressFunction
    {
        private readonly ILogger<GetAddressFunction> _logger;
        private readonly GetAddressService _addressService;

        public GetAddressFunction(ILogger<GetAddressFunction> logger, GetAddressService addressService)
        {
            _logger = logger;
            _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
        }

        [Function("GetAddressFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "user/{accountId}/addresses")] HttpRequest req, int accountId)
        {
            try
            {
                var addresses = await _addressService.GetAddressesByAccountIdAsync(accountId);

                if (addresses == null || !addresses.Any())
                {
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
    

