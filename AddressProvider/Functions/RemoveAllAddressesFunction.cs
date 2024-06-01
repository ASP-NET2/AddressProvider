using System;
using System.Threading.Tasks;
using AddressLibrary.Data.Context;
using AddressProvider.Services;
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
        private readonly RemoveAllService _service;

        public RemoveAllAddressesFunction(ILogger<RemoveAllAddressesFunction> logger, RemoveAllService service)
        {
            _logger = logger;
            _service = service;
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

                if (accountId != 0)
                {
                    await _service.RemoveAllAddressesAsync(accountId);
                }
                else
                {
                    _logger.LogWarning("Received invalid AccountId");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }

        }
    }
}
