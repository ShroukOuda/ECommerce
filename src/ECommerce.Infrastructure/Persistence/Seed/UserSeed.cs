using ECommerce.Core.Enums.User;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class UserSeed
{
    // User ID pattern: 00000001-0000-0000-0000-{i:D12}
    public static string GetUserId(int i) => $"00000001-0000-0000-0000-{i:D12}";

    private static readonly string[] FirstNames =
    {
        "Ahmed", "Fatima", "Mohamed", "Aisha", "Omar", "Nour", "Youssef", "Mariam", "Ali", "Sara",
        "Hassan", "Layla", "Khaled", "Hana", "Tarek", "Dina", "Amr", "Rania", "Mostafa", "Yasmin",
        "Ibrahim", "Salma", "Mahmoud", "Nada", "Karim", "Mona", "Adel", "Rana", "Emad", "Noha",
        "James", "Emma", "William", "Olivia", "Benjamin", "Sophia", "Lucas", "Ava", "Henry", "Isabella",
        "Liam", "Mia", "Noah", "Charlotte", "Ethan", "Amelia", "Mason", "Harper", "Logan", "Evelyn",
        "Pierre", "Marie", "Jean", "Claire", "Louis", "Camille", "Antoine", "Léa", "Hugo", "Manon",
        "Raj", "Priya", "Arjun", "Ananya", "Vikram", "Deepa", "Sanjay", "Kavita", "Ravi", "Sunita",
        "Wei", "Mei", "Jun", "Lin", "Hao", "Xia", "Chen", "Yan", "Ming", "Hua",
        "Carlos", "Maria", "Diego", "Ana", "Jorge", "Sofia", "Ricardo", "Carmen", "Fernando", "Lucia",
        "Yuki", "Sakura", "Kenji", "Haruka", "Takeshi", "Yui", "Hiroshi", "Aoi", "Daisuke", "Misaki",
        "Abdallah", "Zahra", "Hamza", "Amina", "Bilal", "Khadija", "Othman", "Hafsa", "Ziad", "Sumaya",
        "David", "Sarah", "Michael", "Jennifer", "Robert", "Jessica", "Daniel", "Emily", "Matthew", "Ashley",
        "Alexander", "Natasha", "Dmitri", "Elena", "Ivan", "Olga", "Sergei", "Tatiana", "Andrei", "Valentina",
        "Stefan", "Julia", "Markus", "Anna", "Thomas", "Katharina", "Felix", "Lena", "Maximilian", "Sophie",
        "Marco", "Giulia", "Luca", "Francesca", "Giovanni", "Chiara", "Andrea", "Valentina", "Matteo", "Aurora",
        "Abdul", "Fatimah", "Rashid", "Amal", "Saeed", "Huda", "Faisal", "Lina", "Nasser", "Noora",
        "Patrick", "Grace", "Brian", "Chloe", "Kevin", "Hannah", "Sean", "Lily", "Ryan", "Zoe",
        "Oscar", "Elsa", "Erik", "Astrid", "Lars", "Freya", "Nils", "Ingrid", "Sven", "Linnea",
        "Mustafa", "Elif", "Baris", "Defne", "Cem", "Zeynep", "Alp", "Selin", "Onur", "Ebru",
        "John", "Elizabeth", "Richard", "Catherine", "George", "Victoria", "Edward", "Margaret", "Philip", "Diana"
    };

    private static readonly string[] LastNames =
    {
        "Al-Masri", "Hassan", "El-Sayed", "Ibrahim", "Mostafa", "Khalil", "Nasser", "Farouk", "Gamal", "Salem",
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
        "Dupont", "Martin", "Bernard", "Dubois", "Moreau", "Laurent", "Simon", "Michel", "Leroy", "Roux",
        "Patel", "Sharma", "Gupta", "Singh", "Kumar", "Khan", "Agarwal", "Reddy", "Joshi", "Mehta",
        "Wang", "Li", "Zhang", "Liu", "Chen", "Yang", "Huang", "Wu", "Zhou", "Xu",
        "Gonzalez", "Lopez", "Hernandez", "Torres", "Rivera", "Ramirez", "Flores", "Gomez", "Diaz", "Reyes",
        "Tanaka", "Yamamoto", "Sato", "Suzuki", "Watanabe", "Takahashi", "Ito", "Nakamura", "Kobayashi", "Kato",
        "Al-Ahmad", "Al-Ali", "Al-Rashid", "Al-Saeed", "Al-Fayed", "Al-Qasim", "Al-Hamad", "Al-Jaber", "Al-Naimi", "Al-Thani",
        "Anderson", "Thomas", "Jackson", "White", "Harris", "Thompson", "Moore", "Young", "Allen", "King",
        "Petrov", "Ivanov", "Volkov", "Sokolov", "Popov", "Kozlov", "Lebedev", "Smirnov", "Federov", "Morozov",
        "Müller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer", "Wagner", "Becker", "Hoffmann", "Schäfer",
        "Rossi", "Russo", "Ferrari", "Esposito", "Bianchi", "Romano", "Colombo", "Ricci", "Marino", "Greco",
        "Al-Harbi", "Al-Ghamdi", "Al-Malki", "Al-Otaibi", "Al-Zahrani", "Al-Dosari", "Al-Shehri", "Al-Shahrani", "Al-Qahtani", "Al-Mutairi",
        "Yilmaz", "Kaya", "Demir", "Celik", "Sahin", "Ozturk", "Aydin", "Arslan", "Dogan", "Kilic",
        "Park", "Kim", "Lee", "Choi", "Jung", "Kang", "Cho", "Yoon", "Jang", "Lim",
        "Murphy", "Kelly", "O'Brien", "Walsh", "O'Sullivan", "Byrne", "Ryan", "O'Connor", "Kennedy", "Lynch",
        "Johansson", "Andersson", "Karlsson", "Nilsson", "Eriksson", "Larsson", "Olsson", "Persson", "Svensson", "Gustafsson",
        "Silva", "Santos", "Oliveira", "Souza", "Pereira", "Costa", "Ferreira", "Rodrigues", "Almeida", "Nascimento",
        "Adams", "Nelson", "Hill", "Campbell", "Mitchell", "Roberts", "Carter", "Phillips", "Evans", "Turner",
        "Bakr", "Mansour", "Youssef", "Hamdi", "Fathi", "Taha", "Zaki", "Hamed", "Saber", "Othman"
    };

    private static readonly string[] EmailDomains = { "gmail.com", "yahoo.com", "hotmail.com", "outlook.com" };

    private static readonly DateTime BaseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void SeedUsers(ModelBuilder modelBuilder)
    {
        var hasher = new PasswordHasher<ECommerce.Core.Entities.User.User>();
        var users = new List<ECommerce.Core.Entities.User.User>();
        var userRoles = new List<IdentityUserRole<string>>();

        for (int i = 1; i <= 200; i++)
        {
            var userId = GetUserId(i);
            var firstName = FirstNames[(i - 1) % FirstNames.Length];
            var lastName = LastNames[(i - 1) % LastNames.Length];
            var domain = EmailDomains[(i - 1) % EmailDomains.Length];
            var email = $"{firstName.ToLower()}.{lastName.ToLower().Replace("'", "").Replace(" ", "")}_{i}@{domain}";
            var normalizedEmail = email.ToUpper();
            var userName = email;

            // Determine role
            string roleId;
            if (i <= 5)
                roleId = RoleSeed.AdminRoleId;
            else if (i <= 15)
                roleId = RoleSeed.StaffRoleId;
            else
                roleId = RoleSeed.CustomerRoleId;

            // Determine status
            UserStatus status;
            if (i >= 186 && i <= 195) status = UserStatus.Inactive;
            else if (i >= 196 && i <= 200) status = UserStatus.Banned;
            else status = UserStatus.Active;

            // Determine gender
            Gender? gender = (i % 2 == 0) ? Gender.Female : Gender.Male;

            // Spread CreatedAt over 2 years
            var dayOffset = (i * 3) % 730; // ~2 years of days
            var hourOffset = (i * 7) % 24;
            var createdAt = BaseDate.AddDays(-730 + dayOffset).AddHours(hourOffset);

            // Country codes varied
            string[] countryCodes = { "EG", "AE", "SA", "GB", "US", "FR", "IN", "CN", "ES", "JP", "TR", "DE", "IT", "SE", "BR" };
            var countryCode = countryCodes[(i - 1) % countryCodes.Length];

            // Phone numbers
            string[] phonePatterns = {
                "+20-10{0:D7}", "+20-11{0:D7}", "+20-12{0:D7}",
                "+971-50{0:D7}", "+966-55{0:D7}", "+44-77{0:D7}",
                "+1-555{0:D7}", "+33-6{0:D8}", "+91-98{0:D8}",
                "+86-138{0:D7}", "+34-6{0:D8}", "+81-90{0:D7}",
                "+49-170{0:D7}", "+90-532{0:D7}", "+55-11{0:D7}"
            };
            var phonePattern = phonePatterns[(i - 1) % phonePatterns.Length];
            var phone = string.Format(phonePattern, 1000000 + i);

            var user = new ECommerce.Core.Entities.User.User
            {
                Id = userId,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                NormalizedEmail = normalizedEmail,
                UserName = userName,
                NormalizedUserName = normalizedEmail,
                EmailConfirmed = true,
                PhoneNumber = phone,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                SecurityStamp = $"SECURITY-{i:D12}",
                ConcurrencyStamp = $"00000001-0000-0000-0001-{i:D12}",
                DateOfBirth = new DateTime(1970 + (i % 35), ((i % 12) + 1), ((i % 28) + 1), 0, 0, 0, DateTimeKind.Utc),
                CountryCode = countryCode,
                Status = status,
                Gender = gender,
                CreatedAt = createdAt,
                UpdatedAt = createdAt.AddDays(i % 30)
            };

            // Hash password
            user.PasswordHash = hasher.HashPassword(user, "Password@123");
            users.Add(user);

            userRoles.Add(new IdentityUserRole<string>
            {
                UserId = userId,
                RoleId = roleId
            });
        }

        modelBuilder.Entity<ECommerce.Core.Entities.User.User>().HasData(users.ToArray());
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(userRoles.ToArray());
    }
}
