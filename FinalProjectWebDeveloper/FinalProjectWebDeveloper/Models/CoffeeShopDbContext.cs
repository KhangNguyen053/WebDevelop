using Microsoft.EntityFrameworkCore;

namespace FinalProjectWebDeveloper.Models;

public class CoffeeShopDbContext : DbContext
{
    public CoffeeShopDbContext(DbContextOptions<CoffeeShopDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Account> Accounts { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductSize> ProductSizes { get; set; }
    public virtual DbSet<Topping> Toppings { get; set; }
    public virtual DbSet<ProductTopping> ProductToppings { get; set; }
    public virtual DbSet<Cart> Carts { get; set; }
    public virtual DbSet<CartItem> CartItems { get; set; }
    public virtual DbSet<Voucher> Vouchers { get; set; }
    public virtual DbSet<OrderHeader> OrderHeaders { get; set; }
    public virtual DbSet<OrderDetail> OrderDetails { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình khóa chính phức hợp cho bảng ProductTopping
        modelBuilder.Entity<ProductTopping>()
            .HasKey(pt => new { pt.ProductID, pt.ToppingID });
    }
}
