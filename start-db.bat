@echo off
docker run --name orders-postgres ^
  -e POSTGRES_USER=postgres ^
  -e POSTGRES_PASSWORD=postgres ^
  -e POSTGRES_DB=ordersdb ^
  -p 5432:5432 ^
  -d postgres:16
