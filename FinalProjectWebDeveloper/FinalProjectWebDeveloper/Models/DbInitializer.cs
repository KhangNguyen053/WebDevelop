using Microsoft.EntityFrameworkCore;
using FinalProjectWebDeveloper.Models;
using System;
using System.Linq;

namespace FinalProjectWebDeveloper.Models;

public static class DbInitializer
{
    public static void Seed(CoffeeShopDbContext context)
    {
        // Ensure database is created
        context.Database.EnsureCreated();

        // Check if database has already been seeded
        if (context.Roles.Any())
        {
            return; 
        }

        using var transaction = context.Database.BeginTransaction();
        try
        {
            // Seed Roles
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Role] ON");
            context.Roles.AddRange(
                new Role { RoleID = 1, RoleName = "Admin" },
                new Role { RoleID = 2, RoleName = "User" }
            );
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Role] OFF");

            // Seed Accounts
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Account] ON");
            context.Accounts.AddRange(
                new Account { AccountID = 1, Username = "admin", PasswordHash = "123456", RoleID = 1, IsActive = true },
                new Account { AccountID = 2, Username = "user01", PasswordHash = "123456", RoleID = 2, IsActive = true },
                new Account { AccountID = 3, Username = "user02", PasswordHash = "123456", RoleID = 2, IsActive = true }
            );
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Account] OFF");

            // Seed Customers
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Customer] ON");
            context.Customers.AddRange(
                new Customer { CustomerID = 1, FullName = "Nguyễn Văn Đình An", Email = "nguyenvanan@gmail.com", Phone = "0901111111", Address = "TP Hồ Chí Minh", AccountID = 2 },
                new Customer { CustomerID = 2, FullName = "Trần Thị Bích", Email = "tranthibich@gmail.com", Phone = "0902222222", Address = "Bình Dương", AccountID = 3 }
            );
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Customer] OFF");

            // Seed Categories
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Category] ON");
            context.Categories.AddRange(
                new Category { CategoryID = 1, CategoryName = "Gà Rán" },
                new Category { CategoryID = 2, CategoryName = "Hamburger" },
                new Category { CategoryID = 3, CategoryName = "Pizza" },
                new Category { CategoryID = 4, CategoryName = "Đồ Ăn Khác" },
                new Category { CategoryID = 5, CategoryName = "Nước Uống" }
            );
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Category] OFF");

            // Seed Products
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Product] ON");
            context.Products.AddRange(
                new Product { ProductID = 1, ProductName = "Gà Rán Chiên Giòn", ImageURL = "Id 1.png", CategoryID = 1, IsAvailable = true },
                new Product { ProductID = 2, ProductName = "Gà Viên Lắc Phô Mai", ImageURL = "Id 2.png", CategoryID = 1, IsAvailable = true },
                new Product { ProductID = 3, ProductName = "Gà Sốt Cay", ImageURL = "Id 3.png", CategoryID = 1, IsAvailable = true },
                new Product { ProductID = 4, ProductName = "Gà Sốt BBQ", ImageURL = "Id 4.png", CategoryID = 1, IsAvailable = true },
                new Product { ProductID = 5, ProductName = "Gà Sốt Chua Cay", ImageURL = "Id 5.png", CategoryID = 1, IsAvailable = true },
                
                new Product { ProductID = 6, ProductName = "Hamburger Bò", ImageURL = "Id 6.png", CategoryID = 2, IsAvailable = true },
                new Product { ProductID = 7, ProductName = "Hamburger Tôm", ImageURL = "Id 7.png", CategoryID = 2, IsAvailable = true },
                new Product { ProductID = 8, ProductName = "Hamburger Gà", ImageURL = "Id 8.png", CategoryID = 2, IsAvailable = true },
                new Product { ProductID = 9, ProductName = "Hamburger Cá", ImageURL = "Id 9.png", CategoryID = 2, IsAvailable = true },
                new Product { ProductID = 10, ProductName = "Hamburger Chay", ImageURL = "Id 10.png", CategoryID = 2, IsAvailable = true },
                new Product { ProductID = 11, ProductName = "Hamburger Heo", ImageURL = "Id 11.png", CategoryID = 2, IsAvailable = true },

                new Product { ProductID = 12, ProductName = "Pizza Phô Mai", ImageURL = "Id 12.png", CategoryID = 3, IsAvailable = true },
                new Product { ProductID = 13, ProductName = "Pizza Xúc Xích Thịt Xông Khói", ImageURL = "Id 13.png", CategoryID = 3, IsAvailable = true },
                new Product { ProductID = 14, ProductName = "Pizza Hải Sản", ImageURL = "Id 14.png", CategoryID = 3, IsAvailable = true },
                new Product { ProductID = 15, ProductName = "Pizza Xúc Xích Thanh Cua", ImageURL = "Id 15.png", CategoryID = 3, IsAvailable = true },
                new Product { ProductID = 16, ProductName = "Pizza Thập Cẩm", ImageURL = "Id 16.png", CategoryID = 3, IsAvailable = true },
                new Product { ProductID = 17, ProductName = "Pizza Thanh Đạm", ImageURL = "Id 17.png", CategoryID = 3, IsAvailable = true },

                new Product { ProductID = 18, ProductName = "Khoai Tây Chiên", ImageURL = "Id 18.jpg", CategoryID = 4, IsAvailable = true },
                new Product { ProductID = 19, ProductName = "Khoai Tây Lắc Phô Mai", ImageURL = "Id 19.jpg", CategoryID = 4, IsAvailable = true },
                new Product { ProductID = 20, ProductName = "Rau củ trộn", ImageURL = "Id 20.jpg", CategoryID = 4, IsAvailable = true },
                new Product { ProductID = 21, ProductName = "Bánh Táo", ImageURL = "Id 21.jpg", CategoryID = 4, IsAvailable = true },
                new Product { ProductID = 22, ProductName = "Kem Ốc Quế", ImageURL = "Id 22.jpg", CategoryID = 4, IsAvailable = true },
                new Product { ProductID = 23, ProductName = "Kem Ly", ImageURL = "Id 23.jpg", CategoryID = 4, IsAvailable = true },

                new Product { ProductID = 24, ProductName = "Pepsi", ImageURL = "Id 24.jpg", CategoryID = 5, IsAvailable = true },
                new Product { ProductID = 25, ProductName = "Coca", ImageURL = "Id 25.jpg", CategoryID = 5, IsAvailable = true },
                new Product { ProductID = 26, ProductName = "7Up", ImageURL = "Id 26.jpg", CategoryID = 5, IsAvailable = true },
                new Product { ProductID = 27, ProductName = "Milo", ImageURL = "Id 27.jpg", CategoryID = 5, IsAvailable = true },
                new Product { ProductID = 28, ProductName = "Fanta Cam", ImageURL = "Id 28.jpg", CategoryID = 5, IsAvailable = true },
                new Product { ProductID = 29, ProductName = "Mirinda Soda Kem", ImageURL = "Id 29.jpg", CategoryID = 5, IsAvailable = true },
                new Product { ProductID = 30, ProductName = "Nước Suối", ImageURL = "Id 30.jpg", CategoryID = 5, IsAvailable = true }
            );
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Product] OFF");

            // Seed ProductSizes
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [ProductSize] ON");
            context.ProductSizes.AddRange(
                new ProductSize { ProductSizeID = 1, ProductID = 1, SizeName = "1 miếng", Price = 25000 },
                new ProductSize { ProductSizeID = 2, ProductID = 2, SizeName = "1 phần", Price = 30000 },
                new ProductSize { ProductSizeID = 3, ProductID = 3, SizeName = "1 miếng", Price = 30000 },
                new ProductSize { ProductSizeID = 4, ProductID = 4, SizeName = "1 miếng", Price = 30000 },
                new ProductSize { ProductSizeID = 5, ProductID = 5, SizeName = "1 miếng", Price = 30000 },

                new ProductSize { ProductSizeID = 6, ProductID = 6, SizeName = "1 cái", Price = 29000 },
                new ProductSize { ProductSizeID = 7, ProductID = 7, SizeName = "1 cái", Price = 32000 },
                new ProductSize { ProductSizeID = 8, ProductID = 8, SizeName = "1 cái", Price = 35000 },
                new ProductSize { ProductSizeID = 9, ProductID = 9, SizeName = "1 cái", Price = 35000 },
                new ProductSize { ProductSizeID = 10, ProductID = 10, SizeName = "1 cái", Price = 25000 },
                new ProductSize { ProductSizeID = 11, ProductID = 11, SizeName = "1 cái", Price = 35000 },

                new ProductSize { ProductSizeID = 12, ProductID = 12, SizeName = "Size M", Price = 50000 },
                new ProductSize { ProductSizeID = 13, ProductID = 12, SizeName = "Size L", Price = 100000 },
                new ProductSize { ProductSizeID = 14, ProductID = 13, SizeName = "Size M", Price = 75000 },
                new ProductSize { ProductSizeID = 15, ProductID = 13, SizeName = "Size L", Price = 150000 },
                new ProductSize { ProductSizeID = 16, ProductID = 14, SizeName = "Size M", Price = 90000 },
                new ProductSize { ProductSizeID = 17, ProductID = 14, SizeName = "Size L", Price = 175000 },
                new ProductSize { ProductSizeID = 18, ProductID = 15, SizeName = "Size M", Price = 70000 },
                new ProductSize { ProductSizeID = 19, ProductID = 15, SizeName = "Size L", Price = 140000 },
                new ProductSize { ProductSizeID = 20, ProductID = 16, SizeName = "Size M", Price = 100000 },
                new ProductSize { ProductSizeID = 21, ProductID = 16, SizeName = "Size L", Price = 200000 },
                new ProductSize { ProductSizeID = 22, ProductID = 17, SizeName = "Size M", Price = 50000 },
                new ProductSize { ProductSizeID = 23, ProductID = 17, SizeName = "Size L", Price = 100000 },

                new ProductSize { ProductSizeID = 24, ProductID = 18, SizeName = "1 phần", Price = 30000 },
                new ProductSize { ProductSizeID = 25, ProductID = 19, SizeName = "1 phần", Price = 35000 },
                new ProductSize { ProductSizeID = 26, ProductID = 20, SizeName = "1 phần", Price = 30000 },
                new ProductSize { ProductSizeID = 27, ProductID = 21, SizeName = "1 cái", Price = 25000 },
                new ProductSize { ProductSizeID = 28, ProductID = 22, SizeName = "1 cây", Price = 15000 },
                new ProductSize { ProductSizeID = 29, ProductID = 23, SizeName = "1 ly", Price = 25000 },

                new ProductSize { ProductSizeID = 30, ProductID = 24, SizeName = "Lon", Price = 15000 },
                new ProductSize { ProductSizeID = 31, ProductID = 24, SizeName = "Combo 2 lon", Price = 28000 },
                new ProductSize { ProductSizeID = 32, ProductID = 25, SizeName = "Lon", Price = 15000 },
                new ProductSize { ProductSizeID = 33, ProductID = 25, SizeName = "Combo 2 lon", Price = 28000 },
                new ProductSize { ProductSizeID = 34, ProductID = 26, SizeName = "Lon", Price = 15000 },
                new ProductSize { ProductSizeID = 35, ProductID = 26, SizeName = "Combo 2 lon", Price = 28000 },
                new ProductSize { ProductSizeID = 36, ProductID = 27, SizeName = "Hộp", Price = 18000 },
                new ProductSize { ProductSizeID = 37, ProductID = 27, SizeName = "Lốc 4 hộp", Price = 70000 },
                new ProductSize { ProductSizeID = 38, ProductID = 28, SizeName = "Lon", Price = 15000 },
                new ProductSize { ProductSizeID = 39, ProductID = 28, SizeName = "Combo 2 lon", Price = 28000 },
                new ProductSize { ProductSizeID = 40, ProductID = 29, SizeName = "Lon", Price = 15000 },
                new ProductSize { ProductSizeID = 41, ProductID = 29, SizeName = "Combo 2 lon", Price = 28000 },
                new ProductSize { ProductSizeID = 42, ProductID = 30, SizeName = "500ml", Price = 10000 },
                new ProductSize { ProductSizeID = 43, ProductID = 30, SizeName = "1.5L", Price = 18000 }
            );
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [ProductSize] OFF");

            // Seed Vouchers
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Voucher] ON");
            context.Vouchers.AddRange(
                new Voucher { VoucherID = 1, VoucherCode = "WELCOME10", DiscountPercent = 10, StartDate = new DateTime(2026, 1, 1), EndDate = new DateTime(2026, 12, 31), IsActive = true },
                new Voucher { VoucherID = 2, VoucherCode = "SALE20", DiscountPercent = 20, StartDate = new DateTime(2026, 1, 1), EndDate = new DateTime(2026, 12, 31), IsActive = true },
                new Voucher { VoucherID = 3, VoucherCode = "VIP30", DiscountPercent = 30, StartDate = new DateTime(2026, 1, 1), EndDate = new DateTime(2026, 12, 31), IsActive = true }
            );
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Voucher] OFF");

            // Seed Reviews
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Review] ON");
            context.Reviews.AddRange(
                new Review { ReviewID = 1, CustomerID = 1, ProductID = 1, Rating = 5, Comment = "Gà rất giòn và ngon", ReviewDate = new DateTime(2026, 6, 25) },
                new Review { ReviewID = 2, CustomerID = 1, ProductID = 6, Rating = 5, Comment = "Hamburger bò rất thơm", ReviewDate = new DateTime(2026, 6, 25) },
                new Review { ReviewID = 3, CustomerID = 2, ProductID = 18, Rating = 4, Comment = "Khoai tây chiên giòn, giá hợp lý", ReviewDate = new DateTime(2026, 6, 24) }
            );
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Review] OFF");

            // Seed OrderHeaders
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [OrderHeader] ON");
            context.OrderHeaders.AddRange(
                new OrderHeader { OrderID = 1, CustomerID = 1, VoucherID = 1, OrderDate = new DateTime(2026, 6, 25, 10, 30, 0), TotalAmount = 70000, Status = "Đã thanh toán" },
                new OrderHeader { OrderID = 2, CustomerID = 2, VoucherID = null, OrderDate = new DateTime(2026, 6, 25, 15, 45, 0), TotalAmount = 105000, Status = "Đã thanh toán" },
                new OrderHeader { OrderID = 3, CustomerID = 1, VoucherID = null, OrderDate = new DateTime(2026, 6, 24, 9, 15, 0), TotalAmount = 69000, Status = "Đã thanh toán" },
                new OrderHeader { OrderID = 4, CustomerID = 2, VoucherID = 2, OrderDate = new DateTime(2026, 6, 23, 14, 20, 0), TotalAmount = 200000, Status = "Đã thanh toán" },
                new OrderHeader { OrderID = 5, CustomerID = 1, VoucherID = null, OrderDate = new DateTime(2026, 6, 22, 17, 0, 0), TotalAmount = 45000, Status = "Đã thanh toán" },
                new OrderHeader { OrderID = 6, CustomerID = 2, VoucherID = null, OrderDate = new DateTime(2026, 6, 21, 11, 10, 0), TotalAmount = 65000, Status = "Đã thanh toán" },
                new OrderHeader { OrderID = 7, CustomerID = 1, VoucherID = 3, OrderDate = new DateTime(2026, 6, 20, 16, 30, 0), TotalAmount = 175000, Status = "Đã thanh toán" },
                new OrderHeader { OrderID = 8, CustomerID = 2, VoucherID = null, OrderDate = new DateTime(2026, 6, 19, 13, 0, 0), TotalAmount = 77000, Status = "Đã thanh toán" },
                new OrderHeader { OrderID = 9, CustomerID = 1, VoucherID = null, OrderDate = new DateTime(2026, 6, 25, 18, 0, 0), TotalAmount = 50000, Status = "Mới" }
            );
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [OrderHeader] OFF");

            // Seed Payments
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Payment] ON");
            context.Payments.AddRange(
                new Payment { PaymentID = 1, OrderID = 1, PaymentMethod = "MoMo", PaymentDate = new DateTime(2026, 6, 25, 10, 31, 0), Amount = 70000 },
                new Payment { PaymentID = 2, OrderID = 2, PaymentMethod = "Tiền mặt", PaymentDate = new DateTime(2026, 6, 25, 15, 46, 0), Amount = 105000 },
                new Payment { PaymentID = 3, OrderID = 3, PaymentMethod = "Chuyển khoản", PaymentDate = new DateTime(2026, 6, 24, 9, 16, 0), Amount = 69000 },
                new Payment { PaymentID = 4, OrderID = 4, PaymentMethod = "MoMo", PaymentDate = new DateTime(2026, 6, 23, 14, 21, 0), Amount = 200000 },
                new Payment { PaymentID = 5, OrderID = 5, PaymentMethod = "Tiền mặt", PaymentDate = new DateTime(2026, 6, 22, 17, 0, 0), Amount = 45000 },
                new Payment { PaymentID = 6, OrderID = 6, PaymentMethod = "MoMo", PaymentDate = new DateTime(2026, 6, 21, 11, 11, 0), Amount = 65000 },
                new Payment { PaymentID = 7, OrderID = 7, PaymentMethod = "Tiền mặt", PaymentDate = new DateTime(2026, 6, 20, 16, 31, 0), Amount = 175000 },
                new Payment { PaymentID = 8, OrderID = 8, PaymentMethod = "Chuyển khoản", PaymentDate = new DateTime(2026, 6, 19, 13, 1, 0), Amount = 77000 }
            );
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [Payment] OFF");

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
            throw;
        }
    }
}
