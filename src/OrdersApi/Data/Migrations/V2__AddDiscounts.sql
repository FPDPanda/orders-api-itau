-- ============================================================
-- V2__AddDiscounts.sql
-- Creates the Discounts table and seeds default rules per OrderType
-- ============================================================

CREATE TABLE IF NOT EXISTS "Discounts" (
    "Id"           UUID          NOT NULL DEFAULT gen_random_uuid(),
    "OrderType"    VARCHAR(50)   NOT NULL,
    "DiscountType" VARCHAR(50)   NOT NULL,
    "Rate"         DECIMAL(10,4) NOT NULL,
    "Active"       BOOLEAN       NOT NULL DEFAULT TRUE,
    CONSTRAINT "PK_Discounts" PRIMARY KEY ("Id")
);

-- Seed: one active rule per OrderType
-- Standard: no adjustment (0%)
-- Express:  +15% surcharge (positive rate)
-- Subscription: -10% discount (negative rate)
INSERT INTO "Discounts" ("Id", "OrderType", "DiscountType", "Rate", "Active")
VALUES
    ('c3d4e5f6-0001-0001-0001-000000000001', 'Standard',     'Percentage',  0.0000, TRUE),
    ('c3d4e5f6-0002-0002-0002-000000000002', 'Express',      'Percentage',  0.1500, TRUE),
    ('c3d4e5f6-0003-0003-0003-000000000003', 'Subscription', 'Percentage', -0.1000, TRUE)
ON CONFLICT ("Id") DO NOTHING;
