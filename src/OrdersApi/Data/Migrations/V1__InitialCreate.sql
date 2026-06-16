-- ============================================================
-- V1__InitialCreate.sql
-- Creates the initial schema and seeds sample data
-- ============================================================

-- Orders table
CREATE TABLE IF NOT EXISTS "Orders" (
    "Id"               UUID          NOT NULL DEFAULT gen_random_uuid(),
    "CreationDateTime" TIMESTAMPTZ   NOT NULL,
    "Type"             VARCHAR(50)   NOT NULL,
    "OriginalValue"    DECIMAL(18,2) NOT NULL,
    "DebitedValue"     DECIMAL(18,2) NOT NULL,
    "User"             VARCHAR(255)  NOT NULL,
    "Status"           VARCHAR(50)   NOT NULL,
    "TrackingURL"      VARCHAR(500)  NOT NULL DEFAULT '',
    CONSTRAINT "PK_Orders" PRIMARY KEY ("Id")
);

-- Products table
CREATE TABLE IF NOT EXISTS "Products" (
    "Id"          UUID          NOT NULL DEFAULT gen_random_uuid(),
    "ImageURL"    VARCHAR(500)  NOT NULL DEFAULT '',
    "Description" VARCHAR(1000) NOT NULL DEFAULT '',
    "Price"       DECIMAL(18,2) NOT NULL,
    "OrderId"     UUID          NULL,
    CONSTRAINT "PK_Products"         PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Products_Orders"  FOREIGN KEY ("OrderId")
        REFERENCES "Orders" ("Id") ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS "IX_Products_OrderId" ON "Products" ("OrderId");

-- ============================================================
-- Seed data
-- ============================================================

-- Orders
INSERT INTO "Orders" ("Id", "CreationDateTime", "Type", "OriginalValue", "DebitedValue", "User", "Status", "TrackingURL")
VALUES
    ('a1b2c3d4-0001-0001-0001-000000000001',
     NOW(),
     'Standard',
     199.90,
     199.90,
     'joao.silva@email.com',
     'Confirmed',
     'https://tracking.example.com/order/001'),

    ('a1b2c3d4-0002-0002-0002-000000000002',
     NOW(),
     'Express',
     549.50,
     549.50,
     'maria.santos@email.com',
     'Shipped',
     'https://tracking.example.com/order/002');

-- Products (linked to orders above)
INSERT INTO "Products" ("Id", "ImageURL", "Description", "Price", "OrderId")
VALUES
    ('b2c3d4e5-0001-0001-0001-000000000001',
     'https://images.example.com/tshirt-blue.png',
     'Blue cotton T-shirt, size M',
     49.90,
     'a1b2c3d4-0001-0001-0001-000000000001'),

    ('b2c3d4e5-0002-0002-0002-000000000002',
     'https://images.example.com/jeans-black.png',
     'Black slim-fit jeans, size 40',
     150.00,
     'a1b2c3d4-0001-0001-0001-000000000001'),

    ('b2c3d4e5-0003-0003-0003-000000000003',
     'https://images.example.com/sneaker-white.png',
     'White running sneakers, size 42',
     299.50,
     'a1b2c3d4-0002-0002-0002-000000000002'),

    ('b2c3d4e5-0004-0004-0004-000000000004',
     'https://images.example.com/cap-red.png',
     'Red baseball cap, one size',
     250.00,
     'a1b2c3d4-0002-0002-0002-000000000002');
