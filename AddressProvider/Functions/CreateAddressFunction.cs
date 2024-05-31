using AddressLibrary.Data.Context;
using AddressLibrary.Data.Entities;
using AddressProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace AddressProvider.Functions
{
    public class CreateAddressFunction(ILogger<CreateAddressFunction> logger, DataContext context)
    {
        private readonly ILogger<CreateAddressFunction> _logger = logger;
        private readonly DataContext _context = context;

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

                var accountUser = await _context.AccountUser.FirstOrDefaultAsync(a => a.AccountId == data.AccountId);

                if(accountUser == null)
                {
                    return new NotFoundObjectResult($"Did not find a user {accountUser}");
                }

                _context.Addresses.Add(data);
                await _context.SaveChangesAsync();

                return new OkObjectResult(data);

            }catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new BadRequestObjectResult($"Failed to run {ex.Message}");
            }

        }
    }
}
