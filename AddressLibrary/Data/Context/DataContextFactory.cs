using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AddressLibrary.Data.Context;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
       var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseSqlServer("Server=tcp:authproviderserver.database.windows.net,1433;Initial Catalog=AuthDatabase;Persist Security Info=False;User ID=SqlAdmin;Password=Fredag5532!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        return new DataContext(optionsBuilder.Options);
    }
}
