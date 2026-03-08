using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class RoleSeed
{
    public static readonly string AdminRoleId = "00000000-0000-0000-0000-000000000001";
    public static readonly string StaffRoleId = "00000000-0000-0000-0000-000000000002";
    public static readonly string CustomerRoleId = "00000000-0000-0000-0000-000000000003";

    public static void SeedRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = AdminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "00000000-0000-0000-0000-000000000001"
            },
            new IdentityRole
            {
                Id = StaffRoleId,
                Name = "Staff",
                NormalizedName = "STAFF",
                ConcurrencyStamp = "00000000-0000-0000-0000-000000000002"
            },
            new IdentityRole
            {
                Id = CustomerRoleId,
                Name = "Customer",
                NormalizedName = "CUSTOMER",
                ConcurrencyStamp = "00000000-0000-0000-0000-000000000003"
            }
        );
    }
}
