using AddressLibrary.Data.Context;
using AddressLibrary.Data.Entities;
using AddressProvider.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace AddressProviderTest;

public class DeleteAddressFunctionTest
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
    public async Task DeleteAddressAsync_InvalidAddressId_ShouldReturnFalse()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<DeleteService>>();
        var addressService = new DeleteService(context, loggerMock.Object);

        // Act
        var result = await addressService.DeleteAddressAsync(99);

        // Assert
        Assert.False(result);
    }
    [Fact]
    public async Task DeleteAddressAsync_ValidAddressId_ShouldReturnTrue()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<DeleteService>>();
        var addressService = new DeleteService(context, loggerMock.Object);

        // Act
        var result = await addressService.DeleteAddressAsync(1);

        // Assert
        Assert.True(result);
        var deletedAddress = await context.Addresses.FindAsync(1);
        Assert.Null(deletedAddress);
    }
}


