using ECommerce.Core.Entities.Product;
using ECommerce.Core.Enums.Product;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class ProductOptionSeed
{
    private static readonly DateTime CreatedAt = new(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc);

    // Option definition: (OptionId, ProductId, Name, DisplayType, AttributeKey, Values[])
    // Value definition: (ValueId, Value, Label, PriceAdjust)
    public static void SeedProductOptions(ModelBuilder modelBuilder)
    {
        var options = new List<ProductOption>();
        var optionValues = new List<ProductOptionValue>();

        int optId = 1;
        int valId = 1;

        // Helper to add an option with values
        void AddOption(int productId, string name, OptionDisplayType display, string attrKey,
            params (string val, string label, decimal priceAdj)[] values)
        {
            var currentOptId = optId++;
            options.Add(new ProductOption
            {
                Id = currentOptId,
                ProductId = productId,
                Name = name,
                DisplayType = display,
                Type = OptionType.VariantSelector,
                AttributeKey = attrKey,
                IsRequired = true,
                PriceValue = 0,
                SortOrder = options.Count(o => o.ProductId == productId),
                CreatedAt = CreatedAt,
                UpdatedAt = CreatedAt,
                IsDeleted = false
            });

            foreach (var (val, label, priceAdj) in values)
            {
                optionValues.Add(new ProductOptionValue
                {
                    Id = valId++,
                    OptionId = currentOptId,
                    Value = val,
                    Label = label,
                    PriceValue = priceAdj,
                    IsDefault = optionValues.All(v => v.OptionId != currentOptId),
                    SortOrder = optionValues.Count(v => v.OptionId == currentOptId),
                    ImageUrl = string.Empty,
                    CreatedAt = CreatedAt,
                    UpdatedAt = CreatedAt,
                    IsDeleted = false
                });
            }
        }

        // ==========================================
        // SMARTPHONES — Storage + Color options
        // ==========================================

        // 1: iPhone 15 Pro Max
        AddOption(1, "Storage", OptionDisplayType.Dropdown, "storage",
            ("256GB", "256GB", 0), ("512GB", "512GB", 200), ("1TB", "1TB", 400));
        AddOption(1, "Color", OptionDisplayType.ColorSwatch, "color",
            ("natural-titanium", "Natural Titanium", 0), ("black-titanium", "Black Titanium", 0),
            ("blue-titanium", "Blue Titanium", 0), ("white-titanium", "White Titanium", 0));

        // 2: iPhone 15
        AddOption(2, "Storage", OptionDisplayType.Dropdown, "storage",
            ("128GB", "128GB", 0), ("256GB", "256GB", 100), ("512GB", "512GB", 300));
        AddOption(2, "Color", OptionDisplayType.ColorSwatch, "color",
            ("black", "Black", 0), ("blue", "Blue", 0), ("green", "Green", 0), ("yellow", "Yellow", 0), ("pink", "Pink", 0));

        // 3: Samsung Galaxy S24 Ultra
        AddOption(3, "Storage", OptionDisplayType.Dropdown, "storage",
            ("256GB", "256GB", 0), ("512GB", "512GB", 120), ("1TB", "1TB", 240));
        AddOption(3, "Color", OptionDisplayType.ColorSwatch, "color",
            ("titanium-gray", "Titanium Gray", 0), ("titanium-black", "Titanium Black", 0),
            ("titanium-violet", "Titanium Violet", 0), ("titanium-yellow", "Titanium Yellow", 0));

        // 4: Samsung Galaxy S24
        AddOption(4, "Storage", OptionDisplayType.Dropdown, "storage",
            ("128GB", "128GB", 0), ("256GB", "256GB", 60));
        AddOption(4, "Color", OptionDisplayType.ColorSwatch, "color",
            ("onyx-black", "Onyx Black", 0), ("marble-gray", "Marble Gray", 0), ("cobalt-violet", "Cobalt Violet", 0));

        // 5-6: Galaxy A55/A35 — Color only
        AddOption(5, "Color", OptionDisplayType.ColorSwatch, "color",
            ("awesome-iceblue", "Awesome Iceblue", 0), ("awesome-lilac", "Awesome Lilac", 0), ("awesome-navy", "Awesome Navy", 0));
        AddOption(6, "Color", OptionDisplayType.ColorSwatch, "color",
            ("awesome-iceblue", "Awesome Iceblue", 0), ("awesome-lilac", "Awesome Lilac", 0), ("awesome-lemon", "Awesome Lemon", 0));

        // 7: Xiaomi 14 Pro
        AddOption(7, "Storage", OptionDisplayType.Dropdown, "storage",
            ("256GB", "256GB", 0), ("512GB", "512GB", 100));
        AddOption(7, "Color", OptionDisplayType.ColorSwatch, "color",
            ("black", "Black", 0), ("white", "White", 0), ("green", "Jade Green", 0));

        // 8: Redmi Note 13 Pro
        AddOption(8, "Storage", OptionDisplayType.Dropdown, "storage",
            ("128GB", "128GB", 0), ("256GB", "256GB", 30));
        AddOption(8, "Color", OptionDisplayType.ColorSwatch, "color",
            ("midnight-black", "Midnight Black", 0), ("ocean-teal", "Ocean Teal", 0), ("lavender-purple", "Lavender Purple", 0));

        // 9-20: Remaining smartphones — Color option only
        for (int pid = 9; pid <= 20; pid++)
        {
            AddOption(pid, "Color", OptionDisplayType.ColorSwatch, "color",
                ("black", "Black", 0), ("blue", "Blue", 0), ("white", "White", 0));
        }

        // ==========================================
        // LAPTOPS — RAM + Storage options for select models
        // ==========================================

        // 21: MacBook Pro 14 M3 Pro
        AddOption(21, "RAM", OptionDisplayType.Dropdown, "ram",
            ("18GB", "18GB Unified Memory", 0), ("36GB", "36GB Unified Memory", 200));
        AddOption(21, "Storage", OptionDisplayType.Dropdown, "storage",
            ("512GB", "512GB SSD", 0), ("1TB", "1TB SSD", 200));
        AddOption(21, "Color", OptionDisplayType.ColorSwatch, "color",
            ("space-black", "Space Black", 0), ("silver", "Silver", 0));

        // 22: MacBook Air 15 M3
        AddOption(22, "Storage", OptionDisplayType.Dropdown, "storage",
            ("256GB", "256GB SSD", 0), ("512GB", "512GB SSD", 200));
        AddOption(22, "Color", OptionDisplayType.ColorSwatch, "color",
            ("midnight", "Midnight", 0), ("starlight", "Starlight", 0), ("space-gray", "Space Gray", 0));

        // 23: Dell XPS 15
        AddOption(23, "RAM", OptionDisplayType.Dropdown, "ram",
            ("16GB", "16GB DDR5", 0), ("32GB", "32GB DDR5", 200));
        AddOption(23, "Storage", OptionDisplayType.Dropdown, "storage",
            ("512GB", "512GB SSD", 0), ("1TB", "1TB SSD", 100));

        // 24-28: Other laptops — color only
        for (int pid = 24; pid <= 28; pid++)
        {
            AddOption(pid, "Color", OptionDisplayType.ColorSwatch, "color",
                ("silver", "Silver", 0), ("black", "Black", 0));
        }

        // 29: Asus ROG Strix G16
        AddOption(29, "RAM", OptionDisplayType.Dropdown, "ram",
            ("16GB", "16GB DDR5", 0), ("32GB", "32GB DDR5", 150));
        AddOption(29, "Storage", OptionDisplayType.Dropdown, "storage",
            ("512GB", "512GB SSD", 0), ("1TB", "1TB SSD", 100));

        // 30-35: Remaining laptops — color only
        for (int pid = 30; pid <= 35; pid++)
        {
            AddOption(pid, "Color", OptionDisplayType.ColorSwatch, "color",
                ("silver", "Silver", 0), ("dark-gray", "Dark Gray", 0));
        }

        // ==========================================
        // TABLETS — Storage + Connectivity
        // ==========================================

        // 36: iPad Pro 12.9 M4
        AddOption(36, "Storage", OptionDisplayType.Dropdown, "storage",
            ("256GB", "256GB", 0), ("512GB", "512GB", 200), ("1TB", "1TB", 400));
        AddOption(36, "Connectivity", OptionDisplayType.RadioButtons, "connectivity",
            ("wifi", "Wi-Fi", 0), ("wifi-cellular", "Wi-Fi + Cellular", 200));

        // 37: iPad Air 11 M2
        AddOption(37, "Storage", OptionDisplayType.Dropdown, "storage",
            ("128GB", "128GB", 0), ("256GB", "256GB", 100));
        AddOption(37, "Color", OptionDisplayType.ColorSwatch, "color",
            ("space-gray", "Space Gray", 0), ("starlight", "Starlight", 0), ("purple", "Purple", 0), ("blue", "Blue", 0));

        // 38-43: Other tablets — simple color
        for (int pid = 38; pid <= 43; pid++)
        {
            AddOption(pid, "Color", OptionDisplayType.ColorSwatch, "color",
                ("space-gray", "Space Gray", 0), ("silver", "Silver", 0));
        }

        // ==========================================
        // HEADPHONES — Color only
        // ==========================================
        for (int pid = 44; pid <= 53; pid++)
        {
            AddOption(pid, "Color", OptionDisplayType.ColorSwatch, "color",
                ("black", "Black", 0), ("white", "White", 0));
        }

        // ==========================================
        // TVs — Size options for Samsung QLED
        // ==========================================
        // 54: Samsung QLED Q80C — size variants
        AddOption(54, "Size", OptionDisplayType.Dropdown, "size",
            ("55-inch", "55 inch", -200), ("65-inch", "65 inch", 0), ("75-inch", "75 inch", 400));

        // 55-61: Other TVs — no options (single variant)
        // We still add them with a default "Size" option for consistency
        for (int pid = 55; pid <= 61; pid++)
        {
            var defaultSize = pid switch
            {
                55 => "55-inch",
                56 => "65-inch",
                57 => "43-inch",
                58 => "55-inch",
                59 => "50-inch",
                60 => "55-inch",
                61 => "65-inch",
                _ => "55-inch"
            };
            var defaultLabel = pid switch
            {
                55 => "55 inch",
                56 => "65 inch",
                57 => "43 inch",
                58 => "55 inch",
                59 => "50 inch",
                60 => "55 inch",
                61 => "65 inch",
                _ => "55 inch"
            };
            AddOption(pid, "Size", OptionDisplayType.Dropdown, "size",
                (defaultSize, defaultLabel, 0));
        }

        // ==========================================
        // CAMERAS — Color only where applicable
        // ==========================================
        for (int pid = 62; pid <= 69; pid++)
        {
            AddOption(pid, "Color", OptionDisplayType.ColorSwatch, "color",
                ("black", "Black", 0));
        }

        // ==========================================
        // GAMING — Edition/Color
        // ==========================================

        // 70: PS5 — Edition
        AddOption(70, "Edition", OptionDisplayType.RadioButtons, "edition",
            ("disc", "Disc Edition", 0), ("digital", "Digital Edition", -100));

        // 71-73: Consoles — color only (single)
        for (int pid = 71; pid <= 73; pid++)
        {
            AddOption(pid, "Color", OptionDisplayType.ColorSwatch, "color",
                ("black", "Black", 0), ("white", "White", 0));
        }

        // 74: DualSense — Color variants
        AddOption(74, "Color", OptionDisplayType.ColorSwatch, "color",
            ("white", "White", 0), ("midnight-black", "Midnight Black", 0),
            ("cosmic-red", "Cosmic Red", 0), ("starlight-blue", "Starlight Blue", 0));

        // 75: Xbox Controller — Color variants
        AddOption(75, "Color", OptionDisplayType.ColorSwatch, "color",
            ("carbon-black", "Carbon Black", 0), ("robot-white", "Robot White", 0),
            ("shock-blue", "Shock Blue", 0), ("pulse-red", "Pulse Red", 0));

        // 76: Razer Kraken — Color
        AddOption(76, "Color", OptionDisplayType.ColorSwatch, "color",
            ("black", "Black", 0), ("green", "Green", 0));

        // ==========================================
        // ACCESSORIES — Color only
        // ==========================================
        AddOption(77, "Color", OptionDisplayType.ColorSwatch, "color",
            ("black", "Black", 0), ("white", "White", 0));
        AddOption(78, "Color", OptionDisplayType.ColorSwatch, "color",
            ("black", "Black", 0));
        AddOption(79, "Color", OptionDisplayType.ColorSwatch, "color",
            ("graphite", "Graphite", 0), ("pale-gray", "Pale Gray", 0));
        AddOption(80, "Color", OptionDisplayType.ColorSwatch, "color",
            ("white", "White", 0));

        modelBuilder.Entity<ProductOption>().HasData(options.ToArray());
        modelBuilder.Entity<ProductOptionValue>().HasData(optionValues.ToArray());
    }
}
