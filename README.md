# orders-api-itau

A simple .NET 8 Orders API with PostgreSQL (Docker) and MediatR.

## Project Structure

```
OrdersApi/
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
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Docker](https://www.docker.com/get-started)

## Getting Started

### 1. Start PostgreSQL with Docker

```bash
docker compose up -d
```

### 2. Run the API

```bash
cd OrdersApi
dotnet run
```

The API will be available at `http://localhost:5000`.

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
