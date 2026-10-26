using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using FinalProjectWebDeveloper.Models;

namespace FinalProjectWebDeveloper.Controllers;

public class AdminProductsController : Controller
{
    private readonly CoffeeShopDbContext _context;
    private readonly IWebHostEnvironment _env;

    public AdminProductsController(CoffeeShopDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // Bảo vệ quyền Admin
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var roleId = context.HttpContext.Session.GetInt32("RoleId");
        if (roleId != 1)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
        }
        base.OnActionExecuting(context);
    }

    // GET: AdminProducts
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductSizes)
            .ToListAsync();
        return View(products);
    }

    // GET: AdminProducts/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View();
    }

    // POST: AdminProducts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product, IFormFile? imageFile, decimal? priceS, decimal? priceM, decimal? priceL)
    {
        // 1. Xử lý tải ảnh lên
        if (imageFile != null && imageFile.Length > 0)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            string uploadPath = Path.Combine(_env.WebRootPath, "images", fileName);
            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            product.ImageURL = fileName;
        }
        else
        {
            product.ImageURL = "caphesua.jpg"; // Ảnh mặc định nếu không tải lên
        }

        product.IsAvailable = true;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // 2. Thêm ProductSizes tương ứng
        if (priceS.HasValue && priceS.Value > 0)
        {
            _context.ProductSizes.Add(new ProductSize { ProductID = product.ProductID, SizeName = "360ml", Price = priceS.Value });
        }
        if (priceM.HasValue && priceM.Value > 0)
        {
            _context.ProductSizes.Add(new ProductSize { ProductID = product.ProductID, SizeName = "500ml", Price = priceM.Value });
        }
        if (priceL.HasValue && priceL.Value > 0)
        {
            _context.ProductSizes.Add(new ProductSize { ProductID = product.ProductID, SizeName = "700ml", Price = priceL.Value });
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Thêm sản phẩm mới thành công!";
        return RedirectToAction(nameof(Index));
    }

    // GET: AdminProducts/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products
            .Include(p => p.ProductSizes)
            .FirstOrDefaultAsync(p => p.ProductID == id);

        if (product == null)
        {
            return NotFound();
        }

        ViewBag.Categories = await _context.Categories.ToListAsync();

        // Đọc giá các size hiện tại của sản phẩm
        ViewBag.PriceS = product.ProductSizes.FirstOrDefault(s => s.SizeName == "360ml")?.Price;
        ViewBag.PriceM = product.ProductSizes.FirstOrDefault(s => s.SizeName == "500ml")?.Price;
        ViewBag.PriceL = product.ProductSizes.FirstOrDefault(s => s.SizeName == "700ml")?.Price;

        return View(product);
    }

    // POST: AdminProducts/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product updatedProduct, IFormFile? imageFile, decimal? priceS, decimal? priceM, decimal? priceL)
    {
        var product = await _context.Products
            .Include(p => p.ProductSizes)
            .FirstOrDefaultAsync(p => p.ProductID == id);

        if (product == null)
        {
            return NotFound();
        }

        // Cập nhật các thông tin cơ bản
        product.ProductName = updatedProduct.ProductName;
        product.CategoryID = updatedProduct.CategoryID;
        product.IsAvailable = updatedProduct.IsAvailable;

        // Xử lý ảnh tải lên mới
        if (imageFile != null && imageFile.Length > 0)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            string uploadPath = Path.Combine(_env.WebRootPath, "images", fileName);
            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            product.ImageURL = fileName;
        }

        // Cập nhật ProductSizes
        UpdateSizePrice(product, "360ml", priceS);
        UpdateSizePrice(product, "500ml", priceM);
        UpdateSizePrice(product, "700ml", priceL);

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
        return RedirectToAction(nameof(Index));
    }

    // POST: AdminProducts/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products
            .Include(p => p.ProductSizes)
            .FirstOrDefaultAsync(p => p.ProductID == id);

        if (product == null)
        {
            return NotFound();
        }

        try
        {
            // Kiểm tra xem sản phẩm đã có trong hóa đơn nào chưa
            bool hasOrders = await _context.OrderDetails
                .AnyAsync(od => od.ProductSize != null && od.ProductSize.ProductID == id);

            if (hasOrders)
            {
                // Nếu đã có hóa đơn -> Soft delete (Ẩn sản phẩm khỏi thực đơn)
                product.IsAvailable = false;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sản phẩm đã có lịch sử đơn hàng. Hệ thống đã ẩn sản phẩm khỏi danh sách bán hàng thay vì xóa hoàn toàn.";
            }
            else
            {
                // Nếu chưa có hóa đơn -> Xóa hoàn toàn
                // Xóa các size trước
                _context.ProductSizes.RemoveRange(product.ProductSizes);
                // Xóa sản phẩm
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Không thể xóa sản phẩm. Lỗi: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private void UpdateSizePrice(Product product, string sizeName, decimal? newPrice)
    {
        var size = product.ProductSizes.FirstOrDefault(s => s.SizeName == sizeName);

        if (newPrice.HasValue && newPrice.Value > 0)
        {
            if (size != null)
            {
                size.Price = newPrice.Value;
            }
            else
            {
                _context.ProductSizes.Add(new ProductSize
                {
                    ProductID = product.ProductID,
                    SizeName = sizeName,
                    Price = newPrice.Value
                });
            }
        }
        else
        {
            // Nếu xóa giá (để trống hoặc 0) thì xóa size này đi
            if (size != null)
            {
                _context.ProductSizes.Remove(size);
            }
        }
    }
}
