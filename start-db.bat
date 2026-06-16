@echo off
docker run --name orders-postgres ^
  -e POSTGRES_USER=postgres ^
  -e POSTGRES_PASSWORD=postgres ^
  -e POSTGRES_DB=ordersdb ^
  -p 5432:5432 ^
  -d postgres:16

echo Waiting for PostgreSQL to be ready...
:wait
docker exec orders-postgres pg_isready -U postgres >nul 2>&1
if errorlevel 1 (
  timeout /t 1 /nobreak >nul
  goto wait
)

echo Running migrations...
docker exec -i orders-postgres psql -U postgres -d ordersdb < src\OrdersApi\Data\Migrations\V1__InitialCreate.sql
docker exec -i orders-postgres psql -U postgres -d ordersdb < src\OrdersApi\Data\Migrations\V2__AddDiscounts.sql
echo Done.
