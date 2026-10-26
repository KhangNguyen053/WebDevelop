using Microsoft.EntityFrameworkCore;
using FinalProjectWebDeveloper.Models;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();


// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DbContext
builder.Services.AddDbContext<CoffeeShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Session Services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Apply any pending database migrations automatically and seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CoffeeShopDbContext>();
        
        // Check if Category table already exists (which means DB is already created by SQL script)
        // If so, we can ensure the __EFMigrationsHistory table exists and contains the migration record
        // so that context.Database.Migrate() doesn't try to recreate tables.
        bool categoryExists = false;
        try
        {
            context.Database.OpenConnection();
            using (var cmd = context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT OBJECT_ID(N'[Category]')";
                var res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value)
                {
                    categoryExists = true;
                }
            }
        }
        catch { }
        finally
        {
            context.Database.CloseConnection();
        }

        if (categoryExists)
        {
            // Ensure __EFMigrationsHistory exists and has the record
            context.Database.ExecuteSqlRaw(@"
                IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
                BEGIN
                    CREATE TABLE [__EFMigrationsHistory] (
                        [MigrationId] nvarchar(150) NOT NULL,
                        [ProductVersion] nvarchar(32) NOT NULL,
                        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                    );
                END;
            ");
            
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260629014800_InitialCreate')
                BEGIN
                    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                    VALUES ('20260629014800_InitialCreate', '10.0.9');
                END;
            ");
        }

        context.Database.Migrate();
        DbInitializer.Seed(context);

        // Fix encoding/spelling issues in database (mi?ng -> miếng, ph?n -> phần, c?i -> cái, n??c -> nước)
        try
        {
            context.Database.ExecuteSqlRaw("UPDATE [ProductSize] SET [SizeName] = N'1 miếng' WHERE [SizeName] LIKE '%mi%ng%' OR [SizeName] LIKE '%mi?ng%' OR [SizeName] = '1 mi?ng'");
            context.Database.ExecuteSqlRaw("UPDATE [ProductSize] SET [SizeName] = N'1 phần' WHERE [SizeName] LIKE '%ph%n%' OR [SizeName] LIKE '%ph?n%' OR [SizeName] = '1 ph?n'");
            context.Database.ExecuteSqlRaw("UPDATE [ProductSize] SET [SizeName] = N'1 cái' WHERE [SizeName] LIKE '%c%i%' OR [SizeName] LIKE '%c?i%' OR [SizeName] = '1 c?i'");
            context.Database.ExecuteSqlRaw("UPDATE [ProductSize] SET [SizeName] = N'1 cây' WHERE [SizeName] LIKE '%c%y%' OR [SizeName] LIKE '%c?y%' OR [SizeName] = '1 c?y'");
            context.Database.ExecuteSqlRaw("UPDATE [ProductSize] SET [SizeName] = N'1 ly' WHERE [SizeName] LIKE '%ly%'");
            context.Database.ExecuteSqlRaw("UPDATE [ProductSize] SET [SizeName] = N'Nước Suối 500ml' WHERE [SizeName] = '500ml'");
            context.Database.ExecuteSqlRaw("UPDATE [ProductSize] SET [SizeName] = N'Nước Suối 1.5L' WHERE [SizeName] = '1.5L'");

            context.Database.ExecuteSqlRaw("UPDATE [Category] SET [CategoryName] = N'Gà Rán' WHERE [CategoryID] = 1");
            context.Database.ExecuteSqlRaw("UPDATE [Category] SET [CategoryName] = N'Hamburger' WHERE [CategoryID] = 2");
            context.Database.ExecuteSqlRaw("UPDATE [Category] SET [CategoryName] = N'Pizza' WHERE [CategoryID] = 3");
            context.Database.ExecuteSqlRaw("UPDATE [Category] SET [CategoryName] = N'Đồ Ăn Khác' WHERE [CategoryID] = 4");
            context.Database.ExecuteSqlRaw("UPDATE [Category] SET [CategoryName] = N'Nước Uống' WHERE [CategoryID] = 5");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error repairing database spelling/encoding: {ex.Message}");
        }

        // Update product images extension to match actual physical assets (.png) if they were inserted as .jpg
        try
        {
            context.Database.ExecuteSqlRaw("UPDATE [Product] SET [ImageURL] = REPLACE([ImageURL], '.jpg', '.png') WHERE [ImageURL] LIKE '%.jpg'");
        }
        catch { }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while migrating or seeding the database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession(); // Enable session

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
