-- ============================================================
-- V3__AddOrderItems.sql
-- Introduces OrderItems join table (Order → Product + Quantity)
-- Migrates existing Product.OrderId data and drops that column
-- ============================================================

CREATE TABLE IF NOT EXISTS "OrderItems" (
    "Id"        UUID NOT NULL DEFAULT gen_random_uuid(),
    "OrderId"   UUID NOT NULL,
    "ProductId" UUID NOT NULL,
    "Quantity"  INT  NOT NULL DEFAULT 1,
    CONSTRAINT "PK_OrderItems"          PRIMARY KEY ("Id"),
    CONSTRAINT "FK_OrderItems_Orders"   FOREIGN KEY ("OrderId")   REFERENCES "Orders"   ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_OrderItems_Products" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_OrderItems_OrderId"   ON "OrderItems" ("OrderId");
CREATE INDEX IF NOT EXISTS "IX_OrderItems_ProductId" ON "OrderItems" ("ProductId");

-- Migrate existing Product→Order links into OrderItems rows (quantity 1 each)
INSERT INTO "OrderItems" ("Id", "OrderId", "ProductId", "Quantity")
SELECT gen_random_uuid(), p."OrderId", p."Id", 1
FROM "Products" p
WHERE p."OrderId" IS NOT NULL;

-- Remove the old FK column from Products
DROP INDEX IF EXISTS "IX_Products_OrderId";
ALTER TABLE "Products" DROP COLUMN IF EXISTS "OrderId";
