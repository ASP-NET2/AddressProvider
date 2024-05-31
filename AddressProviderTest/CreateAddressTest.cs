using Moq;
using Microsoft.Extensions.Logging;
using AddressLibrary.Data.Context;
using AddressLibrary.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using AddressProvider.Functions;
using Microsoft.VisualStudio.Services.Account;

namespace AddressProviderTest;

public class CreateAddressFunctionTests
{
    private readonly Mock<ILogger<CreateAddressFunction>> _mockLogger;
    private readonly DataContext _context;

    public CreateAddressFunctionTests()
    {
        _mockLogger = new Mock<ILogger<CreateAddressFunction>>();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDb")
            .Options;

        _context = new DataContext(options);
    }

    private HttpRequest CreateHttpRequest(object body)
    {
        var context = new DefaultHttpContext();
        var request = context.Request;
        var json = JsonConvert.SerializeObject(body);
        request.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
        request.ContentType = "application/json";
        return request;
    }

    [Fact]
    public async Task Run_ReturnsBadRequest_WhenAddressDataIsInvalid()
    {
        // Arrange
        var function = new CreateAddressFunction(_mockLogger.Object, _context);
        var context = new DefaultHttpContext();
        var request = context.Request;
        request.Body = new MemoryStream(Encoding.UTF8.GetBytes("invalid data"));

        // Act
        var result = await function.Run(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid address data", badRequestResult.Value);
    }

    [Fact]
    public async Task Run_ReturnsNotFound_WhenAccountUserDoesNotExist()
    {
        // Arrange
        var addressEntity = new AddressEntity { AccountId = 1 };
        var function = new CreateAddressFunction(_mockLogger.Object, _context);
        var request = CreateHttpRequest(addressEntity);

        // Act
        var result = await function.Run(request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Did not find a user with accountId 1", notFoundResult.Value);
    }

    [Fact]
    public async Task Run_ReturnsOk_WhenAddressIsCreated()
    {
        // Arrange
        var addressEntity = new AddressEntity { AccountId = 1 };
        var accountUserEntity = new AccountUserEntity { AccountId = 1, FirstName = "John", LastName = "Doe" };

        _context.AccountUser.Add(accountUserEntity);
        await _context.SaveChangesAsync();

        var function = new CreateAddressFunction(_mockLogger.Object, _context);
        var request = CreateHttpRequest(addressEntity);

        // Act
        var result = await function.Run(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(addressEntity, okResult.Value);
    }

    [Fact]
    public async Task Run_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var addressEntity = new AddressEntity { AccountId = 1 };
        var function = new CreateAddressFunction(_mockLogger.Object, _context);
        var request = CreateHttpRequest(addressEntity);

        // Mocka SaveChangesAsync för att kasta ett undantag
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _context.Addresses.Add(new AddressEntity());  // Lägg till en dummy-post
        await _context.SaveChangesAsync();

        // För att framkalla en konflikt eller liknande fel som kan kasta undantag
        _context.Addresses.Add(addressEntity);
        await _context.SaveChangesAsync();

        // Act
        var result = await function.Run(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.StartsWith("Failed to run", badRequestResult.Value.ToString());
    }
}

