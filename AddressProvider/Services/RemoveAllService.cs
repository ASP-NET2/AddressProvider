using AddressLibrary.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressProvider.Services
{
    public class RemoveAllService
    {
        private readonly DataContext _context;
        private readonly ILogger<RemoveAllService> _logger;

        public RemoveAllService(DataContext context, ILogger<RemoveAllService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task RemoveAllAddressesAsync(int accountId)
        {
            _logger.LogInformation($"Received request to remove all addresses for AccountId: {accountId}");

            try
            {
                var addresses = await _context.Addresses
                    .Where(a => a.AccountId == accountId)
                    .ToListAsync();

                _context.Addresses.RemoveRange(addresses);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Deleted {addresses.Count} addresses for AccountId {accountId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing addresses");
                throw;
            }
        }
    }
}
