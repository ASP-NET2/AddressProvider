using AddressLibrary.Data.Context;
using AddressLibrary.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressProvider.Services
{
    public class GetAddressService(DataContext context, ILogger<GetAddressService> logger)
    {
        private readonly DataContext _context = context;
        private readonly ILogger<GetAddressService> _logger = logger;

        public async Task<List<AddressEntity>> GetAddressesByAccountIdAsync(int accountId)
        {
            _logger.LogInformation($"Received request for addresses of user ID: {accountId}");

            try
            {
                var addresses = await _context.Addresses
                    .Where(a => a.AccountId == accountId)
                    .ToListAsync();

                if (addresses == null || !addresses.Any())
                {
                    _logger.LogInformation($"No addresses found for user ID: {accountId}");
                }

                return addresses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
