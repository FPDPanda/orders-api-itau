-- ============================================================
-- V5__AddOrderItemUnitPrice.sql
-- Adds UnitPrice snapshot to OrderItems so historical order
-- totals are not affected by future product price changes
-- ============================================================

ALTER TABLE "OrderItems" ADD COLUMN IF NOT EXISTS "UnitPrice" DECIMAL(18,2) NOT NULL DEFAULT 0;

-- Backfill from current product prices (best approximation for existing rows)
UPDATE "OrderItems" oi
SET "UnitPrice" = p."Price"
FROM "Products" p
WHERE oi."ProductId" = p."Id";
