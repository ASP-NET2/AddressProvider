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

public class GetAddressServiceTests
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

        var address = new AddressEntity
        {
            AddressId = 1,
            AccountId = 1,
            AddressLine_1 = "123 Main St",
            City = "Anytown"
        };

        context.AccountUser.Add(accountUser);
        context.Addresses.Add(address);
        context.SaveChanges();
    }

    [Fact]
    public async Task GetAddressesByAccountIdAsync_ValidAccountId_ShouldReturnAddresses()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<GetAddressService>>();
        var addressService = new GetAddressService(context, loggerMock.Object);

        // Act
        var result = await addressService.GetAddressesByAccountIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("123 Main St", result.First().AddressLine_1);
    }

    [Fact]
    public async Task GetAddressesByAccountIdAsync_InvalidAccountId_ShouldReturnEmptyList()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<GetAddressService>>();
        var addressService = new GetAddressService(context, loggerMock.Object);

        // Act
        var result = await addressService.GetAddressesByAccountIdAsync(99);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}

