using AddressLibrary.Data.Context;
using AddressLibrary.Data.Entities;
using AddressProvider.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddressProvider.Services;

public class UpdateService
{
    private readonly DataContext _context;
    private readonly ILogger<UpdateService> _logger;

    public UpdateService(DataContext context, ILogger<UpdateService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AddressEntity> UpdateAddressAsync(int addressId, UpdateAddressModel updatedAddress)
    {
        _logger.LogInformation($"Received request to update address ID: {addressId}");

        try
        {
            var existingAddress = await _context.Addresses.FirstOrDefaultAsync(x => x.AddressId == addressId);

            if (existingAddress == null)
            {
                _logger.LogWarning($"No address found with ID: {addressId}");
                return null;
            }

            existingAddress.AddressTitle = updatedAddress.AddressTitle;
            existingAddress.AddressLine_1 = updatedAddress.AddressLine_1;
            existingAddress.PostalCode = updatedAddress.PostalCode;
            existingAddress.City = updatedAddress.City;

            _context.Addresses.Update(existingAddress);
            await _context.SaveChangesAsync();

            return existingAddress;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving the entity changes.");
            if (ex.InnerException != null)
            {
                _logger.LogError(ex.InnerException, "Inner exception details.");
            }
            throw;
        }
    }
}
