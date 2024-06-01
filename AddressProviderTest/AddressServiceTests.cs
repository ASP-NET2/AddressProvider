using AddressLibrary.Data.Context;
using AddressLibrary.Data.Entities;
using AddressProvider.Services;
using Microsoft.EntityFrameworkCore;
using Moq;


namespace AddressProviderTest;

public class AddressServiceTests
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

        context.AccountUser.Add(accountUser);
        context.SaveChanges();
    }

    [Fact]
    public async Task CreateAddressAsync_ValidData_ShouldReturnCreatedAddress()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var addressService = new AddressService(context);

        var address = new AddressEntity
        {
            AccountId = 1,
            AddressLine_1 = "123 Main St",
            City = "Anytown"
        };

        // Act
        var result = await addressService.CreateAddressAsync(address);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(address.AccountId, result.AccountId);
        Assert.Equal(address.AddressLine_1, result.AddressLine_1);
        Assert.Equal(address.City, result.City);
    }

    [Fact]
    public async Task CreateAddressAsync_InvalidAccountId_ShouldThrowException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var addressService = new AddressService(context);

        var address = new AddressEntity
        {
            AccountId = -1,
            AddressLine_1 = "123 Main St",
            City = "Anytown"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => addressService.CreateAddressAsync(address));
        Assert.Equal("Did not find a user with account ID -1", exception.Message);
    }
}
