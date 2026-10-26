using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalProjectWebDeveloper.Models;

namespace FinalProjectWebDeveloper.Controllers;

public class ProductController : Controller
{
    private readonly CoffeeShopDbContext _context;

    public ProductController(CoffeeShopDbContext context)
    {
        _context = context;
    }

    // GET: Product
    public async Task<IActionResult> Index(int? categoryId, string? search, string? sortBy, int page = 1)
    {
        int pageSize = 9;
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductSizes)
            .Where(p => p.IsAvailable == true);

        // 1. Tìm kiếm theo tên
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.ProductName.Contains(search));
        }

        // 2. Lọc theo loại sản phẩm (Category)
        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(p => p.CategoryID == categoryId.Value);
        }

        // 3. Sắp xếp theo giá (dựa trên giá nhỏ nhất của các size)
        if (sortBy == "price_asc")
        {
            query = query.OrderBy(p => p.ProductSizes.Min(ps => (decimal?)ps.Price) ?? 0);
        }
        else if (sortBy == "price_desc")
        {
            query = query.OrderByDescending(p => p.ProductSizes.Min(ps => (decimal?)ps.Price) ?? 0);
        }
        else
        {
            // Sắp xếp mặc định
            query = query.OrderBy(p => p.ProductID);
        }

        // Phân trang
        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        if (page < 1) page = 1;
        if (totalPages > 0 && page > totalPages) page = totalPages;

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Nạp danh sách các Category để hiển thị ở Sidebar
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.CurrentCategory = categoryId;
        ViewBag.SearchTerm = search;
        ViewBag.SortBy = sortBy;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(products);
    }

    // GET: Product/Detail/5
    public async Task<IActionResult> Detail(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductSizes)
            .Include(p => p.Reviews)
                .ThenInclude(r => r.Customer)
            .FirstOrDefaultAsync(p => p.ProductID == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }
}
