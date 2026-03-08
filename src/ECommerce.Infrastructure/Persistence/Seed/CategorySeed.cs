using ECommerce.Core.Entities.Category;
using ECommerce.Core.Enums.Category;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class CategorySeed
{
    // Category IDs keyed by a descriptive name for cross-reference
    // Top level: 1-8
    public const int Electronics = 1;
    public const int ClothingFashion = 2;
    public const int BooksMedia = 3;
    public const int HomeGarden = 4;
    public const int SportsOutdoors = 5;
    public const int HealthBeauty = 6;
    public const int ToysGames = 7;
    public const int FoodGrocery = 8;

    // --- Electronics sub-categories ---
    public const int SmartphonesTablets = 9;
    public const int AndroidPhones = 10;
    public const int IPhones = 11;
    public const int Tablets = 12;
    public const int PhoneAccessories = 13;
    public const int CasesCovers = 14;
    public const int ScreenProtectors = 15;
    public const int ChargersCables = 16;
    public const int PowerBanks = 17;
    public const int LaptopsComputers = 18;
    public const int GamingLaptops = 19;
    public const int Ultrabooks = 20;
    public const int DesktopPCs = 21;
    public const int Monitors = 22;
    public const int KeyboardsMice = 23;
    public const int StorageDevices = 24;
    public const int AudioHeadphones = 25;
    public const int WirelessEarbuds = 26;
    public const int OverEarHeadphones = 27;
    public const int Speakers = 28;
    public const int Soundbars = 29;
    public const int TVsHomeTheater = 30;
    public const int SmartTVs = 31;
    public const int Projectors = 32;
    public const int StreamingDevices = 33;
    public const int CamerasPhotography = 34;
    public const int DSLRCameras = 35;
    public const int MirrorlessCameras = 36;
    public const int ActionCameras = 37;
    public const int Drones = 38;
    public const int CameraAccessories = 39;
    public const int Gaming = 40;
    public const int Consoles = 41;
    public const int Games = 42;
    public const int ControllersAccessories = 43;
    public const int GamingChairs = 44;
    public const int GamingHeadsets = 45;
    public const int SmartHome = 46;
    public const int SmartSpeakers = 47;
    public const int SmartLighting = 48;
    public const int SecurityCameras = 49;
    public const int SmartLocks = 50;
    public const int RobotVacuums = 51;
    public const int Wearables = 52;
    public const int Smartwatches = 53;
    public const int FitnessTrackers = 54;
    public const int SmartGlasses = 55;

    // --- Clothing sub-categories ---
    public const int MensClothing = 56;
    public const int MensTShirtsTops = 57;
    public const int MensShirts = 58;
    public const int MensPantsJeans = 59;
    public const int SuitsBlazers = 60;
    public const int MensActivewear = 61;
    public const int WomensClothing = 62;
    public const int Dresses = 63;
    public const int TopsBlouses = 64;
    public const int WomensPantsLeggings = 65;
    public const int AbayasModestWear = 66;
    public const int WomensActivewear = 67;
    public const int KidsClothing = 68;
    public const int ShoesFootwear = 69;
    public const int Sneakers = 70;
    public const int FormalShoes = 71;
    public const int SandalsSlippers = 72;
    public const int BagsAccessories = 73;
    public const int Handbags = 74;
    public const int Backpacks = 75;
    public const int Wallets = 76;
    public const int Sunglasses = 77;
    public const int Watches = 78;

    // --- Books sub-categories ---
    public const int Fiction = 79;
    public const int NonFiction = 80;
    public const int ScienceTechnology = 81;
    public const int BusinessFinance = 82;
    public const int ChildrensBooks = 83;
    public const int ArabicBooks = 84;
    public const int Textbooks = 85;

    // --- Home & Garden sub-categories ---
    public const int Furniture = 86;
    public const int KitchenDining = 87;
    public const int BeddingBath = 88;
    public const int Lighting = 89;
    public const int GardenTools = 90;
    public const int HomeDecor = 91;

    // --- Sports sub-categories ---
    public const int GymFitness = 92;
    public const int FootballSoccer = 93;
    public const int Basketball = 94;
    public const int Tennis = 95;
    public const int Swimming = 96;
    public const int Cycling = 97;
    public const int OutdoorCamping = 98;

    // --- Health & Beauty sub-categories ---
    public const int Skincare = 99;
    public const int Haircare = 100;
    public const int Fragrances = 101;
    public const int VitaminsSupplements = 102;
    public const int MedicalDevices = 103;

    // --- Toys & Games sub-categories ---
    public const int ActionFigures = 104;
    public const int BoardGames = 105;
    public const int EducationalToys = 106;
    public const int LegoBuilding = 107;
    public const int VideoGames = 108;

    private static readonly DateTime CreatedAt = new(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static List<Category> SeedCategories(ModelBuilder modelBuilder)
    {
        var categories = new List<Category>
        {
            // ========================
            // TOP LEVEL (8 categories)
            // ========================
            Cat(Electronics,      "Electronics",           "electronics",             null,                "Devices, gadgets, and cutting-edge technology products"),
            Cat(ClothingFashion,  "Clothing & Fashion",    "clothing-fashion",        null,                "Apparel, shoes, and fashion accessories for all ages"),
            Cat(BooksMedia,       "Books & Media",         "books-media",             null,                "Books, e-books, music, movies, and digital media"),
            Cat(HomeGarden,       "Home & Garden",         "home-garden",             null,                "Furniture, home décor, kitchen essentials, and garden supplies"),
            Cat(SportsOutdoors,   "Sports & Outdoors",     "sports-outdoors",         null,                "Sports equipment, outdoor gear, and fitness accessories"),
            Cat(HealthBeauty,     "Health & Beauty",       "health-beauty",           null,                "Skincare, haircare, supplements, and personal care"),
            Cat(ToysGames,        "Toys & Games",          "toys-games",              null,                "Toys, board games, puzzles, and fun for all ages"),
            Cat(FoodGrocery,      "Food & Grocery",        "food-grocery",            null,                "Fresh food, pantry staples, snacks, and beverages"),

            // ===========================
            // ELECTRONICS HIERARCHY (deep)
            // ===========================
            // Level 2
            Cat(SmartphonesTablets,    "Smartphones & Tablets",    "smartphones-tablets",      Electronics,         "Mobile phones, tablets, and related accessories"),
            Cat(LaptopsComputers,      "Laptops & Computers",      "laptops-computers",        Electronics,         "Laptops, desktops, and computing peripherals"),
            Cat(AudioHeadphones,       "Audio & Headphones",       "audio-headphones",         Electronics,         "Headphones, earbuds, speakers, and audio equipment"),
            Cat(TVsHomeTheater,        "TVs & Home Theater",       "tvs-home-theater",         Electronics,         "Televisions, projectors, and home cinema systems"),
            Cat(CamerasPhotography,    "Cameras & Photography",    "cameras-photography",      Electronics,         "DSLR, mirrorless, action cameras, and drones"),
            Cat(Gaming,                "Gaming",                   "gaming",                   Electronics,         "Consoles, games, controllers, and gaming accessories"),
            Cat(SmartHome,             "Smart Home",               "smart-home",               Electronics,         "Smart speakers, lighting, security, and home automation"),
            Cat(Wearables,             "Wearables",                "wearables",                Electronics,         "Smartwatches, fitness trackers, and smart glasses"),

            // Level 3 — Smartphones & Tablets children
            Cat(AndroidPhones,     "Android Phones",        "android-phones",        SmartphonesTablets,   "Samsung, Xiaomi, OnePlus, and other Android smartphones"),
            Cat(IPhones,           "iPhones",               "iphones",               SmartphonesTablets,   "Apple iPhone models and editions"),
            Cat(Tablets,           "Tablets",               "tablets",               SmartphonesTablets,   "iPad, Galaxy Tab, and other tablets"),
            Cat(PhoneAccessories,  "Phone Accessories",     "phone-accessories",     SmartphonesTablets,   "Cases, chargers, cables, and phone add-ons"),

            // Level 4 — Phone Accessories children
            Cat(CasesCovers,       "Cases & Covers",        "cases-covers",          PhoneAccessories,     "Protective cases and decorative covers"),
            Cat(ScreenProtectors,  "Screen Protectors",     "screen-protectors",     PhoneAccessories,     "Tempered glass and film screen protectors"),
            Cat(ChargersCables,    "Chargers & Cables",     "chargers-cables",       PhoneAccessories,     "USB-C, Lightning, and wireless chargers"),
            Cat(PowerBanks,        "Power Banks",           "power-banks",           PhoneAccessories,     "Portable battery packs and power stations"),

            // Level 3 — Laptops & Computers children
            Cat(GamingLaptops,   "Gaming Laptops",    "gaming-laptops",    LaptopsComputers,  "High-performance laptops for gaming"),
            Cat(Ultrabooks,      "Ultrabooks",        "ultrabooks",        LaptopsComputers,  "Thin and lightweight premium laptops"),
            Cat(DesktopPCs,      "Desktop PCs",       "desktop-pcs",       LaptopsComputers,  "Tower PCs, all-in-ones, and workstations"),
            Cat(Monitors,        "Monitors",          "monitors",          LaptopsComputers,  "LED, IPS, and curved monitors"),
            Cat(KeyboardsMice,   "Keyboards & Mice",  "keyboards-mice",    LaptopsComputers,  "Mechanical keyboards, ergonomic mice, and combos"),
            Cat(StorageDevices,  "Storage Devices",   "storage-devices",   LaptopsComputers,  "SSDs, HDDs, flash drives, and memory cards"),

            // Level 3 — Audio & Headphones children
            Cat(WirelessEarbuds,    "Wireless Earbuds",     "wireless-earbuds",     AudioHeadphones,  "True wireless and Bluetooth earbuds"),
            Cat(OverEarHeadphones,  "Over-ear Headphones",  "over-ear-headphones",  AudioHeadphones,  "Full-size over-ear headphones with ANC"),
            Cat(Speakers,           "Speakers",             "speakers",             AudioHeadphones,  "Portable, desktop, and home speakers"),
            Cat(Soundbars,          "Soundbars",            "soundbars",            AudioHeadphones,  "Soundbars and home audio bars"),

            // Level 3 — TVs & Home Theater children
            Cat(SmartTVs,          "Smart TVs",          "smart-tvs",          TVsHomeTheater,  "4K, 8K, and OLED smart televisions"),
            Cat(Projectors,        "Projectors",         "projectors",         TVsHomeTheater,  "Home theater and portable projectors"),
            Cat(StreamingDevices,  "Streaming Devices",  "streaming-devices",  TVsHomeTheater,  "Fire Stick, Chromecast, Apple TV, and more"),

            // Level 3 — Cameras & Photography children
            Cat(DSLRCameras,        "DSLR Cameras",        "dslr-cameras",        CamerasPhotography,  "Digital SLR cameras for professionals"),
            Cat(MirrorlessCameras,  "Mirrorless Cameras",  "mirrorless-cameras",  CamerasPhotography,  "Compact mirrorless interchangeable lens cameras"),
            Cat(ActionCameras,      "Action Cameras",      "action-cameras",      CamerasPhotography,  "Waterproof and rugged action cameras"),
            Cat(Drones,             "Drones",              "drones",              CamerasPhotography,  "Camera drones and aerial photography"),
            Cat(CameraAccessories,  "Camera Accessories",  "camera-accessories",  CamerasPhotography,  "Lenses, tripods, bags, and filters"),

            // Level 3 — Gaming children
            Cat(Consoles,                "Consoles",                  "consoles",                  Gaming,  "PlayStation, Xbox, and Nintendo consoles"),
            Cat(Games,                   "Games",                     "games",                     Gaming,  "Video game titles for all platforms"),
            Cat(ControllersAccessories,  "Controllers & Accessories", "controllers-accessories",   Gaming,  "Game controllers, joysticks, and accessories"),
            Cat(GamingChairs,            "Gaming Chairs",             "gaming-chairs",             Gaming,  "Ergonomic chairs designed for gamers"),
            Cat(GamingHeadsets,          "Gaming Headsets",           "gaming-headsets",           Gaming,  "Headsets with surround sound for gaming"),

            // Level 3 — Smart Home children
            Cat(SmartSpeakers,    "Smart Speakers",     "smart-speakers",     SmartHome,  "Alexa, Google Home, and smart speakers"),
            Cat(SmartLighting,    "Smart Lighting",     "smart-lighting",     SmartHome,  "Smart bulbs, strips, and lighting systems"),
            Cat(SecurityCameras,  "Security Cameras",   "security-cameras",   SmartHome,  "Indoor and outdoor smart security cameras"),
            Cat(SmartLocks,       "Smart Locks",        "smart-locks",        SmartHome,  "Keyless and smart door locks"),
            Cat(RobotVacuums,     "Robot Vacuums",      "robot-vacuums",      SmartHome,  "Automated robot vacuum cleaners"),

            // Level 3 — Wearables children
            Cat(Smartwatches,     "Smartwatches",      "smartwatches",      Wearables,  "Apple Watch, Galaxy Watch, and smartwatches"),
            Cat(FitnessTrackers,  "Fitness Trackers",  "fitness-trackers",  Wearables,  "Activity and health tracking bands"),
            Cat(SmartGlasses,     "Smart Glasses",     "smart-glasses",     Wearables,  "Augmented reality and smart eyewear"),

            // ===========================
            // CLOTHING & FASHION HIERARCHY
            // ===========================
            Cat(MensClothing,    "Men's Clothing",     "mens-clothing",     ClothingFashion,  "T-shirts, shirts, pants, suits for men"),
            Cat(WomensClothing,  "Women's Clothing",   "womens-clothing",   ClothingFashion,  "Dresses, tops, pants, and modest wear for women"),
            Cat(KidsClothing,    "Kids' Clothing",     "kids-clothing",     ClothingFashion,  "Clothing for children and toddlers"),
            Cat(ShoesFootwear,   "Shoes & Footwear",   "shoes-footwear",    ClothingFashion,  "Sneakers, formal shoes, and sandals"),
            Cat(BagsAccessories, "Bags & Accessories", "bags-accessories",  ClothingFashion,  "Handbags, backpacks, wallets, and accessories"),

            // Men's Clothing children
            Cat(MensTShirtsTops,  "T-Shirts & Tops",   "mens-tshirts-tops",   MensClothing,  "Casual and graphic t-shirts for men"),
            Cat(MensShirts,       "Shirts",            "mens-shirts",          MensClothing,  "Formal, casual, and polo shirts"),
            Cat(MensPantsJeans,   "Pants & Jeans",     "mens-pants-jeans",     MensClothing,  "Denim jeans, chinos, and trousers"),
            Cat(SuitsBlazers,     "Suits & Blazers",   "suits-blazers",        MensClothing,  "Business suits and casual blazers"),
            Cat(MensActivewear,   "Activewear",        "mens-activewear",      MensClothing,  "Gym shorts, joggers, and athletic wear"),

            // Women's Clothing children
            Cat(Dresses,              "Dresses",              "dresses",               WomensClothing,  "Casual, evening, and cocktail dresses"),
            Cat(TopsBlouses,          "Tops & Blouses",       "tops-blouses",          WomensClothing,  "Blouses, crop tops, and elegant tops"),
            Cat(WomensPantsLeggings,  "Pants & Leggings",     "womens-pants-leggings", WomensClothing,  "Skinny pants, wide-leg, and leggings"),
            Cat(AbayasModestWear,     "Abayas & Modest Wear", "abayas-modest-wear",    WomensClothing,  "Traditional abayas and modest fashion"),
            Cat(WomensActivewear,     "Activewear",           "womens-activewear",     WomensClothing,  "Sports bras, yoga pants, and workout gear"),

            // Shoes children
            Cat(Sneakers,         "Sneakers",            "sneakers",            ShoesFootwear,  "Sport and lifestyle sneakers"),
            Cat(FormalShoes,      "Formal Shoes",        "formal-shoes",        ShoesFootwear,  "Oxford, loafers, and dress shoes"),
            Cat(SandalsSlippers,  "Sandals & Slippers",  "sandals-slippers",    ShoesFootwear,  "Casual sandals and comfortable slippers"),

            // Bags children
            Cat(Handbags,    "Handbags",    "handbags",    BagsAccessories,  "Designer and everyday handbags"),
            Cat(Backpacks,   "Backpacks",   "backpacks",   BagsAccessories,  "School, travel, and laptop backpacks"),
            Cat(Wallets,     "Wallets",     "wallets",     BagsAccessories,  "Leather, fabric, and cardholder wallets"),
            Cat(Sunglasses,  "Sunglasses",  "sunglasses",  BagsAccessories,  "UV-protection and fashion sunglasses"),
            Cat(Watches,     "Watches",     "watches",     BagsAccessories,  "Analog, digital, and luxury watches"),

            // ===========================
            // BOOKS & MEDIA HIERARCHY
            // ===========================
            Cat(Fiction,            "Fiction",               "fiction",               BooksMedia,  "Novels, short stories, and literary fiction"),
            Cat(NonFiction,         "Non-Fiction",           "non-fiction",           BooksMedia,  "Biographies, essays, and factual books"),
            Cat(ScienceTechnology,  "Science & Technology",  "science-technology",    BooksMedia,  "STEM books and technology guides"),
            Cat(BusinessFinance,    "Business & Finance",    "business-finance",      BooksMedia,  "Entrepreneurship, investing, and management"),
            Cat(ChildrensBooks,     "Children's Books",      "childrens-books",       BooksMedia,  "Picture books and young reader stories"),
            Cat(ArabicBooks,        "Arabic Books",          "arabic-books",          BooksMedia,  "كتب عربية — Arabic literature and non-fiction"),
            Cat(Textbooks,          "Textbooks",             "textbooks",             BooksMedia,  "Academic and educational textbooks"),

            // ===========================
            // HOME & GARDEN HIERARCHY
            // ===========================
            Cat(Furniture,     "Furniture",        "furniture",        HomeGarden,  "Tables, chairs, sofas, and storage"),
            Cat(KitchenDining, "Kitchen & Dining", "kitchen-dining",   HomeGarden,  "Cookware, utensils, and dining sets"),
            Cat(BeddingBath,   "Bedding & Bath",   "bedding-bath",     HomeGarden,  "Sheets, towels, and bathroom accessories"),
            Cat(Lighting,      "Lighting",         "lighting",         HomeGarden,  "Lamps, ceiling lights, and LED fixtures"),
            Cat(GardenTools,   "Garden Tools",     "garden-tools",     HomeGarden,  "Shovels, lawn mowers, and irrigation"),
            Cat(HomeDecor,     "Home Decor",       "home-decor",       HomeGarden,  "Wall art, rugs, candles, and decor items"),

            // ===========================
            // SPORTS & OUTDOORS HIERARCHY
            // ===========================
            Cat(GymFitness,     "Gym & Fitness",      "gym-fitness",      SportsOutdoors,  "Weights, mats, resistance bands, and gym gear"),
            Cat(FootballSoccer, "Football / Soccer",  "football-soccer",  SportsOutdoors,  "Footballs, boots, and training equipment"),
            Cat(Basketball,     "Basketball",         "basketball",       SportsOutdoors,  "Basketballs, hoops, and gear"),
            Cat(Tennis,         "Tennis",             "tennis",           SportsOutdoors,  "Rackets, balls, and tennis equipment"),
            Cat(Swimming,       "Swimming",           "swimming",         SportsOutdoors,  "Goggles, swimsuits, and pool accessories"),
            Cat(Cycling,        "Cycling",            "cycling",          SportsOutdoors,  "Bicycles, helmets, and cycling accessories"),
            Cat(OutdoorCamping, "Outdoor & Camping",  "outdoor-camping",  SportsOutdoors,  "Tents, sleeping bags, and camping gear"),

            // ===========================
            // HEALTH & BEAUTY HIERARCHY
            // ===========================
            Cat(Skincare,             "Skincare",                "skincare",               HealthBeauty,  "Moisturizers, serums, sunscreen, and cleansers"),
            Cat(Haircare,             "Haircare",                "haircare",               HealthBeauty,  "Shampoo, conditioner, styling, and hair tools"),
            Cat(Fragrances,           "Fragrances",              "fragrances",             HealthBeauty,  "Perfumes, colognes, and body sprays"),
            Cat(VitaminsSupplements,  "Vitamins & Supplements",  "vitamins-supplements",   HealthBeauty,  "Multivitamins, omega-3, and dietary supplements"),
            Cat(MedicalDevices,       "Medical Devices",         "medical-devices",        HealthBeauty,  "Blood pressure monitors, thermometers, and medical tools"),

            // ===========================
            // TOYS & GAMES HIERARCHY
            // ===========================
            Cat(ActionFigures,   "Action Figures",    "action-figures",    ToysGames,  "Superhero, anime, and collectible figures"),
            Cat(BoardGames,      "Board Games",       "board-games",       ToysGames,  "Strategy, party, and family board games"),
            Cat(EducationalToys, "Educational Toys",  "educational-toys",  ToysGames,  "STEM kits, puzzles, and learning toys"),
            Cat(LegoBuilding,    "LEGO & Building",   "lego-building",     ToysGames,  "LEGO sets, building blocks, and construction toys"),
            Cat(VideoGames,      "Video Games",       "video-games",       ToysGames,  "Console, PC, and mobile video games"),
        };

        modelBuilder.Entity<Category>().HasData(categories.ToArray());

        // Seed category images for top-level categories
        var categoryImages = new List<CategoryImage>();
        for (int i = 1; i <= 8; i++)
        {
            categoryImages.Add(new CategoryImage
            {
                Id = i,
                CategoryId = i,
                ImageUrl = $"/images/categories/{categories[i - 1].Slug}-banner.jpg",
                AltText = $"{categories[i - 1].Name} Banner",
                IsMain = true,
                SortOrder = 0,
                SubType = ECommerce.Core.Enums.Media.ImageSubType.CategoryBanner,
                UploadedAt = CreatedAt,
                CreatedAt = CreatedAt,
                UpdatedAt = CreatedAt,
                IsDeleted = false
            });
        }

        modelBuilder.Entity<CategoryImage>().HasData(categoryImages.ToArray());

        return categories;
    }

    private static Category Cat(int id, string name, string slug, int? parentId, string description)
    {
        return new Category
        {
            Id = id,
            Name = name,
            Slug = slug,
            ParentCategoryId = parentId,
            Description = description,
            Status = CategoryStatus.Active,
            CreatedAt = CreatedAt,
            UpdatedAt = CreatedAt,
            IsDeleted = false
        };
    }
}
