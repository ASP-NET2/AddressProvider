using AddressLibrary.Data.Context;
using AddressLibrary.Data.Entities;
using AddressProvider.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressProviderTest;

public class RemoveAllAddressesServiceTests
{
    private DataContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_" + System.Guid.NewGuid().ToString())
            .Options;

        var context = new DataContext(options);
        SeedDatabase(context);
        return context;
    }

    private void SeedDatabase(DataContext context)
    {
        var accountUser = new AccountUserEntity
        {
            AccountId = 1,
            IdentityUserId = "user1",
            FirstName = "John",
            LastName = "Doe"
        };

        var address1 = new AddressEntity
        {
            AddressId = 1,
            AccountId = 1,
            AddressLine_1 = "123 Main St",
            City = "Anytown"
        };

        var address2 = new AddressEntity
        {
            AddressId = 2,
            AccountId = 1,
            AddressLine_1 = "456 Oak St",
            City = "Othertown"
        };

        context.AccountUser.Add(accountUser);
        context.Addresses.AddRange(address1, address2);
        context.SaveChanges();
    }

    [Fact]
    public async Task RemoveAllAddressesAsync_ValidAccountId_ShouldRemoveAddresses()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<RemoveAllService>>();
        var addressService = new RemoveAllService(context, loggerMock.Object);

        // Act
        await addressService.RemoveAllAddressesAsync(1);

        // Assert
        var remainingAddresses = await context.Addresses.Where(a => a.AccountId == 1).ToListAsync();
        Assert.Empty(remainingAddresses);
    }

    [Fact]
    public async Task RemoveAllAddressesAsync_InvalidAccountId_ShouldDoNothing()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<RemoveAllService>>();
        var addressService = new RemoveAllService(context, loggerMock.Object);

        // Act
        await addressService.RemoveAllAddressesAsync(99);

        // Assert
        var remainingAddresses = await context.Addresses.ToListAsync();
        Assert.Equal(2, remainingAddresses.Count); // Original 2 addresses should remain
    }
}

