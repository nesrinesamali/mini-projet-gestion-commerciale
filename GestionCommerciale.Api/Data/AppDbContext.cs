using GestionCommerciale.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionCommerciale.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ClientAddress> ClientAddresses => Set<ClientAddress>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(e =>
        {
            e.Property(c => c.LastName).HasMaxLength(100).IsRequired();
            e.Property(c => c.FirstNameOrCompany).HasMaxLength(150).IsRequired();
            e.Property(c => c.Email).HasMaxLength(150).IsRequired();
            e.Property(c => c.Phone).HasMaxLength(30);
            e.HasMany(c => c.Addresses).WithOne(a => a.Client)
                .HasForeignKey(a => a.ClientId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.Property(p => p.Reference).HasMaxLength(50).IsRequired();
            e.HasIndex(p => p.Reference).IsUnique();
            e.Property(p => p.Name).HasMaxLength(150).IsRequired();
            e.Property(p => p.Description).HasMaxLength(500);
            e.Property(p => p.UnitPriceHt).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.Property(o => o.OrderNumber).HasMaxLength(30).IsRequired();
            e.HasIndex(o => o.OrderNumber).IsUnique();
            e.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(o => o.TotalHt).HasPrecision(18, 2);
            e.Property(o => o.TotalTtc).HasPrecision(18, 2);
            e.HasOne(o => o.Client).WithMany(c => c.Orders)
                .HasForeignKey(o => o.ClientId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(o => o.Lines).WithOne(l => l.Order)
                .HasForeignKey(l => l.OrderId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderLine>(e =>
        {
            e.Property(l => l.UnitPrice).HasPrecision(18, 2);
            e.Property(l => l.LineTotal).HasPrecision(18, 2);
            e.HasOne(l => l.Product).WithMany()
                .HasForeignKey(l => l.ProductId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AppUser>(e =>
        {
            e.Property(u => u.Username).HasMaxLength(50).IsRequired();
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.PasswordHash).IsRequired();
        });
    }
}