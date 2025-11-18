# **BaratoInventory -- Product Inventory API**

A clean, modular, and containerized **Product Inventory Management
backend** built with **ASP.NET Core (.NET 8)**, **EF Core**, **SQL
Server**, and **Redis**.\
Implements complete CRUD operations, search, caching, clean
architecture, and full unit test coverage for repository and service
layers.

> **Backend is fully complete.**\
> Blazor frontend will be added later.

------------------------------------------------------------------------

## 🚀 **Tech Stack**

-   **ASP.NET Core Web API (.NET 8)**
-   **Entity Framework Core + SQL Server**
-   **Redis caching**
-   **Clean Architecture**
-   **NUnit + Moq**
-   **Docker + Docker Compose**

------------------------------------------------------------------------

## 🧩 **Features**

### ✔ Product Management

-   Create / Update / Delete\
-   Get all products\
-   Get product by ID\
-   Search (name, category, price)

### ✔ Redis Caching

-   List cache (`products:all`)\
-   Individual product cache (`products:<id>`)\
-   Automatic cache invalidation on CRUD

### ✔ API Design

-   DTO-based input/output\
-   Validation + structured error responses\
-   Logging using `ILogger`\
-   Swagger available in development

### ✔ Testing Coverage

**ProductRepositoryTests** - InMemory DB\
- CRUD operations\
- Search functionality\
- Update error handling\
- Database-level behaviors

**ProductServiceTests** - Cache hit/miss\
- Cache invalidation rules\
- Repository calls mocked via Moq\
- Full service-layer logic tested

------------------------------------------------------------------------

## 📦 **API Endpoints**

  Method   Endpoint                       Description
  -------- ------------------------------ -------------------
  GET      `/api/products`                Get all products
  GET      `/api/products/{id}`           Get product by ID
  GET      `/api/products/search?term=`   Search products
  POST     `/api/products`                Create product
  PUT      `/api/products/{id}`           Update product
  DELETE   `/api/products/{id}`           Delete product

------------------------------------------------------------------------
