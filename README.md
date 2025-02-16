# E-Commerce Application - ASP.NET Core MVC

A modern e-commerce web application built with ASP.NET Core MVC, featuring a robust shopping cart system, product management, and order processing.

## Features

- **Product Management**
  - Browse products with filtering and sorting
  - Product categories
  - Price and stock tracking
  - Admin interface for product management

- **Shopping Cart**
  - Add/remove items
  - Update quantities
  - Persistent cart using session storage
  - Real-time cart updates

- **Checkout Process**
  - Guest checkout
  - Order confirmation
  - Email notifications
  - Address validation

- **Admin Features**
  - Product inventory management
  - Order management
  - Category management
  - Stock level monitoring

## Technologies Used

- **Backend**
  - ASP.NET Core MVC (.NET 9.0)
  - Entity Framework Core
  - PostgreSQL Database

- **Frontend**
  - Bootstrap 5
  - jQuery
  - Bootstrap Icons
  - Toastr notifications

## Prerequisites

- .NET 9.0 SDK
- PostgreSQL
- Visual Studio 2022 or VS Code

## Getting Started

1. **Clone the Repository**
   ```bash
   git clone [repository-url]
   cd ASP.NET-ASSIGN01
   ```

2. **Database Setup**
   - Update the connection string in `appsettings.json`
   - Run Entity Framework migrations:
     ```bash
     dotnet ef database update
     ```

3. **Run the Application**
   ```bash
   dotnet run
   ```
   The application will be available at `https://localhost:5001`

## Project Structure

- `Areas/` - Feature-specific areas (Guest, Admin)
- `Controllers/` - MVC Controllers
- `Models/` - Data models and ViewModels
- `Views/` - Razor views
- `Services/` - Business logic services
- `Data/` - Database context and configurations

## Key Features Implementation

### Shopping Cart
- Session-based cart storage
- Real-time quantity updates
- Stock validation
- Price calculations

### Order Processing
- Guest checkout flow
- Order confirmation
- Transaction management
- Email notifications

### Product Management
- Category organization
- Price management
- Stock tracking
- Image handling

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Bootstrap for the UI framework
- ASP.NET Core team for the excellent framework
- Entity Framework Core for data access
