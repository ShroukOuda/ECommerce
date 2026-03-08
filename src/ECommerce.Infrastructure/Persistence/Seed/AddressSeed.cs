using ECommerce.Core.Entities.User;
using ECommerce.Core.Enums.Address;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class AddressSeed
{
    // Address ID pattern: sequential from 1
    private static readonly (string Street, string City, string State, string PostalCode, string Country)[] AddressPool =
    {
        // Cairo addresses
        ("15 El-Tahrir Street, Downtown", "Cairo", "Cairo Governorate", "11511", "Egypt"),
        ("23 Road 9, Maadi", "Cairo", "Cairo Governorate", "11728", "Egypt"),
        ("88 El-Nasr Road, Nasr City", "Cairo", "Cairo Governorate", "11371", "Egypt"),
        ("45 Shehab Street, Mohandiseen", "Cairo", "Giza Governorate", "12411", "Egypt"),
        ("12 El-Merghany Street, Heliopolis", "Cairo", "Cairo Governorate", "11341", "Egypt"),
        ("7 Abbas El-Akkad Street, Nasr City", "Cairo", "Cairo Governorate", "11765", "Egypt"),
        ("102 Corniche El Nile, Garden City", "Cairo", "Cairo Governorate", "11519", "Egypt"),
        ("33 El-Haram Street", "Giza", "Giza Governorate", "12556", "Egypt"),
        // Alexandria addresses
        ("56 Corniche Road, Stanley", "Alexandria", "Alexandria Governorate", "21500", "Egypt"),
        ("14 El-Horreya Road, Azarita", "Alexandria", "Alexandria Governorate", "21131", "Egypt"),
        ("78 Port Said Street, El-Mansheya", "Alexandria", "Alexandria Governorate", "21511", "Egypt"),
        ("29 Smouha, 14th of May", "Alexandria", "Alexandria Governorate", "21615", "Egypt"),
        // Luxor / Aswan
        ("8 Khaled Ibn El-Walid Street", "Luxor", "Luxor Governorate", "85951", "Egypt"),
        ("3 Corniche El Nile, West Bank", "Luxor", "Luxor Governorate", "85952", "Egypt"),
        ("17 Abtal El-Tahrir Street", "Aswan", "Aswan Governorate", "81511", "Egypt"),
        // Dubai addresses
        ("Villa 22, Al Barsha 1", "Dubai", "Dubai", "00000", "UAE"),
        ("Apt 1204, Marina Heights, Dubai Marina", "Dubai", "Dubai", "00000", "UAE"),
        ("Office 305, Business Bay Tower", "Dubai", "Dubai", "00000", "UAE"),
        ("Flat 502, JBR Walk, Jumeirah Beach", "Dubai", "Dubai", "00000", "UAE"),
        ("Building A7, International City", "Dubai", "Dubai", "00000", "UAE"),
        // Riyadh addresses
        ("King Fahd Road, Al-Olaya District", "Riyadh", "Riyadh Province", "12211", "Saudi Arabia"),
        ("Prince Sultan Street, Al-Sulaimaniyah", "Riyadh", "Riyadh Province", "11421", "Saudi Arabia"),
        ("Exit 15, Northern Ring Road", "Riyadh", "Riyadh Province", "13311", "Saudi Arabia"),
        // London addresses
        ("42 Baker Street", "London", "England", "W1U 3BW", "United Kingdom"),
        ("18 King's Road, Chelsea", "London", "England", "SW3 4RP", "United Kingdom"),
        ("7 Canary Wharf, Docklands", "London", "England", "E14 5AB", "United Kingdom"),
        ("55 Oxford Street", "London", "England", "W1D 2EQ", "United Kingdom"),
        // New York addresses
        ("350 Fifth Avenue, Suite 3400", "New York", "NY", "10118", "United States"),
        ("123 Broadway, Financial District", "New York", "NY", "10006", "United States"),
        ("456 Park Avenue, Upper East Side", "New York", "NY", "10022", "United States"),
        ("789 Madison Avenue", "New York", "NY", "10065", "United States"),
        // Paris addresses
        ("25 Rue de Rivoli, 1er", "Paris", "Île-de-France", "75001", "France"),
        ("12 Avenue des Champs-Élysées", "Paris", "Île-de-France", "75008", "France"),
        ("8 Boulevard Haussmann", "Paris", "Île-de-France", "75009", "France"),
        ("33 Rue du Faubourg Saint-Honoré", "Paris", "Île-de-France", "75008", "France"),
        // Additional global cities
        ("10 Unter den Linden", "Berlin", "Berlin", "10117", "Germany"),
        ("Via Roma 28", "Rome", "Lazio", "00184", "Italy"),
        ("Gran Vía 32", "Madrid", "Madrid", "28013", "Spain"),
        ("Shibuya Crossing 1-2-3", "Tokyo", "Tokyo", "150-0002", "Japan"),
        ("MG Road, Brigade Gateway", "Bangalore", "Karnataka", "560001", "India"),
        ("Nanjing Road 200", "Shanghai", "Shanghai", "200001", "China"),
        ("Av. Paulista 1578", "São Paulo", "SP", "01310-200", "Brazil"),
        ("Istiklal Avenue 45", "Istanbul", "Istanbul", "34433", "Turkey"),
        ("Kungsgatan 12", "Stockholm", "Stockholm", "111 35", "Sweden"),
        ("Rua Augusta 100", "Lisbon", "Lisbon", "1100-053", "Portugal"),
    };

    public static void SeedAddresses(ModelBuilder modelBuilder)
    {
        var addresses = new List<Address>();
        int addressId = 1;

        for (int userIndex = 1; userIndex <= 200; userIndex++)
        {
            var userId = UserSeed.GetUserId(userIndex);
            // Each user gets 1-3 addresses based on pattern
            int addressCount = (userIndex % 3) + 1; // 1, 2, or 3

            for (int a = 0; a < addressCount; a++)
            {
                var poolIndex = (addressId - 1) % AddressPool.Length;
                var addrData = AddressPool[poolIndex];

                var addressType = a switch
                {
                    0 => AddressType.Shipping,
                    1 => AddressType.Billing,
                    _ => AddressType.Shipping
                };

                // Deterministic creation dates
                var createdAt = new DateTime(2023, 6, 1, 10, 0, 0, DateTimeKind.Utc)
                    .AddDays((addressId * 2) % 365)
                    .AddHours((addressId * 3) % 24);

                addresses.Add(new Address
                {
                    Id = addressId,
                    AddressLine1 = addrData.Street,
                    AddressLine2 = a == 1 ? "Floor 2, Apt 4B" : null,
                    City = addrData.City,
                    State = addrData.State,
                    PostalCode = addrData.PostalCode,
                    Country = addrData.Country,
                    IsDefault = a == 0, // First address is default
                    Type = addressType,
                    Status = AddressStatus.Active,
                    UserId = userId,
                    CreatedAt = createdAt,
                    UpdatedAt = createdAt,
                    IsDeleted = false
                });

                addressId++;
            }
        }

        modelBuilder.Entity<Address>().HasData(addresses.ToArray());
    }
}
