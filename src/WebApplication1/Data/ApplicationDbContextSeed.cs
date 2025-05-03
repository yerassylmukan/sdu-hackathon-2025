using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data;

public class ApplicationDbContextSeed
{
    public static void SeedData(ApplicationDbContext context)
    {
        if (context.Database.IsNpgsql()) context.Database.Migrate();
    }
}