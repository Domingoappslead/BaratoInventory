# **Barato Inventory**

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

## 📦 **API Endpoints**

| Method | Endpoint | Description |
| :---: | :--- | :--- |
| **GET** | `/api/products` | Get all products |
| **GET** | `/api/products/{id}` | Get product by ID |
| **GET** | `/api/products/search?term=` | Search products |
| **POST** | `/api/products` | Create product |
| **PUT** | `/api/products/{id}` | Update product |
| **DELETE** | `/api/products/{id}` | Delete product |

## 🚀 Running the Project Locally (Docker)

This project is configured to run easily using **Docker Compose**. Follow these steps to set up and run the application using Visual Studio:

### **Prerequisites**

* **Docker Desktop** (running)
* **Visual Studio** (2019 or newer, with **.NET and Docker workloads** installed)

### **Setup Instructions**

1.  **Clone the Repository**

2.  **Open the Solution:**
    * Double-click the solution file (`.sln`) to open the project in Visual Studio.

3.  **Set Docker Compose as Startup Project:**
    * In the **Solution Explorer**, right-click the `docker-compose` project (usually named `docker-compose` or `docker-compose.dcproj`).
    * Select **Set as Startup Project**.

4.  **Build and Run:**
    * Click the **Docker Compose Play Button** (green triangle) in the Visual Studio toolbar.
    * *Alternatively,* you can use the menu: **Debug** > **Start Debugging** (or press **F5**).

    > **Note:** The first time you run this, Visual Studio will **build the Docker images** and start the containers, which may take a few minutes.

5.  **Access the Application:**
    * The application will automatically launch in your browser
