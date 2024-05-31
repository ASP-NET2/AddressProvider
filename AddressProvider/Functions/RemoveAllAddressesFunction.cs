using System;
using System.Threading.Tasks;
using AddressLibrary.Data.Context;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AddressProvider.Functions
{
    public class RemoveAllAddressesFunction
    {
        private readonly ILogger<RemoveAllAddressesFunction> _logger;
        private readonly DataContext _context;
        public RemoveAllAddressesFunction(ILogger<RemoveAllAddressesFunction> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function(nameof(RemoveAllAddressesFunction))]
        public async Task Run(
            [ServiceBusTrigger("account-request", Connection = "ServiceBusConnection")]
           string message)
        {
            _logger.LogInformation("Message ID: {id}", message);

            try
            {
                var accountId = JsonConvert.DeserializeObject<int>(message);

                if(accountId != 0)
                {
                    var addresses = await _context.Addresses
                        .Where(a => a.AccountId == accountId)
                        .ToListAsync();

                    _context.Addresses.RemoveRange(addresses);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Deleted {addresses.Count} addresses for AccountId {accountId}");
                }
                else
                {
                    _logger.LogWarning("Received invalid AccountId");
                }



            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
           
        }
    }
}
