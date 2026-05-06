using ECommerce.Application.Interfaces.Seed;
using ECommerce.Infrastructure.Persistence.Seed.Identity;



namespace ECommerce.Infrastructure.Persistence.Seed;


public class DataSeeder : IDataSeeder
{
    
    private readonly RoleSeeder _roleSeeder;
    private readonly AdminSeeder _adminSeeder;

    public DataSeeder(
        RoleSeeder roleSeeder,
        AdminSeeder adminSeeder)
    {
        _roleSeeder = roleSeeder;
        _adminSeeder = adminSeeder;
    }

    public async Task SeedAsync()
    {
        await _roleSeeder.SeedAsync();
        await _adminSeeder.SeedAsync();
    }
    
}