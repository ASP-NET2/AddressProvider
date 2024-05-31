using AddressLibrary.Data.Entities;
using AddressLibrary.Migrations;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AddressLibrary.Data.Context;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<AccountUserEntity> AccountUser { get; set; }
    public DbSet<AddressEntity> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AddressEntity>()
            .ToTable("Addresses")
            .HasOne(a => a.AccountUser)
            .WithMany(a => a.Addresses)
            .HasForeignKey(a => a.AccountId);

        builder.Entity<AccountUserEntity>()
            .ToTable("AccountUser")
            .Property(a => a.AccountId).HasColumnName("AccountId");
      
    }
}
