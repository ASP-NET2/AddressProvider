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
    public class GetOneService
    {
        private readonly DataContext _context;
        private readonly ILogger<GetOneService> _logger;

        public GetOneService(DataContext context, ILogger<GetOneService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AddressEntity> GetAddressByIdAsync(int addressId)
        {
            _logger.LogInformation($"Received request for address ID: {addressId}");

            try
            {
                var address = await _context.Addresses.FirstOrDefaultAsync(x => x.AddressId == addressId);

                if (address == null)
                {
                    _logger.LogWarning($"No address found with ID: {addressId}");
                }

                return address;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
