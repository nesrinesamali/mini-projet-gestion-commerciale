using GestionCommerciale.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestionCommerciale.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration config)
    {
        var db = services.GetRequiredService<AppDbContext>();

        if (await db.Users.AnyAsync())
        {
            return;
        }

        var admin = new AppUser { Username = config["AdminSeed:Username"]! };
        admin.PasswordHash = new PasswordHasher<AppUser>().HashPassword(admin, config["AdminSeed:Password"]!);

        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}