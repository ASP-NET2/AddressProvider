using AddressLibrary.Data.Context;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressProvider.Services
{
    public class DeleteService
    {
        private readonly DataContext _context;
        private readonly ILogger<DeleteService> _logger;

        public DeleteService(DataContext context, ILogger<DeleteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> DeleteAddressAsync(int addressId)
        {
            _logger.LogInformation($"Received request to delete address ID: {addressId}");

            try
            {
                var existingAddress = await _context.Addresses.FindAsync(addressId);
                if (existingAddress == null)
                {
                    _logger.LogInformation("The address is not found");
                    return false;
                }

                _context.Addresses.Remove(existingAddress);
                await _context.SaveChangesAsync();

                _logger.LogInformation("The address deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
