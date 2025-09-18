using LRA.Account.DBInfrastructure.Seeds.Configurations;
using Microsoft.EntityFrameworkCore;

namespace LRA.Account.DBInfrastructure.Seeds;

public static class SeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new RoleSeedConfiguration());
        modelBuilder.ApplyConfiguration(new SuperAdminSeedConfiguration());
    }
}
