using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using FinalProjectWebDeveloper.Models;

namespace FinalProjectWebDeveloper.Controllers;

public class HomeController : Controller
{
    private readonly CoffeeShopDbContext _context;

    public HomeController(CoffeeShopDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Lấy 4 sản phẩm ngẫu nhiên hoặc nổi bật để hiển thị ở trang chủ
        var featuredProducts = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductSizes)
            .Where(p => p.IsAvailable == true)
            .OrderBy(p => p.ProductID)
            .Take(4)
            .ToListAsync();

        return View(featuredProducts);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
