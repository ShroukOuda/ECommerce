using ECommerce.Core.Entities.Product;
using ECommerce.Core.Enums.Inventory;
using ECommerce.Core.Enums.Product;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class ProductVariantSeed
{
    private static readonly DateTime CreatedAt = new(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Seeds product variants and the junction table ProductVariantOptionValue.
    /// Because EF Core HasData requires static data, we build the full cartesian product
    /// of option values per product to create all variant combinations.
    ///
    /// We use the ProductOptionSeed internally to derive the same option/value IDs.
    /// </summary>
    public static void SeedProductVariants(ModelBuilder modelBuilder)
    {
        // We need to rebuild the option/value structure to compute cartesian products.
        // We'll replicate the same ID assignment logic from ProductOptionSeed.

        var options = new List<(int OptId, int ProductId, string AttrKey)>();
        var optionValues = new List<(int ValId, int OptId, int ProductId, string Value, string Label, decimal PriceAdj)>();

        int optId = 1;
        int valId = 1;

        void AddOpt(int productId, string attrKey, params (string val, string label, decimal priceAdj)[] values)
        {
            int currentOptId = optId++;
            options.Add((currentOptId, productId, attrKey));
            foreach (var (v, l, p) in values)
            {
                optionValues.Add((valId++, currentOptId, productId, v, l, p));
            }
        }

        // ---- Replicate exact same option definitions from ProductOptionSeed ----

        // Product 1
        AddOpt(1, "storage", ("256GB","256GB",0), ("512GB","512GB",200), ("1TB","1TB",400));
        AddOpt(1, "color", ("natural-titanium","Natural Titanium",0), ("black-titanium","Black Titanium",0), ("blue-titanium","Blue Titanium",0), ("white-titanium","White Titanium",0));
        // Product 2
        AddOpt(2, "storage", ("128GB","128GB",0), ("256GB","256GB",100), ("512GB","512GB",300));
        AddOpt(2, "color", ("black","Black",0), ("blue","Blue",0), ("green","Green",0), ("yellow","Yellow",0), ("pink","Pink",0));
        // Product 3
        AddOpt(3, "storage", ("256GB","256GB",0), ("512GB","512GB",120), ("1TB","1TB",240));
        AddOpt(3, "color", ("titanium-gray","Titanium Gray",0), ("titanium-black","Titanium Black",0), ("titanium-violet","Titanium Violet",0), ("titanium-yellow","Titanium Yellow",0));
        // Product 4
        AddOpt(4, "storage", ("128GB","128GB",0), ("256GB","256GB",60));
        AddOpt(4, "color", ("onyx-black","Onyx Black",0), ("marble-gray","Marble Gray",0), ("cobalt-violet","Cobalt Violet",0));
        // Product 5
        AddOpt(5, "color", ("awesome-iceblue","Awesome Iceblue",0), ("awesome-lilac","Awesome Lilac",0), ("awesome-navy","Awesome Navy",0));
        // Product 6
        AddOpt(6, "color", ("awesome-iceblue","Awesome Iceblue",0), ("awesome-lilac","Awesome Lilac",0), ("awesome-lemon","Awesome Lemon",0));
        // Product 7
        AddOpt(7, "storage", ("256GB","256GB",0), ("512GB","512GB",100));
        AddOpt(7, "color", ("black","Black",0), ("white","White",0), ("green","Jade Green",0));
        // Product 8
        AddOpt(8, "storage", ("128GB","128GB",0), ("256GB","256GB",30));
        AddOpt(8, "color", ("midnight-black","Midnight Black",0), ("ocean-teal","Ocean Teal",0), ("lavender-purple","Lavender Purple",0));
        // Products 9-20: color only
        for (int pid = 9; pid <= 20; pid++)
            AddOpt(pid, "color", ("black","Black",0), ("blue","Blue",0), ("white","White",0));
        // Product 21
        AddOpt(21, "ram", ("18GB","18GB Unified Memory",0), ("36GB","36GB Unified Memory",200));
        AddOpt(21, "storage", ("512GB","512GB SSD",0), ("1TB","1TB SSD",200));
        AddOpt(21, "color", ("space-black","Space Black",0), ("silver","Silver",0));
        // Product 22
        AddOpt(22, "storage", ("256GB","256GB SSD",0), ("512GB","512GB SSD",200));
        AddOpt(22, "color", ("midnight","Midnight",0), ("starlight","Starlight",0), ("space-gray","Space Gray",0));
        // Product 23
        AddOpt(23, "ram", ("16GB","16GB DDR5",0), ("32GB","32GB DDR5",200));
        AddOpt(23, "storage", ("512GB","512GB SSD",0), ("1TB","1TB SSD",100));
        // Products 24-28: color only
        for (int pid = 24; pid <= 28; pid++)
            AddOpt(pid, "color", ("silver","Silver",0), ("black","Black",0));
        // Product 29
        AddOpt(29, "ram", ("16GB","16GB DDR5",0), ("32GB","32GB DDR5",150));
        AddOpt(29, "storage", ("512GB","512GB SSD",0), ("1TB","1TB SSD",100));
        // Products 30-35: color only
        for (int pid = 30; pid <= 35; pid++)
            AddOpt(pid, "color", ("silver","Silver",0), ("dark-gray","Dark Gray",0));
        // Product 36
        AddOpt(36, "storage", ("256GB","256GB",0), ("512GB","512GB",200), ("1TB","1TB",400));
        AddOpt(36, "connectivity", ("wifi","Wi-Fi",0), ("wifi-cellular","Wi-Fi + Cellular",200));
        // Product 37
        AddOpt(37, "storage", ("128GB","128GB",0), ("256GB","256GB",100));
        AddOpt(37, "color", ("space-gray","Space Gray",0), ("starlight","Starlight",0), ("purple","Purple",0), ("blue","Blue",0));
        // Products 38-43: color only
        for (int pid = 38; pid <= 43; pid++)
            AddOpt(pid, "color", ("space-gray","Space Gray",0), ("silver","Silver",0));
        // Products 44-53: color only
        for (int pid = 44; pid <= 53; pid++)
            AddOpt(pid, "color", ("black","Black",0), ("white","White",0));
        // Product 54: TV size
        AddOpt(54, "size", ("55-inch","55 inch",-200), ("65-inch","65 inch",0), ("75-inch","75 inch",400));
        // Products 55-61: single size
        AddOpt(55, "size", ("55-inch","55 inch",0));
        AddOpt(56, "size", ("65-inch","65 inch",0));
        AddOpt(57, "size", ("43-inch","43 inch",0));
        AddOpt(58, "size", ("55-inch","55 inch",0));
        AddOpt(59, "size", ("50-inch","50 inch",0));
        AddOpt(60, "size", ("55-inch","55 inch",0));
        AddOpt(61, "size", ("65-inch","65 inch",0));
        // Products 62-69: cameras color
        for (int pid = 62; pid <= 69; pid++)
            AddOpt(pid, "color", ("black","Black",0));
        // Product 70: PS5 edition
        AddOpt(70, "edition", ("disc","Disc Edition",0), ("digital","Digital Edition",-100));
        // Products 71-73: consoles color
        for (int pid = 71; pid <= 73; pid++)
            AddOpt(pid, "color", ("black","Black",0), ("white","White",0));
        // Product 74: DualSense colors
        AddOpt(74, "color", ("white","White",0), ("midnight-black","Midnight Black",0), ("cosmic-red","Cosmic Red",0), ("starlight-blue","Starlight Blue",0));
        // Product 75: Xbox controller colors
        AddOpt(75, "color", ("carbon-black","Carbon Black",0), ("robot-white","Robot White",0), ("shock-blue","Shock Blue",0), ("pulse-red","Pulse Red",0));
        // Product 76: Razer
        AddOpt(76, "color", ("black","Black",0), ("green","Green",0));
        // Products 77-80: accessories
        AddOpt(77, "color", ("black","Black",0), ("white","White",0));
        AddOpt(78, "color", ("black","Black",0));
        AddOpt(79, "color", ("graphite","Graphite",0), ("pale-gray","Pale Gray",0));
        AddOpt(80, "color", ("white","White",0));

        // Now build variants as cartesian products per product
        var variants = new List<ProductVariant>();
        var variantOptionValues = new List<ProductVariantOptionValue>();
        int variantId = 1;
        int vovId = 1;

        // Base prices per product (from ProductSeed)
        var basePrices = new Dictionary<int, decimal>();
        for (int i = 1; i <= 80; i++)
        {
            basePrices[i] = i switch
            {
                1 => 1299.99m, 2 => 799.99m, 3 => 1199.99m, 4 => 799.99m, 5 => 449.99m,
                6 => 349.99m, 7 => 899.99m, 8 => 299.99m, 9 => 949.99m, 10 => 799.99m,
                11 => 999.99m, 12 => 499.99m, 13 => 699.99m, 14 => 599.99m, 15 => 129.99m,
                16 => 179.99m, 17 => 169.99m, 18 => 249.99m, 19 => 119.99m, 20 => 139.99m,
                21 => 1999.99m, 22 => 1299.99m, 23 => 1499.99m, 24 => 699.99m, 25 => 1399.99m,
                26 => 549.99m, 27 => 1799.99m, 28 => 599.99m, 29 => 1699.99m, 30 => 449.99m,
                31 => 2199.99m, 32 => 499.99m, 33 => 1099.99m, 34 => 1599.99m, 35 => 1449.99m,
                36 => 1099.99m, 37 => 599.99m, 38 => 499.99m, 39 => 1199.99m, 40 => 449.99m,
                41 => 399.99m, 42 => 149.99m, 43 => 499.99m, 44 => 249.99m, 45 => 169.99m,
                46 => 249.99m, 47 => 349.99m, 48 => 299.99m, 49 => 279.99m, 50 => 99.99m,
                51 => 79.99m, 52 => 179.99m, 53 => 59.99m, 54 => 1299.99m, 55 => 1499.99m,
                56 => 2199.99m, 57 => 349.99m, 58 => 499.99m, 59 => 379.99m, 60 => 1299.99m,
                61 => 899.99m, 62 => 2499.99m, 63 => 2499.99m, 64 => 2499.99m, 65 => 1199.99m,
                66 => 399.99m, 67 => 299.99m, 68 => 759.99m, 69 => 1699.99m, 70 => 499.99m,
                71 => 499.99m, 72 => 299.99m, 73 => 349.99m, 74 => 69.99m, 75 => 59.99m,
                76 => 79.99m, 77 => 35.99m, 78 => 65.99m, 79 => 99.99m, 80 => 39.99m,
                _ => 99.99m
            };
        }

        // SKU bases
        var skuBases = new Dictionary<int, string>();
        for (int i = 1; i <= 80; i++)
        {
            skuBases[i] = i switch
            {
                1 => "APL-IP15PM", 2 => "APL-IP15", 3 => "SAM-S24U", 4 => "SAM-S24",
                5 => "SAM-A55", 6 => "SAM-A35", 7 => "XIA-14P", 8 => "XIA-RN13P",
                9 => "HUA-P60P", 10 => "OP-12", 11 => "GOO-PX8P", 12 => "GOO-PX8A",
                13 => "APL-IP14", 14 => "SAM-S23FE", 15 => "XIA-R12C", 16 => "SAM-M14",
                17 => "RLM-C55", 18 => "OPP-A78", 19 => "TEC-SP20", 20 => "INF-H40",
                21 => "APL-MBP14M3", 22 => "APL-MBA15M3", 23 => "DEL-XPS15", 24 => "DEL-INS15",
                25 => "HP-SPX360", 26 => "HP-PAV15", 27 => "LEN-X1C", 28 => "LEN-IPS5",
                29 => "ASU-ROGG16", 30 => "ASU-VB15", 31 => "ACR-PH16", 32 => "ACR-ASP5",
                33 => "MSF-SP10", 34 => "HUA-MBXP", 35 => "SAM-GB4P",
                36 => "APL-IPP12M4", 37 => "APL-IPA11M2", 38 => "APL-IPMN7", 39 => "SAM-TS9U",
                40 => "SAM-TS9FE", 41 => "XIA-PAD6P", 42 => "AMZ-FHD10", 43 => "LEN-TP12P",
                44 => "APL-APP2", 45 => "APL-AP3", 46 => "SAM-GB3P", 47 => "SON-XM5",
                48 => "SON-WF5", 49 => "BOS-QC45", 50 => "JBL-T770", 51 => "ANK-LIB4",
                52 => "JBL-CHG5", 53 => "SON-XB100",
                54 => "SAM-Q80C", 55 => "LG-OLEDC3", 56 => "SON-BRXR", 57 => "XIA-TVA2",
                58 => "TCL-QLED", 59 => "HIS-UHD", 60 => "SAM-FRM", 61 => "LG-NANO",
                62 => "SON-A7IV", 63 => "CAN-R6MK2", 64 => "NIK-Z6III", 65 => "CAN-90D",
                66 => "GPR-H12B", 67 => "DJI-OA4", 68 => "DJI-MN4P", 69 => "FUJ-XT5",
                70 => "SON-PS5", 71 => "MSF-XSX", 72 => "MSF-XSS", 73 => "NIN-SWOLED",
                74 => "SON-DS5", 75 => "MSF-XWC", 76 => "RZR-KRK",
                77 => "ANK-65W", 78 => "ANK-PC26", 79 => "LOG-MXM3S", 80 => "APL-MGSC",
                _ => $"PRD-{i}"
            };
        }

        for (int productId = 1; productId <= 80; productId++)
        {
            var productOptions = options.Where(o => o.ProductId == productId).ToList();
            if (productOptions.Count == 0) continue;

            // Get the value sets for each option
            var valueSets = productOptions
                .Select(o => optionValues.Where(v => v.OptId == o.OptId).ToList())
                .ToList();

            // Create cartesian product
            var combos = CartesianProduct(valueSets);

            foreach (var combo in combos)
            {
                var priceAdj = combo.Sum(c => c.PriceAdj);
                var variantName = string.Join(" / ", combo.Select(c => c.Label));
                var sku = $"{skuBases[productId]}-V{variantId:D4}";
                var stockQty = 20 + ((variantId * 7) % 181);  // 20-200 deterministic

                // Determine color/size from combo
                string? color = null, size = null, material = null;
                foreach (var c in combo)
                {
                    var opt = options.First(o => o.OptId == c.OptId);
                    switch (opt.AttrKey)
                    {
                        case "color": color = c.Label; break;
                        case "size": size = c.Value; break;
                    }
                }

                variants.Add(new ProductVariant
                {
                    Id = variantId,
                    ProductId = productId,
                    Sku = sku,
                    VariantName = variantName,
                    Color = color,
                    Size = size,
                    Material = material,
                    PriceAdjustment = priceAdj,
                    StockQuantity = stockQty,
                    StockStatus = stockQty > 0 ? StockStatus.InStock : StockStatus.OutOfStock,
                    Status = ProductVariantStatus.Active,
                    CreatedAt = CreatedAt,
                    UpdatedAt = CreatedAt,
                    IsDeleted = false
                });

                // Junction records
                foreach (var c in combo)
                {
                    variantOptionValues.Add(new ProductVariantOptionValue
                    {
                        Id = vovId++,
                        ProductVariantId = variantId,
                        ProductOptionValueId = c.ValId,
                        CreatedAt = CreatedAt,
                        UpdatedAt = CreatedAt,
                        IsDeleted = false
                    });
                }

                variantId++;
            }
        }

        modelBuilder.Entity<ProductVariant>().HasData(variants.ToArray());
        modelBuilder.Entity<ProductVariantOptionValue>().HasData(variantOptionValues.ToArray());
    }

    /// <summary>
    /// Compute the cartesian product of multiple value lists.
    /// </summary>
    private static List<List<(int ValId, int OptId, int ProductId, string Value, string Label, decimal PriceAdj)>>
        CartesianProduct(List<List<(int ValId, int OptId, int ProductId, string Value, string Label, decimal PriceAdj)>> sets)
    {
        var result = new List<List<(int, int, int, string, string, decimal)>>();
        result.Add(new List<(int, int, int, string, string, decimal)>());

        foreach (var set in sets)
        {
            var newResult = new List<List<(int, int, int, string, string, decimal)>>();
            foreach (var existing in result)
            {
                foreach (var item in set)
                {
                    var combo = new List<(int, int, int, string, string, decimal)>(existing) { item };
                    newResult.Add(combo);
                }
            }
            result = newResult;
        }

        return result;
    }
}
