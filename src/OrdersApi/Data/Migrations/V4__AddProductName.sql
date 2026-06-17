-- ============================================================
-- V4__AddProductName.sql
-- Adds Name column to Products and backfills seed rows
-- ============================================================

ALTER TABLE "Products" ADD COLUMN IF NOT EXISTS "Name" VARCHAR(255) NOT NULL DEFAULT '';

-- Backfill names for seed products from V1
UPDATE "Products" SET "Name" = 'Blue Cotton T-Shirt'        WHERE "Id" = 'b2c3d4e5-0001-0001-0001-000000000001';
UPDATE "Products" SET "Name" = 'Black Slim-Fit Jeans'       WHERE "Id" = 'b2c3d4e5-0002-0002-0002-000000000002';
UPDATE "Products" SET "Name" = 'White Running Sneakers'     WHERE "Id" = 'b2c3d4e5-0003-0003-0003-000000000003';
UPDATE "Products" SET "Name" = 'Red Baseball Cap'           WHERE "Id" = 'b2c3d4e5-0004-0004-0004-000000000004';
