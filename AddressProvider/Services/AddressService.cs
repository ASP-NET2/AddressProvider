using AddressLibrary.Data.Context;
using AddressLibrary.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressProvider.Services;

public class AddressService
{
    private readonly DataContext _context;

    public AddressService(DataContext context)
    {
        _context = context;
    }

    public async Task<AddressEntity> CreateAddressAsync(AddressEntity data)
    {
        var accountUser = await _context.AccountUser.FirstOrDefaultAsync(a => a.AccountId == data.AccountId);

        if(accountUser == null)
        {
            throw new Exception($"Did not find a user with account ID {data.AccountId}");
        }

        _context.Addresses.Add(data);
        await _context.SaveChangesAsync();

        return data;

    }
}
