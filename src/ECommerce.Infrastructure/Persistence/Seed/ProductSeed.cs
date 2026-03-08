using ECommerce.Core.Entities.Product;
using ECommerce.Core.Enums.Inventory;
using ECommerce.Core.Enums.Product;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class ProductSeed
{
    private static readonly DateTime CreatedAt = new(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc);

    // ===== PRODUCT DEFINITIONS =====
    // Each tuple: (Id, Name, Slug, Description, BasePrice, CompareAtPrice, Sku, CategoryId, BrandId, StockQty, Status, IsBestSeller, IsNewArrival, Rating, ReviewCount)
    private static readonly (int Id, string Name, string Slug, string Desc, decimal BasePrice, decimal SalePrice,
        string Sku, int CatId, int BrandId, int Stock, ProductStatus Status, bool BestSeller, bool NewArrival,
        decimal Rating, int Reviews)[] Products =
    {
        // ===== SMARTPHONES (20) =====
        (1,  "iPhone 15 Pro Max",         "iphone-15-pro-max",         "The most advanced iPhone ever with A17 Pro chip. Features a 6.7-inch Super Retina XDR display and a 48MP camera system with 5x optical zoom.", 1299.99m, 1299.99m, "APL-IP15PM-001", CategorySeed.IPhones, 1, 250, ProductStatus.Published, true, true, 4.8m, 45),
        (2,  "iPhone 15",                 "iphone-15",                 "Features the powerful A16 Bionic chip, Dynamic Island, and a 48MP main camera. Available in five stunning new colors.", 799.99m, 799.99m, "APL-IP15-001", CategorySeed.IPhones, 1, 300, ProductStatus.Published, true, false, 4.6m, 38),
        (3,  "Samsung Galaxy S24 Ultra",  "samsung-galaxy-s24-ultra",  "Galaxy AI is here. The most powerful Galaxy with built-in AI, a 200MP camera, and the embedded S Pen for effortless creativity.", 1199.99m, 1199.99m, "SAM-S24U-001", CategorySeed.AndroidPhones, 2, 200, ProductStatus.Published, true, true, 4.7m, 42),
        (4,  "Samsung Galaxy S24",        "samsung-galaxy-s24",        "AI-powered Galaxy experience with a compact design. Features a 6.2-inch Dynamic AMOLED display and 50MP triple camera.", 799.99m, 799.99m, "SAM-S24-001", CategorySeed.AndroidPhones, 2, 250, ProductStatus.Published, false, true, 4.5m, 30),
        (5,  "Samsung Galaxy A55",        "samsung-galaxy-a55",        "Premium mid-range smartphone with a 6.6-inch Super AMOLED display, 50MP OIS camera, and 5000mAh battery. Water resistant IP67.", 449.99m, 449.99m, "SAM-A55-001", CategorySeed.AndroidPhones, 2, 400, ProductStatus.Published, false, false, 4.3m, 25),
        (6,  "Samsung Galaxy A35",        "samsung-galaxy-a35",        "Affordable Galaxy with flagship features including a 6.6-inch Super AMOLED display and 50MP camera. Long-lasting 5000mAh battery.", 349.99m, 349.99m, "SAM-A35-001", CategorySeed.AndroidPhones, 2, 500, ProductStatus.Published, false, false, 4.2m, 20),
        (7,  "Xiaomi 14 Pro",             "xiaomi-14-pro",             "Flagship smartphone with Leica optics, Snapdragon 8 Gen 3, and a stunning 6.73-inch 2K LTPO AMOLED display.", 899.99m, 899.99m, "XIA-14P-001", CategorySeed.AndroidPhones, 6, 150, ProductStatus.Published, false, true, 4.5m, 18),
        (8,  "Xiaomi Redmi Note 13 Pro",  "xiaomi-redmi-note-13-pro",  "Best-in-class 200MP camera in its price segment. Features a 6.67-inch AMOLED display and 67W turbo charging.", 299.99m, 299.99m, "XIA-RN13P-001", CategorySeed.AndroidPhones, 6, 600, ProductStatus.Published, true, false, 4.4m, 35),
        (9,  "Huawei P60 Pro",            "huawei-p60-pro",            "Ultra Lighting camera system with variable aperture. Features a 6.67-inch LTPO OLED display and ultra-reliable Kunlun Glass.", 949.99m, 949.99m, "HUA-P60P-001", CategorySeed.AndroidPhones, 5, 100, ProductStatus.Published, false, false, 4.3m, 15),
        (10, "OnePlus 12",                "oneplus-12",                "Flagship killer with Snapdragon 8 Gen 3, Hasselblad camera, and 100W SUPERVOOC charging. 6.82-inch 2K ProXDR display.", 799.99m, 799.99m, "OP-12-001", CategorySeed.AndroidPhones, 7, 200, ProductStatus.Published, false, true, 4.6m, 22),
        (11, "Google Pixel 8 Pro",        "google-pixel-8-pro",        "The best of Google AI in a phone. Features Tensor G3, a 50MP main camera with AI photography, and 7 years of OS updates.", 999.99m, 999.99m, "GOO-PX8P-001", CategorySeed.AndroidPhones, 30, 150, ProductStatus.Published, false, true, 4.5m, 28),
        (12, "Google Pixel 8a",           "google-pixel-8a",           "Pixel intelligence at an accessible price. Features AI-powered camera, 6.1-inch OLED display, and Tensor G3 chip.", 499.99m, 499.99m, "GOO-PX8A-001", CategorySeed.AndroidPhones, 30, 300, ProductStatus.Published, false, false, 4.3m, 20),
        (13, "iPhone 14",                 "iphone-14",                 "The previous generation iPhone with A15 Bionic chip, crash detection, and an impressive camera system. Great value.", 699.99m, 799.99m, "APL-IP14-001", CategorySeed.IPhones, 1, 350, ProductStatus.Published, false, false, 4.5m, 50),
        (14, "Samsung Galaxy S23 FE",     "samsung-galaxy-s23-fe",     "Fan Edition brings flagship features to a more affordable price point. 6.4-inch Dynamic AMOLED display and 50MP camera.", 599.99m, 649.99m, "SAM-S23FE-001", CategorySeed.AndroidPhones, 2, 250, ProductStatus.Published, false, false, 4.3m, 22),
        (15, "Xiaomi Redmi 12C",          "xiaomi-redmi-12c",          "Budget-friendly smartphone with a 6.71-inch display, 50MP main camera, and 5000mAh battery for all-day use.", 129.99m, 129.99m, "XIA-R12C-001", CategorySeed.AndroidPhones, 6, 800, ProductStatus.Published, false, false, 4.0m, 15),
        (16, "Samsung Galaxy M14",        "samsung-galaxy-m14",        "Monster battery smartphone with 6000mAh battery, 50MP triple camera, and 6.6-inch Full HD+ display.", 179.99m, 179.99m, "SAM-M14-001", CategorySeed.AndroidPhones, 2, 600, ProductStatus.Published, false, false, 4.1m, 12),
        (17, "Realme C55",                "realme-c55",                "Dynamic Island-inspired Mini Capsule. Features a 64MP main camera, 6.72-inch FHD+ display, and 33W SUPERVOOC charge.", 169.99m, 169.99m, "RLM-C55-001", CategorySeed.AndroidPhones, 30, 500, ProductStatus.Published, false, false, 4.0m, 10),
        (18, "OPPO A78",                  "oppo-a78",                  "Stylish smartphone with 67W SUPERVOOC flash charge, 50MP camera, and 6.43-inch AMOLED display. 5000mAh battery.", 249.99m, 249.99m, "OPP-A78-001", CategorySeed.AndroidPhones, 30, 400, ProductStatus.Published, false, false, 4.1m, 14),
        (19, "Tecno Spark 20",            "tecno-spark-20",            "Entertainment powerhouse with a 6.56-inch display, 32MP AI selfie camera, and 5000mAh battery with Type-C charging.", 119.99m, 119.99m, "TEC-SP20-001", CategorySeed.AndroidPhones, 30, 700, ProductStatus.Published, false, false, 3.9m, 8),
        (20, "Infinix Hot 40",            "infinix-hot-40",            "Powerful gaming smartphone with MediaTek Helio G88, 6.56-inch 90Hz display, and 50MP dual camera. Style meets performance.", 139.99m, 139.99m, "INF-H40-001", CategorySeed.AndroidPhones, 30, 600, ProductStatus.Published, false, false, 3.8m, 7),

        // ===== LAPTOPS (15) =====
        (21, "MacBook Pro 14-inch M3 Pro",  "macbook-pro-14-m3-pro",   "Supercharged by M3 Pro chip with up to 18-core GPU. Features a stunning Liquid Retina XDR display and up to 18 hours battery.", 1999.99m, 1999.99m, "APL-MBP14M3-001", CategorySeed.Ultrabooks, 1, 100, ProductStatus.Published, true, true, 4.9m, 35),
        (22, "MacBook Air 15-inch M3",      "macbook-air-15-m3",       "Impossibly thin 15-inch laptop with M3 chip, 18-hour battery, and a brilliant Liquid Retina display. Fanless design.", 1299.99m, 1299.99m, "APL-MBA15M3-001", CategorySeed.Ultrabooks, 1, 150, ProductStatus.Published, true, true, 4.8m, 30),
        (23, "Dell XPS 15",                 "dell-xps-15",             "Premium 15.6-inch InfinityEdge display laptop with 13th Gen Intel Core, NVIDIA GeForce RTX graphics, and CNC aluminum build.", 1499.99m, 1499.99m, "DEL-XPS15-001", CategorySeed.Ultrabooks, 8, 120, ProductStatus.Published, false, false, 4.5m, 25),
        (24, "Dell Inspiron 15",            "dell-inspiron-15",         "Versatile everyday laptop with 13th Gen Intel Core i7, 15.6-inch FHD display, and long battery life for work and entertainment.", 699.99m, 699.99m, "DEL-INS15-001", CategorySeed.LaptopsComputers, 8, 300, ProductStatus.Published, false, false, 4.2m, 20),
        (25, "HP Spectre x360",             "hp-spectre-x360",          "Premium 2-in-1 convertible with OLED display, Intel Core Ultra, and gem-cut design. Includes HP MPP2.0 tilt pen.", 1399.99m, 1399.99m, "HP-SPX360-001", CategorySeed.Ultrabooks, 9, 80, ProductStatus.Published, false, true, 4.6m, 18),
        (26, "HP Pavilion 15",              "hp-pavilion-15",           "Reliable everyday computing with AMD Ryzen 7, 15.6-inch FHD display, and B&O audio. Perfect for students and professionals.", 549.99m, 549.99m, "HP-PAV15-001", CategorySeed.LaptopsComputers, 9, 400, ProductStatus.Published, false, false, 4.1m, 15),
        (27, "Lenovo ThinkPad X1 Carbon",   "lenovo-thinkpad-x1-carbon","Iconic business ultrabook with 14-inch 2.8K OLED display, Intel Core Ultra vPro, and MIL-STD-810H durability. Sub-1.2kg.", 1799.99m, 1799.99m, "LEN-X1C-001", CategorySeed.Ultrabooks, 10, 90, ProductStatus.Published, false, false, 4.7m, 22),
        (28, "Lenovo IdeaPad Slim 5",       "lenovo-ideapad-slim-5",    "Thin and light laptop with AMD Ryzen 7, 14-inch FHD IPS display, and rapid charge. Up to 14 hours of battery life.", 599.99m, 599.99m, "LEN-IPS5-001", CategorySeed.LaptopsComputers, 10, 350, ProductStatus.Published, false, false, 4.2m, 16),
        (29, "Asus ROG Strix G16",          "asus-rog-strix-g16",       "Gaming beast with Intel Core i9-13980HX, NVIDIA GeForce RTX 4070, 16-inch QHD 240Hz display, and MUX Switch.", 1699.99m, 1699.99m, "ASU-ROGG16-001", CategorySeed.GamingLaptops, 11, 60, ProductStatus.Published, true, true, 4.6m, 20),
        (30, "Asus VivoBook 15",            "asus-vivobook-15",         "Everyday laptop with AMD Ryzen 5, 15.6-inch FHD display, fingerprint sensor, and ErgoLift hinge design.", 449.99m, 449.99m, "ASU-VB15-001", CategorySeed.LaptopsComputers, 11, 400, ProductStatus.Published, false, false, 4.0m, 12),
        (31, "Acer Predator Helios 16",     "acer-predator-helios-16",  "High-performance gaming laptop with Intel Core i9, NVIDIA RTX 4080, 16-inch WQXGA 240Hz IPS display, and AeroBlade 3D fan.", 2199.99m, 2199.99m, "ACR-PH16-001", CategorySeed.GamingLaptops, 12, 40, ProductStatus.Published, false, true, 4.5m, 15),
        (32, "Acer Aspire 5",               "acer-aspire-5",            "Great value laptop with 13th Gen Intel Core i5, 15.6-inch Full HD IPS display, and Wi-Fi 6. Perfect for daily productivity.", 499.99m, 499.99m, "ACR-ASP5-001", CategorySeed.LaptopsComputers, 12, 500, ProductStatus.Published, false, false, 4.1m, 18),
        (33, "Microsoft Surface Pro 10",    "microsoft-surface-pro-10", "The most powerful Surface Pro with Intel Core Ultra, optional OLED display, and all-day battery. Tablet meets laptop.", 1099.99m, 1099.99m, "MSF-SP10-001", CategorySeed.Tablets, 30, 100, ProductStatus.Published, false, true, 4.4m, 14),
        (34, "Huawei MateBook X Pro",       "huawei-matebook-x-pro",    "Ultra-slim premium laptop with 14.2-inch 3.1K LTPS touchscreen, Intel Core Ultra 9, and 6-speaker sound system.", 1599.99m, 1599.99m, "HUA-MBXP-001", CategorySeed.Ultrabooks, 5, 70, ProductStatus.Published, false, false, 4.4m, 10),
        (35, "Samsung Galaxy Book4 Pro",    "samsung-galaxy-book4-pro", "Ultra-light laptop with 16-inch Dynamic AMOLED 2X display, Intel Core Ultra 7, and seamless Galaxy ecosystem integration.", 1449.99m, 1449.99m, "SAM-GB4P-001", CategorySeed.Ultrabooks, 2, 80, ProductStatus.Published, false, true, 4.3m, 12),

        // ===== TABLETS (8) =====
        (36, "iPad Pro 12.9-inch M4",  "ipad-pro-12-9-m4",   "The ultimate iPad experience with M4 chip, Ultra Retina XDR display with tandem OLED, and Apple Pencil Pro support.", 1099.99m, 1099.99m, "APL-IPP12M4-001", CategorySeed.Tablets, 1, 120, ProductStatus.Published, true, true, 4.8m, 25),
        (37, "iPad Air 11-inch M2",    "ipad-air-11-m2",      "Powerful and versatile with M2 chip, 11-inch Liquid Retina display, and support for Apple Pencil and Magic Keyboard.", 599.99m, 599.99m, "APL-IPA11M2-001", CategorySeed.Tablets, 1, 200, ProductStatus.Published, false, true, 4.6m, 20),
        (38, "iPad mini 7",            "ipad-mini-7",         "Small but mighty with A16 Bionic chip, 8.3-inch Liquid Retina display, and Apple Pencil support. Ultra-portable creativity.", 499.99m, 499.99m, "APL-IPMN7-001", CategorySeed.Tablets, 1, 180, ProductStatus.Published, false, false, 4.5m, 15),
        (39, "Samsung Galaxy Tab S9 Ultra", "galaxy-tab-s9-ultra", "The biggest Galaxy Tab with 14.6-inch Dynamic AMOLED 2X display, Snapdragon 8 Gen 2, and S Pen included. IP68 water resistant.", 1199.99m, 1199.99m, "SAM-TS9U-001", CategorySeed.Tablets, 2, 80, ProductStatus.Published, true, false, 4.6m, 18),
        (40, "Samsung Galaxy Tab S9 FE",    "galaxy-tab-s9-fe",    "Fan Edition tablet with 10.9-inch display, Exynos 1380, S Pen included, and IP68 water resistance at an accessible price.", 449.99m, 449.99m, "SAM-TS9FE-001", CategorySeed.Tablets, 2, 250, ProductStatus.Published, false, false, 4.3m, 14),
        (41, "Xiaomi Pad 6 Pro",       "xiaomi-pad-6-pro",    "High-performance tablet with 11-inch 2.8K display, Snapdragon 8+ Gen 1, 8600mAh battery, and 67W fast charging.", 399.99m, 399.99m, "XIA-PAD6P-001", CategorySeed.Tablets, 6, 200, ProductStatus.Published, false, false, 4.4m, 12),
        (42, "Amazon Fire HD 10",      "amazon-fire-hd-10",   "Affordable 10.1-inch Full HD tablet with 3GB RAM, octa-core processor, and up to 13 hours of battery. Alexa built-in.", 149.99m, 149.99m, "AMZ-FHD10-001", CategorySeed.Tablets, 29, 500, ProductStatus.Published, false, false, 4.0m, 30),
        (43, "Lenovo Tab P12 Pro",     "lenovo-tab-p12-pro",  "Premium Android tablet with 12.6-inch AMOLED display, Snapdragon 870, quad JBL speakers, and stylus included.", 499.99m, 599.99m, "LEN-TP12P-001", CategorySeed.Tablets, 10, 100, ProductStatus.Published, false, false, 4.2m, 10),

        // ===== HEADPHONES & AUDIO (10) =====
        (44, "AirPods Pro 2nd Gen",     "airpods-pro-2",         "Active Noise Cancellation and Adaptive Transparency. USB-C MagSafe case, personalized spatial audio, and up to 6 hours listening.", 249.99m, 249.99m, "APL-APP2-001", CategorySeed.WirelessEarbuds, 1, 500, ProductStatus.Published, true, false, 4.7m, 50),
        (45, "AirPods 3rd Gen",         "airpods-3",             "Personalized spatial audio with dynamic head tracking. Sweat and water resistant with MagSafe charging case.", 169.99m, 169.99m, "APL-AP3-001", CategorySeed.WirelessEarbuds, 1, 400, ProductStatus.Published, false, false, 4.4m, 35),
        (46, "Samsung Galaxy Buds3 Pro","galaxy-buds3-pro",      "Premium true wireless earbuds with AI-powered noise cancellation, 360 audio, and blade-light design. Hi-Fi 24-bit audio.", 249.99m, 249.99m, "SAM-GB3P-001", CategorySeed.WirelessEarbuds, 2, 300, ProductStatus.Published, false, true, 4.5m, 20),
        (47, "Sony WH-1000XM5",        "sony-wh-1000xm5",      "Industry-leading noise canceling over-ear headphones with 30-hour battery, multipoint connection, and speak-to-chat.", 349.99m, 349.99m, "SON-XM5-001", CategorySeed.OverEarHeadphones, 3, 200, ProductStatus.Published, true, false, 4.8m, 45),
        (48, "Sony WF-1000XM5",        "sony-wf-1000xm5",      "The world's best noise-canceling truly wireless earbuds. Hi-Res Audio Wireless, 24-hour battery with case, and LDAC codec.", 299.99m, 299.99m, "SON-WF5-001", CategorySeed.WirelessEarbuds, 3, 250, ProductStatus.Published, true, true, 4.7m, 38),
        (49, "Bose QuietComfort 45",    "bose-quietcomfort-45",  "Legendary noise canceling headphones with TriPort acoustic architecture, 24-hour battery, and Aware mode for transparency.", 279.99m, 329.99m, "BOS-QC45-001", CategorySeed.OverEarHeadphones, 13, 180, ProductStatus.Published, false, false, 4.5m, 40),
        (50, "JBL Tune 770NC",         "jbl-tune-770nc",        "Wireless over-ear headphones with active noise canceling, JBL Pure Bass sound, 70-hour battery, and multi-point connection.", 99.99m, 99.99m, "JBL-T770-001", CategorySeed.OverEarHeadphones, 14, 400, ProductStatus.Published, false, false, 4.2m, 25),
        (51, "Anker Soundcore Liberty 4","anker-liberty-4",      "True wireless earbuds with ACAA 3.0 drivers, spatial audio, heart rate monitor, and all-day ANC. 28-hour total playtime.", 79.99m, 99.99m, "ANK-LIB4-001", CategorySeed.WirelessEarbuds, 19, 350, ProductStatus.Published, false, false, 4.3m, 22),
        (52, "JBL Charge 5",            "jbl-charge-5",          "Portable Bluetooth speaker with powerful JBL Pro Sound, IP67 waterproof, 20-hour battery, and built-in power bank.", 179.99m, 179.99m, "JBL-CHG5-001", CategorySeed.Speakers, 14, 300, ProductStatus.Published, true, false, 4.6m, 35),
        (53, "Sony SRS-XB100",          "sony-srs-xb100",        "Ultra-portable wireless speaker with Extra Bass, IP67 rating, 16-hour battery, and hands-free calling. Compact and colorful.", 59.99m, 59.99m, "SON-XB100-001", CategorySeed.Speakers, 3, 500, ProductStatus.Published, false, false, 4.2m, 20),

        // ===== TVs (8) =====
        (54, "Samsung QLED 65-inch Q80C", "samsung-qled-65-q80c", "Premium 4K QLED TV with Quantum Processor, Direct Full Array, Object Tracking Sound+, and Gaming Hub for cloud gaming.", 1299.99m, 1499.99m, "SAM-Q80C65-001", CategorySeed.SmartTVs, 2, 60, ProductStatus.Published, true, false, 4.5m, 18),
        (55, "LG OLED C3 55-inch",        "lg-oled-c3-55",        "Self-lit OLED pixels for perfect blacks and infinite contrast. Alpha 9 Gen6 AI Processor, Dolby Vision, and 120Hz gaming.", 1499.99m, 1799.99m, "LG-OLEDC3-001", CategorySeed.SmartTVs, 4, 50, ProductStatus.Published, true, false, 4.7m, 22),
        (56, "Sony Bravia XR 65-inch OLED","sony-bravia-xr-65",   "Cognitive Processor XR for lifelike pictures. Features Acoustic Surface Audio+, BRAVIA CORE, and Google TV built-in.", 2199.99m, 2499.99m, "SON-BRXR65-001", CategorySeed.SmartTVs, 3, 30, ProductStatus.Published, false, false, 4.6m, 15),
        (57, "Xiaomi Smart TV A2 43-inch", "xiaomi-tv-a2-43",     "Affordable 4K UHD smart TV with Dolby Vision, DTS:X, MEMC technology, and built-in Chromecast and Google Assistant.", 349.99m, 399.99m, "XIA-TVA243-001", CategorySeed.SmartTVs, 6, 200, ProductStatus.Published, false, false, 4.1m, 12),
        (58, "TCL 55-inch 4K QLED",       "tcl-55-4k-qled",      "QLED color technology for vivid picture quality. Features Google TV, Dolby Vision Atmos, and Game Master 2.0.", 499.99m, 599.99m, "TCL-55QLED-001", CategorySeed.SmartTVs, 30, 150, ProductStatus.Published, false, false, 4.2m, 14),
        (59, "Hisense 50-inch 4K UHD",    "hisense-50-4k-uhd",   "Quantum Dot Color technology with Dolby Vision HDR, DTS Virtual:X, and VIDAA smart TV platform. Game Mode Plus.", 379.99m, 449.99m, "HIS-50UHD-001", CategorySeed.SmartTVs, 30, 180, ProductStatus.Published, false, false, 4.0m, 10),
        (60, "Samsung Frame TV 55-inch",   "samsung-frame-tv-55",  "Art-inspired TV that transforms into a beautiful frame. QLED 4K with Art Mode, customizable bezels, and anti-reflection matte display.", 1299.99m, 1499.99m, "SAM-FRM55-001", CategorySeed.SmartTVs, 2, 70, ProductStatus.Published, false, true, 4.4m, 16),
        (61, "LG NanoCell 65-inch",        "lg-nanocell-65",       "NanoCell technology filters out color impurities for pure, accurate colors. Alpha 7 Gen6 AI Processor, webOS 23, and ThinQ AI.", 899.99m, 1099.99m, "LG-NANO65-001", CategorySeed.SmartTVs, 4, 90, ProductStatus.Published, false, false, 4.2m, 12),

        // ===== CAMERAS (8) =====
        (62, "Sony Alpha A7 IV",        "sony-alpha-a7-iv",      "Full-frame mirrorless camera with 33MP Exmor R sensor, BIONZ XR engine, real-time tracking AF, and 4K60p 10-bit video.", 2499.99m, 2499.99m, "SON-A7IV-001", CategorySeed.MirrorlessCameras, 3, 40, ProductStatus.Published, true, false, 4.7m, 20),
        (63, "Canon EOS R6 Mark II",    "canon-eos-r6-mark-ii",  "High-speed full-frame mirrorless camera with 24.2MP sensor, up to 40fps electronic shutter, and 6K 60p RAW video recording.", 2499.99m, 2499.99m, "CAN-R6MK2-001", CategorySeed.MirrorlessCameras, 15, 35, ProductStatus.Published, false, true, 4.6m, 18),
        (64, "Nikon Z6 III",            "nikon-z6-iii",          "Next-generation full-frame mirrorless with 24.5MP stacked CMOS sensor, partially stacked design, and N-RAW video recording.", 2499.99m, 2499.99m, "NIK-Z6III-001", CategorySeed.MirrorlessCameras, 16, 30, ProductStatus.Published, false, true, 4.5m, 12),
        (65, "Canon EOS 90D",           "canon-eos-90d",         "Versatile DSLR with 32.5MP APS-C sensor, 45-point cross-type AF, 10fps shooting, and uncropped 4K UHD video.", 1199.99m, 1299.99m, "CAN-90D-001", CategorySeed.DSLRCameras, 15, 50, ProductStatus.Published, false, false, 4.4m, 22),
        (66, "GoPro HERO12 Black",      "gopro-hero12-black",    "Compact action camera with 5.3K60 video, HyperSmooth 6.0 stabilization, HDR photo and video, and Max Lens Mod 2.0 support.", 399.99m, 399.99m, "GPR-H12B-001", CategorySeed.ActionCameras, 17, 200, ProductStatus.Published, true, true, 4.5m, 30),
        (67, "DJI Osmo Action 4",       "dji-osmo-action-4",     "Premium action camera with 1/1.3-inch sensor, 4K120 stabilized video, 160-min battery, and waterproof to 18m without a case.", 299.99m, 349.99m, "DJI-OA4-001", CategorySeed.ActionCameras, 18, 150, ProductStatus.Published, false, true, 4.4m, 15),
        (68, "DJI Mini 4 Pro",          "dji-mini-4-pro",        "Sub-249g camera drone with 4K60 HDR video, omnidirectional obstacle sensing, 34-min flight time, and ActiveTrack 360.", 759.99m, 759.99m, "DJI-MN4P-001", CategorySeed.Drones, 18, 80, ProductStatus.Published, true, true, 4.6m, 20),
        (69, "Fujifilm X-T5",           "fujifilm-x-t5",         "Rangefinder-style mirrorless camera with 40.2MP X-Trans 5 HR sensor, 7-stop IBIS, and classic Fujifilm color science simulations.", 1699.99m, 1699.99m, "FUJ-XT5-001", CategorySeed.MirrorlessCameras, 30, 45, ProductStatus.Published, false, false, 4.6m, 16),

        // ===== GAMING (7) =====
        (70, "PlayStation 5",             "playstation-5",            "Experience lightning-fast loading, haptic feedback, adaptive triggers, and stunning 4K gaming with the PS5 console.", 499.99m, 499.99m, "SON-PS5-001", CategorySeed.Consoles, 3, 100, ProductStatus.Published, true, false, 4.8m, 50),
        (71, "Xbox Series X",             "xbox-series-x",            "The fastest, most powerful Xbox ever with 12 teraflops of processing power, 4K gaming at up to 120fps, and Quick Resume.", 499.99m, 499.99m, "MSF-XSX-001", CategorySeed.Consoles, 30, 80, ProductStatus.Published, true, false, 4.7m, 35),
        (72, "Xbox Series S",             "xbox-series-s",            "Next-gen performance in the smallest Xbox ever. Supports up to 1440p at 120fps, features Quick Resume, and is all-digital.", 299.99m, 299.99m, "MSF-XSS-001", CategorySeed.Consoles, 30, 150, ProductStatus.Published, false, false, 4.4m, 25),
        (73, "Nintendo Switch OLED",      "nintendo-switch-oled",     "Enhanced with a vibrant 7-inch OLED screen, wide adjustable stand, 64GB internal storage, and enhanced audio.", 349.99m, 349.99m, "NIN-SWOLED-001", CategorySeed.Consoles, 30, 120, ProductStatus.Published, true, false, 4.6m, 40),
        (74, "DualSense Controller PS5",  "dualsense-controller-ps5", "Wireless controller with haptic feedback, adaptive triggers, built-in microphone, and ergonomic design for PlayStation 5.", 69.99m, 69.99m, "SON-DS5-001", CategorySeed.ControllersAccessories, 3, 500, ProductStatus.Published, false, false, 4.5m, 30),
        (75, "Xbox Wireless Controller",  "xbox-wireless-controller", "Sculpted surfaces and refined geometry for enhanced comfort. Textured grip, Bluetooth, and Share button.", 59.99m, 59.99m, "MSF-XWC-001", CategorySeed.ControllersAccessories, 30, 400, ProductStatus.Published, false, false, 4.4m, 25),
        (76, "Razer Kraken Gaming Headset","razer-kraken-headset",    "Gaming headset with custom-tuned 50mm drivers, cooling gel-infused ear cushions, and retractable noise-isolating microphone.", 79.99m, 79.99m, "RZR-KRK-001", CategorySeed.GamingHeadsets, 30, 300, ProductStatus.Published, false, false, 4.3m, 20),

        // ===== ACCESSORIES (4) =====
        (77, "Anker 65W USB-C Charger",       "anker-65w-usb-c-charger",   "Compact GaN II charger with 65W PD output. Charges MacBook Pro in 2 hours. Foldable plug with 3 ports.", 35.99m, 45.99m, "ANK-65W-001", CategorySeed.ChargersCables, 19, 1000, ProductStatus.Published, true, false, 4.6m, 40),
        (78, "Anker PowerCore 26800",         "anker-powercore-26800",     "Ultra-high capacity 26800mAh portable charger with dual USB-A and USB-C output. Charges iPhone 14 over 6 times.", 65.99m, 79.99m, "ANK-PC26-001", CategorySeed.PowerBanks, 19, 500, ProductStatus.Published, true, false, 4.5m, 35),
        (79, "Logitech MX Master 3S Mouse",   "logitech-mx-master-3s",    "Advanced wireless mouse with 8K DPI tracking, MagSpeed scroll, quiet clicks, and ergonomic shape. Works on glass.", 99.99m, 99.99m, "LOG-MXM3S-001", CategorySeed.KeyboardsMice, 20, 300, ProductStatus.Published, true, false, 4.7m, 30),
        (80, "Apple MagSafe Charger",         "apple-magsafe-charger",     "Magnetically attaches to iPhone for faster, efficient wireless charging up to 15W. Compatible with Qi-enabled devices.", 39.99m, 39.99m, "APL-MGSC-001", CategorySeed.ChargersCables, 1, 800, ProductStatus.Published, false, false, 4.3m, 25),
    };

    public static void SeedProducts(ModelBuilder modelBuilder)
    {
        var products = new List<Product>();
        foreach (var p in Products)
        {
            products.Add(new Product
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Description = p.Desc,
                BasePrice = p.BasePrice,
                SalePrice = p.SalePrice,
                Sku = p.Sku,
                CategoryId = p.CatId,
                BrandId = p.BrandId,
                StockQuantity = p.Stock,
                StockStatus = p.Stock > 50 ? StockStatus.HighStock : (p.Stock > 0 ? StockStatus.InStock : StockStatus.OutOfStock),
                Status = p.Status,
                IsBestSeller = p.BestSeller,
                IsNewArrival = p.NewArrival,
                IsHotDeal = p.SalePrice < p.BasePrice,
                IsTopRated = p.Rating >= 4.5m,
                IsFeatured = p.BestSeller || p.NewArrival,
                AverageRating = p.Rating,
                ReviewCount = p.Reviews,
                CreatedAt = CreatedAt.AddDays(p.Id),
                UpdatedAt = CreatedAt.AddDays(p.Id + 30),
                IsDeleted = false
            });
        }

        modelBuilder.Entity<Product>().HasData(products.ToArray());
    }
}
