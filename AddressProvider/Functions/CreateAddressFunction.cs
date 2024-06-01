using AddressLibrary.Data.Context;
using AddressLibrary.Data.Entities;
using AddressProvider.Models;
using AddressProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace AddressProvider.Functions
{
    public class CreateAddressFunction
    {
        private readonly ILogger<CreateAddressFunction> _logger ;
        private readonly AddressService _addressService;

        public CreateAddressFunction(ILogger<CreateAddressFunction> logger, AddressService addressService)
        {
            _logger = logger;
            _addressService = addressService;
        }

        [Function("CreateAddressFunction")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route ="addresses")] HttpRequest req)
        {
            try
            {
                var data = await req.ReadFromJsonAsync<AddressEntity>();

                if(data == null)
                {
                    return new BadRequestObjectResult("Invalid address data");
                }

                var accountUser = await _addressService.CreateAddressAsync(data);

                return new OkObjectResult(data);

            }catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new BadRequestObjectResult($"Failed to run {ex.Message}");
            }

        }
    }
}
