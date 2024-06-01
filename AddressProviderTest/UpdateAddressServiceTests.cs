
using AddressLibrary.Data.Context;
using AddressLibrary.Data.Entities;
using AddressProvider.Models;
using AddressProvider.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace AddressProviderTest;

public class UpdateAddressServiceTests
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
            AddressTitle = "Home",
            AddressLine_1 = "123 Main St",
            PostalCode = "12345",
            City = "Anytown"
        };

        context.AccountUser.Add(accountUser);
        context.Addresses.Add(address);
        context.SaveChanges();
    }

    [Fact]
    public async Task UpdateAddressAsync_ValidAddressId_ShouldReturnUpdatedAddress()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<UpdateService>>();
        var addressService = new UpdateService(context, loggerMock.Object);

        var updatedAddress = new UpdateAddressModel
        {
            AddressTitle = "Office",
            AddressLine_1 = "456 Oak St",
            PostalCode = "67890",
            City = "Othertown"
        };

        // Act
        var result = await addressService.UpdateAddressAsync(1, updatedAddress);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Office", result.AddressTitle);
        Assert.Equal("456 Oak St", result.AddressLine_1);
        Assert.Equal("67890", result.PostalCode);
        Assert.Equal("Othertown", result.City);
    }

    [Fact]
    public async Task UpdateAddressAsync_InvalidAddressId_ShouldReturnNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<UpdateService>>();
        var addressService = new UpdateService(context, loggerMock.Object);

        var updatedAddress = new UpdateAddressModel
        {
            AddressTitle = "Office",
            AddressLine_1 = "456 Oak St",
            PostalCode = "67890",
            City = "Othertown"
        };

        // Act
        var result = await addressService.UpdateAddressAsync(99, updatedAddress);

        // Assert
        Assert.Null(result);
    }
}
