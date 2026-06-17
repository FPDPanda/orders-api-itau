# orders-api-itau

A simple .NET 8 Orders API with PostgreSQL (Docker) and MediatR.

## Project Structure

```
src/
└── OrdersApi/
    ├── Controllers/        # API Controllers (Orders, Products, Discounts)
    ├── Domain/
    │   ├── Models/         # Domain entities (Order, OrderItem, Product, Discount)
    │   └── Enums/          # Enumerations (OrderType, OrderStatus, DiscountType)
    ├── Dtos/               # Response DTOs with static From() factory methods
    ├── Requests/           # Request records (one per operation)
    ├── Commands/
    │   ├── Orders/         # Order write commands and handlers
    │   ├── Products/       # Product write commands and handlers
    │   └── Discounts/      # Discount write commands and handlers
    ├── Queries/
    │   ├── Orders/         # Order read queries and handlers
    │   ├── Products/       # Product read queries and handlers
    │   └── Discounts/      # Discount read queries and handlers
    ├── Repository/
    │   ├── Interfaces/     # Repository contracts
    │   └── Implementations/# EF Core repository implementations
    ├── Data/               # DbContext
    └── Data/Migrations/    # SQL migration scripts (V1–V5)
tests/
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Docker](https://www.docker.com/get-started)

## Getting Started

### 1. Start PostgreSQL and run migrations

```bash
start-db.bat
```

This starts the PostgreSQL Docker container, waits for it to be ready, and runs `V1__InitialCreate.sql` automatically. The migration is idempotent — safe to run multiple times.

Make sure `appsettings.Development.json` has a matching connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=ordersdb;Username=postgres;Password=postgres"
}
```

### 2. Run the API

```bash
cd src/OrdersApi
dotnet run
```

The API will be available at `http://localhost:5000`.

## Authentication

Authentication is controlled by the `Jwt:Enabled` flag in `appsettings.Development.json`. Set it to `false` to run without auth (Swagger will also hide the Bearer button):

```json
"Jwt": {
  "Enabled": true,
  "Key": "dev-only-secret-key-min-32-chars!!"
}
```

When enabled, all endpoints require a JWT Bearer token. To generate one for local testing, go to [jwt.io](https://jwt.io), keep the default header and payload, and set the **Verify Signature** secret to:

```
dev-only-secret-key-min-32-chars!!
```

Copy the generated token and pass it in the `Authorization` header:

```
Authorization: Bearer <token>
```

In production, set `Jwt__Enabled` and `Jwt__Key` via environment variables.

## Running Tests

### Run tests only

```bash
dotnet test tests/OrdersApi.Tests
```

### Run tests with coverage report

```bash
run-tests.bat
```

This runs all unit tests, generates an HTML coverage report under `coverage/report/`, and opens it automatically in the browser. The `coverage/` folder is git-ignored.

## API Endpoints

### Orders

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/orders` | Create a new order |
| GET | `/orders/{orderId}` | Get an order by ID |
| PATCH | `/orders/{orderId}/status` | Update the status of an order |
| POST | `/orders/{orderId}/items/{itemId}` | Add a product to an order |
| DELETE | `/orders/{orderId}/items/{itemId}` | Remove a product from an order |

### Products

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/products` | Create a new product |
| GET | `/products/{productId}` | Get a product by ID |
| PUT | `/products/{productId}` | Update a product |
| DELETE | `/products/{productId}` | Delete a product |

## Order Payload Example

```json
{
  "type": "Standard",
  "productIds": ["c100f88f-aedb-428a-a9ee-f8cf3b10af66"],
  "user": "jean@itau.com",
  "trackingURL": "https://www.guidgenerator.com"
}
```

> `originalValue` and `debitedValue` are calculated server-side from the sum of the provided product prices.

**Order Types:** `Standard`, `Express`, `Subscription`
**Order Statuses:** `New`, `Confirmed`, `Shipped`, `Completed`

## Product Payload Example

```json
{
  "imageURL": "https://www.guidgenerator.com",
  "description": "Bola de futebol",
  "price": 20.0
}
```

---

## Known Issues

### Business Logic & Security

| # | Description | Affected Files | Severity | Fixed On | PR |
|---|-------------|---------------|----------|----------|----|
| BL-01 | Client sends `OriginalValue` and `DebitedValue` in the request body — any caller can set the price to zero | `OrdersController`, `CreateOrderCommand`, `CreateOrderHandler` | Critical | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/5 |
| BL-02 | No authentication or authorization — all endpoints are publicly accessible | `Program.cs`, all controllers | Critical | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/6 |
| BL-03 | No status transition endpoint — orders are stuck at `New` forever | `OrdersController`, `Order.cs` | High | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/7 |
| BL-04 | No status transition guards — the flow `New → Confirmed → Shipped → Completed` is not enforced | `Order.cs` | High | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/8 |
| BL-05 | Items can be added or removed from an order regardless of its status (e.g. a `Shipped` order) | `OrdersController`, `OrderRepository` | High | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/9 |
| BL-06 | Discount logic is entirely client-side — no discount table, no rules per `OrderType`, no server-side calculation | `CreateOrderCommand`, `CreateOrderHandler` | High | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/10 |
| BL-07 | No quantity on order line items — `Products` is a flat list of IDs, impossible to order 2 units of the same product | `Order.cs`, `OrderRepository` | High | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/11 |
| BL-16 | `Product` has no `Name` field — `Description` was used as both display name and item description, conflating two distinct concerns | `Product.cs`, `ProductsController`, `CreateProductCommand`, `UpdateProductCommand` | Medium | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/11 |
| BL-08 | No price snapshot on order line items — updating a product price retroactively changes historical order totals | `Order.cs`, `OrderRepository` | High | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/12 |
| BL-09 | No input validation anywhere — negative prices, empty strings, and out-of-range decimals are accepted silently | All request records | High | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/13 |
| BL-10 | Removing an item from an order does not recalculate `OriginalValue` or `DebitedValue`, leaving order totals stale | `OrderRepository`, `Order.cs` | High | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/14 |
| BL-11 | `DebitedValue` can exceed `OriginalValue` or be negative with no validation | `CreateOrderRequest` | Medium | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/5 |
| BL-12 | `OrderType` enum has no behaviour — no price modifiers, SLA rules, or discount tiers differ between types | `OrderType.cs`, `CreateOrderHandler` | Medium | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/10 |
| BL-13 | Migration is never applied — `Program.cs` resolves the `DbContext` but never calls `Database.Migrate()` | `Program.cs` | Medium | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/2 |
| BL-14 | `TrackingURL` is client-provided at creation — it should be generated by the system when the order reaches `Shipped` | `CreateOrderRequest`, `CreateOrderHandler` | Medium | | |
| BL-15 | No listing endpoints — no way to list orders by user/status or browse products | `OrdersController`, `ProductsController` | Medium | | |

---

### Architecture & Code Quality

| # | Description | Affected Files | Severity | Fixed On | PR |
|---|-------------|---------------|----------|----------|----|
| AC-01 | Controllers return raw domain entities — no DTOs, exposes DB schema directly and makes contract changes breaking | All controllers and handlers | High | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/15 |
| AC-02 | `CreateOrderRequest` and `CreateProductRequest` are defined inside the controller files, not in their own files | `OrdersController.cs`, `ProductsController.cs` | Medium | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/16 |
| AC-03 | `UpdateProduct` reuses `CreateProductRequest` — semantically wrong, a create and update request are different contracts | `ProductsController.cs` | Medium | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/17 |
| AC-04 | All commands live under the `Queries` namespace and folder — commands and queries should be separated | `OrdersApi/Queries/` | Medium | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/18 |
| AC-05 | `PUT /orders/{id}/items/{itemId}` should be `POST` — `PUT` implies full replacement, not appending to a collection | `OrdersController.cs` | Medium | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/19 |
| AC-06 | Swagger UI is enabled for all environments — should be restricted to `Development` | `Program.cs` | Medium | | |
| AC-07 | `CancellationToken` is accepted by all handlers but never forwarded to repository or EF Core calls | All handlers, all repository methods | Low | | |
| AC-08 | No global exception handling middleware — unhandled exceptions return stack traces in the response | `Program.cs` | Low | | |
| AC-09 | No structured logging — no `ILogger` usage anywhere in handlers or repositories | All handlers, all repositories | Low | | |
