# Inventory Management System

This project is an **Inventory Management System** designed using **Onion Architecture** in **ASP.NET Core**. The system handles the management of items, suppliers, customers, sales, and purchase orders, using a clean and maintainable architecture with separation of concerns.

## Table of Contents

- [Project Overview](#project-overview)
- [Technologies Used](#technologies-used)
- [Architecture](#architecture)
  - [Domain Layer](#domain-layer)
  - [Application Layer](#application-layer)
  - [Infrastructure Layer](#infrastructure-layer)
  - [Presentation Layer](#presentation-layer)
- [Entities](#entities)
- [Relationships](#relationships)
- [Setup Instructions](#setup-instructions)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Project Overview

The **Inventory Management System** allows businesses to manage their suppliers, customers, items, and orders. The system supports functionalities like managing products, handling orders, and tracking financials with GST calculation. This system uses Onion Architecture, which helps in maintaining a scalable and testable codebase.

## Technologies Used

- **ASP.NET Core** (for the API and MVC components)
- **Entity Framework Core** (for database interaction)
- **SQL Server** (for data storage)
- **JWT Authentication** (for secure API access)
- **AutoMapper** (for object mapping)
- **Swagger** (for API documentation)

## Architecture

### Domain Layer

The **Domain Layer** contains the core business logic of the application. It includes:

- **Entities**: Basic objects representing the business entities, such as `User`, `Item`, `PurchaseOrder`, etc.
- **Interfaces**: Repositories for accessing and manipulating the data.

### Application Layer

The **Application Layer** contains the service interfaces and implementations that handle application logic. It is responsible for:

- **DTOs (Data Transfer Objects)**: To transfer data between layers.
- **Services**: Implement business logic for operations like adding, updating, and deleting records.

### Infrastructure Layer

The **Infrastructure Layer** contains the implementation of data access and external services. This layer provides:

- **EF Core Repositories**: Implementation of data access logic using Entity Framework Core.
- **Database Context**: Sets up and manages connections to the database.

### Presentation Layer

The **Presentation Layer** contains:

- **Controllers**: Handling HTTP requests and responses.
- **Views**: Views to display data (for MVC-based projects) or API responses (for Web API).
- **Swagger**: Provides an interactive UI for API documentation.

## Entities

### 1. **UserTypes**

- **Id**: Unique identifier
- **Name**: Type of user (Admin, Manager, etc.)

### 2. **Users**

- **Id**: Unique identifier
- **FullName**: Name of the user
- **Username**: Login username
- **PasswordHash**: Hashed password
- **UserTypeId**: Foreign Key to `UserTypes`

### 3. **Suppliers**

- **Id**: Unique identifier
- **Name**: Name of the supplier
- **Address**: Address of the supplier
- **Contact**: Contact information

### 4. **Customers**

- **Id**: Unique identifier
- **Name**: Name of the customer
- **Address**: Address of the customer
- **Contact**: Contact information

### 5. **Categories**

- **Id**: Unique identifier
- **Name**: Name of the category (e.g., Electronics, Furniture)
- **Description**: Category description

### 6. **Items**

- **Id**: Unique identifier
- **Name**: Name of the item
- **CategoryId**: Foreign Key to `Categories`
- **GSTPercent**: GST rate applied
- **PurchasePrice**: Purchase price from the supplier
- **SellingPrice**: Selling price to customers

### 7. **PurchaseOrders**

- **Id**: Unique identifier
- **OrderNo**: Order number
- **SupplierId**: Foreign Key to `Suppliers`
- **OrderDate**: Date the order was placed
- **TotalAmount**: Total amount for the purchase order

### 8. **SupplierItems**

- **Id**: Unique identifier
- **PurchaseOrderId**: Foreign Key to `PurchaseOrders`
- **ItemId**: Foreign Key to `Items`
- **SupplierId**: Foreign Key to `Suppliers`
- **GSTAmount**: GST on the item
- **TotalAmount**: Total amount for the item in the order

### 9. **SalesOrders**

- **Id**: Unique identifier
- **OrderNo**: Order number
- **CustomerId**: Foreign Key to `Customers`
- **OrderDate**: Date the order was placed
- **TotalAmount**: Total amount for the sales order

### 10. **CustomerItems**

- **Id**: Unique identifier
- **SalesOrderId**: Foreign Key to `SalesOrders`
- **ItemId**: Foreign Key to `Items`
- **CustomerId**: Foreign Key to `Customers`
- **GSTAmount**: GST on the item
- **TotalAmount**: Total amount for the item in the order

## Relationships

![Database Diagram](PresentationApi/Images/DatabaseImages/ImsDatabaseImage.png)

- **Users → UserTypes**: Many-to-One (A user belongs to a user type)
- **Items → Categories**: Many-to-One (An item belongs to a category)
- **SupplierItems → Suppliers**: Many-to-One (A supplier item belongs to a supplier)
- **CustomerItems → Customers**: Many-to-One (A customer item belongs to a customer)
- **SupplierItems → Items**: One-to-One (A supplier item corresponds to one item)
- **CustomerItems → Items**: One-to-One (A customer item corresponds to one item)
- **SupplierItems → PurchaseOrders**: Many-to-One (A supplier item belongs to a purchase order)
- **CustomerItems → SalesOrders**: Many-to-One (A customer item belongs to a sales order)
- **Users → Suppliers**: One-to-One (A user may be linked to one supplier, if applicable)
- **Users → Customers**: One-to-One (A user may be linked to one customer, if applicable)

## Setup Instructions

1. **Clone the repository**:
   ```bash
   git clone https://github.com/your-username/inventory-management-system.git
   cd inventory-management-system
   
2. **Install dependencies**:
  ```bash
  dotnet restore
   ```
3. **Update connection strings**: In appsettings.json, set up the connection string for your database.

4. **Run the application**:
  ```bash
  dotnet run
  ```
5. **Access the API**: Navigate to http://localhost:5000 (or the configured port).

6. **Swagger UI**: Visit http://localhost:5000/swagger to explore the API endpoints.

## Usage

- Authentication: Use JWT authentication for API access.
- Managing Users: Admin can add, update, and delete users.
- Managing Items: Admin can manage items, categories, and suppliers.
- Managing Orders: Users can create, view, and manage purchase and sales orders.
- Managing Financials: The system calculates GST for purchase and sales items.

## Contributing

1. Fork the repository
2. Create a new branch (git checkout -b feature-name)
3. Make your changes
4. Commit your changes (git commit -m 'Add new feature')
5. Push to the branch (git push origin feature-name)
6. Create a new Pull Request

## License
This project is licensed under the MIT License - see the LICENSE file for details.
 ```bash
You can modify the repository URL and other project-specific details as needed. This README file gives an overview of your project, its structure, and how to set up and use the system.

