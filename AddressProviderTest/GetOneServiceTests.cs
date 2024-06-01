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

public class GetOneServiceTests
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
    public async Task GetAddressByIdAsync_ValidAddressId_ShouldReturnAddress()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<GetOneService>>();
        var addressService = new GetOneService(context, loggerMock.Object);

        // Act
        var result = await addressService.GetAddressByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("123 Main St", result.AddressLine_1);
    }

    [Fact]
    public async Task GetAddressByIdAsync_InvalidAddressId_ShouldReturnNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<GetOneService>>();
        var addressService = new GetOneService(context, loggerMock.Object);

        // Act
        var result = await addressService.GetAddressByIdAsync(99);

        // Assert
        Assert.Null(result);
    }
}
