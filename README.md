# orders-api-itau

A simple .NET 8 Orders API with PostgreSQL (Docker) and MediatR.

## Project Structure

```
src/
└── OrdersApi/
    ├── Controllers/        # API Controllers (Orders, Products)
    ├── Domain/
    │   ├── Models/         # Domain entities (Order, Product)
    │   └── Enums/          # Enumerations (OrderType, OrderStatus)
    ├── Queries/
    │   ├── Orders/         # Order commands, queries and handlers
    │   └── Products/       # Product commands, queries and handlers
    ├── Repository/
    │   ├── Interfaces/     # Repository contracts
    │   └── Implementations/# EF Core repository implementations
    ├── Data/               # DbContext
    └── Migrations/         # EF Core migrations
tests/
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Docker](https://www.docker.com/get-started)

## Getting Started

### 1. Start PostgreSQL with Docker

```bash
start-db.bat
```

Make sure `appsettings.Development.json` has a matching connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=ordersdb;Username=postgres;Password=postgres"
}
```

### 2. Seed the database

```bash
docker exec -i orders-postgres psql -U postgres -d ordersdb < src/OrdersApi/Data/Migrations/V1__InitialCreate.sql
```

This creates the schema and inserts two sample orders with products.

### 3. Run the API

```bash
cd src/OrdersApi
dotnet run
```

The API will be available at `http://localhost:5000`.  
Migrations are applied automatically on startup in the `Development` environment.

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
| PUT | `/orders/{orderId}/items/{itemId}` | Add a product to an order |
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
  "originalValue": 10.0,
  "debitedValue": 8.50,
  "products": ["c100f88f-aedb-428a-a9ee-f8cf3b10af66"],
  "user": "jean@itau.com",
  "trackingURL": "https://www.guidgenerator.com"
}
```

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
| BL-01 | Client sends `OriginalValue` and `DebitedValue` in the request body — any caller can set the price to zero | `OrdersController`, `CreateOrderCommand`, `CreateOrderHandler` | Critical | | |
| BL-02 | No authentication or authorization — all endpoints are publicly accessible | `Program.cs`, all controllers | Critical | | |
| BL-03 | No status transition endpoint — orders are stuck at `New` forever | `OrdersController`, `Order.cs` | High | | |
| BL-04 | No status transition guards — the flow `New → Confirmed → Shipped → Completed` is not enforced | `Order.cs` | High | | |
| BL-05 | Items can be added or removed from an order regardless of its status (e.g. a `Shipped` order) | `OrdersController`, `OrderRepository` | High | | |
| BL-06 | Discount logic is entirely client-side — no discount table, no rules per `OrderType`, no server-side calculation | `CreateOrderCommand`, `CreateOrderHandler` | High | | |
| BL-07 | No quantity on order line items — `Products` is a flat list of IDs, impossible to order 2 units of the same product | `Order.cs`, `OrderRepository` | High | | |
| BL-08 | No price snapshot on order line items — updating a product price retroactively changes historical order totals | `Order.cs`, `OrderRepository` | High | | |
| BL-09 | `User` is a free-text string with no validation against an identity system and can be spoofed | `Order.cs`, `CreateOrderRequest` | High | | |
| BL-10 | No input validation anywhere — negative prices, empty strings, and out-of-range decimals are accepted silently | All request records | High | | |
| BL-11 | Deleting a product sets `OrderId` to null on all related products but leaves the order total incorrect | `ProductRepository`, `OrdersDbContext` | High | | |
| BL-12 | `TrackingURL` is client-provided at creation — it should be generated by the system when the order reaches `Shipped` | `CreateOrderRequest`, `CreateOrderHandler` | Medium | | |
| BL-13 | `DebitedValue` can exceed `OriginalValue` or be negative with no validation | `CreateOrderRequest` | Medium | | |
| BL-14 | `OrderType` enum has no behaviour — no price modifiers, SLA rules, or discount tiers differ between types | `OrderType.cs`, `CreateOrderHandler` | Medium | | |
| BL-15 | No listing endpoints — no way to list orders by user/status or browse products | `OrdersController`, `ProductsController` | Medium | | |
| BL-16 | Migration is never applied — `Program.cs` resolves the `DbContext` but never calls `Database.Migrate()` | `Program.cs` | Medium | 2026-06-16 | https://github.com/FPDPanda/orders-api-itau/pull/2 |

---

### Architecture & Code Quality

| # | Description | Affected Files | Severity | Fixed On | PR |
|---|-------------|---------------|----------|----------|----|
| AC-01 | Controllers return raw domain entities — no DTOs, exposes DB schema directly and makes contract changes breaking | All controllers and handlers | High | | |
| AC-02 | `CreateOrderRequest` and `CreateProductRequest` are defined inside the controller files, not in their own files | `OrdersController.cs`, `ProductsController.cs` | Medium | | |
| AC-03 | `UpdateProduct` reuses `CreateProductRequest` — semantically wrong, a create and update request are different contracts | `ProductsController.cs` | Medium | | |
| AC-04 | All commands live under the `Queries` namespace and folder — commands and queries should be separated | `OrdersApi/Queries/` | Medium | | |
| AC-05 | `PUT /orders/{id}/items/{itemId}` should be `POST` — `PUT` implies full replacement, not appending to a collection | `OrdersController.cs` | Medium | | |
| AC-06 | Swagger UI is enabled for all environments — should be restricted to `Development` | `Program.cs` | Medium | | |
| AC-07 | `CancellationToken` is accepted by all handlers but never forwarded to repository or EF Core calls | All handlers, all repository methods | Low | | |
| AC-08 | No global exception handling middleware — unhandled exceptions return stack traces in the response | `Program.cs` | Low | | |
| AC-09 | No structured logging — no `ILogger` usage anywhere in handlers or repositories | All handlers, all repositories | Low | | |
