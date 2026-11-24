# Catalog demo seed and simple products

## Owner migration first

Generate the migration locally after obtaining this branch, review it, then apply it to the intended local test database. Suggested name: `SupportSimpleProductsAndCatalogImages`.

Expected model delta, no migration file is supplied:
- `CartItems.VariantId`: nullable.
- `OrderItems.ProductVariantId`: nullable.
- `InventoryHistories.ProductVariantId`: nullable CLR/model alignment (the previous relationship configuration was already optional; inspect the generated diff).
- Replace `UQ_Product_MainImage`: unique filtered index on `ProductId` with `IsMain = 1`, instead of global uniqueness on `IsMain`.
- Related optional foreign keys/index changes EF requires. Existing variant IDs must remain intact. No table, column, or customer-data deletion is intended. An empty Down path cannot safely restore NOT NULL after simple-product rows exist; review rollback requirements locally.

Do not start with seed enabled against the old schema. Startup auto-applies existing migrations in Development as before. This change does not generate/apply migrations or enable seeding.

## Data and images

Existing fixed IDs for 5 brands, 5 categories and 12 products are preserved. There are 16 active variants across the first 8 products and 4 simple products without variants (Magic Mouse, Galaxy Buds, ThinkVision Monitor and ProArt Monitor). Demo prices/specifications are fixtures, not current offers. Synthetic sales/review counters are zero for fresh rows.

JSON files live under Infrastructure/Persistence/Seed/Data. ProductImages, CategoryImages and BrandLogos use root-relative `/Images/seed/...svg` paths. Files live under API/wwwroot/Images/seed, not remote image hosts. With the user's current origin, for example, open `http://localhost:5000/Images/seed/laptops.svg`. `wwwroot` itself is not in the served URL.

The SVG files are original generic demo illustrations, not photographs of branded models or official brand logos. Alt text labels this. The product image table has one main image per product after the migration.

The seed adds missing IDs rather than skipping whole nonempty tables. Repeating it does not reset existing prices/stock or duplicate existing fixed IDs. If your database has independently created conflicting slugs/SKUs with other IDs, review those conflicts first; the seed does not overwrite or delete them. Restoring deleted demo rows on a later seed run is intentional. Do not run concurrent seed processes.

## How to run on your machine

1. Review the intended local DB and back up anything important. Generate/review/apply the owner migration.
2. The existing full seeder also ensures roles/admin. Check your PRIVATE AdminSeed settings before enabling it. Do not put passwords or secrets into committed JSON.
3. Set `DataInitialization:Enabled` to `true` only in local ignored development settings, start once, check the seed succeeded, then set it back to `false` and restart normally. No `down -v`, drop, reset or live DB steps are needed.
4. Read products, variants and image URLs. Confirm a main thumbnail and open a served image URL.

## Buying either type

- Product with variants: send an active variant's ID in `variantId`. Price adjustment, stock reservation and cancellation restock use that variant. Omitting it is rejected, including when only inactive variants exist.
- Simple product with zero variants: send `variantId: null` (or omit it). Pricing and stock come from Product; order items keep a null ProductVariantId. No hidden Standard variant is created.
- Inventory adjustment follows the same rule: ProductVariantId required when the product has variants, null for a simple product. Existing variant-specific adjustments remain valid.
- Existing carts/orders retain their stored IDs. If variants are added after a simple item enters a cart, checkout rejects omitted selection and asks for a variant. Cancelling a previously placed simple order still restocks the original product, not a later variant.

These tests cover ordinary single-process behavior and InMemory models, not production SQL race safety. SQL concurrency and provider end-to-end payment checks remain release gates.

Image path case follows the actual `wwwroot/Images/seed` folder. This JSON correction does not rewrite previously seeded image rows: the seeder still inserts missing IDs only. If a database already holds `/images/seed/...` URLs, review a separate targeted correction rather than rerunning seed expecting it to overwrite them. No migration is needed for this URL-only correction.
