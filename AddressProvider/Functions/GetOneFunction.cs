using AddressLibrary.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AddressProvider.Functions
{
    public class GetOneFunction
    {
        private readonly ILogger<GetOneFunction> _logger;
        private readonly DataContext _context;

        public GetOneFunction(ILogger<GetOneFunction> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("GetOneFunction")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route ="addresses/{addressId}")] HttpRequest req, int addressId)
        {
            _logger.LogInformation($"Recieved Request for this addressId{addressId}");
            try
            {
                var address = await _context.Addresses.FirstOrDefaultAsync(x=> x.AddressId == addressId);
                
                if(address == null)
                {
                    _logger.LogWarning($"No address found with Id: {addressId}");
                    return new NotFoundObjectResult("Address not found");
                }

                return new OkObjectResult(address);


            }catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
