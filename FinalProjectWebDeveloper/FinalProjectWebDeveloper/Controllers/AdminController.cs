using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using FinalProjectWebDeveloper.Models;

namespace FinalProjectWebDeveloper.Controllers;

public class AdminController : Controller
{
    private readonly CoffeeShopDbContext _context;

    public AdminController(CoffeeShopDbContext context)
    {
        _context = context;
    }

    // Bộ lọc kiểm tra quyền Admin trước khi thực hiện các Action
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var roleId = context.HttpContext.Session.GetInt32("RoleId");
        if (roleId != 1)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
        }
        base.OnActionExecuting(context);
    }

    // GET: Admin
    public async Task<IActionResult> Index(DateTime? date)
    {
        DateTime selectedDate = date ?? DateTime.Today;

        // Tính doanh thu ngày được chọn (chỉ tính trạng thái Đã thanh toán hoặc Đã vận chuyển)
        decimal dailyRevenue = await _context.OrderHeaders
            .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date == selectedDate.Date 
                        && (o.Status == "Đã thanh toán" || o.Status == "Đã vận chuyển"))
            .SumAsync(o => o.TotalAmount);

        // Tính doanh thu tháng được chọn
        decimal monthlyRevenue = await _context.OrderHeaders
            .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == selectedDate.Year 
                        && o.OrderDate.Value.Month == selectedDate.Month 
                        && (o.Status == "Đã thanh toán" || o.Status == "Đã vận chuyển"))
            .SumAsync(o => o.TotalAmount);

        // Tính doanh thu năm được chọn
        decimal yearlyRevenue = await _context.OrderHeaders
            .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == selectedDate.Year 
                        && (o.Status == "Đã thanh toán" || o.Status == "Đã vận chuyển"))
            .SumAsync(o => o.TotalAmount);

        // Chuẩn bị dữ liệu vẽ biểu đồ cột 7 ngày kể từ ngày được chọn trở về trước
        var chartLabels = new List<string>();
        var chartData = new List<decimal>();

        for (int i = 6; i >= 0; i--)
        {
            var targetDay = selectedDate.AddDays(-i);
            chartLabels.Add(targetDay.ToString("dd/MM"));

            decimal dayRev = await _context.OrderHeaders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date == targetDay.Date 
                            && (o.Status == "Đã thanh toán" || o.Status == "Đã vận chuyển"))
                .SumAsync(o => o.TotalAmount);

            chartData.Add(dayRev);
        }

        ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");
        ViewBag.DailyRevenue = dailyRevenue;
        ViewBag.MonthlyRevenue = monthlyRevenue;
        ViewBag.YearlyRevenue = yearlyRevenue;
        ViewBag.ChartLabels = chartLabels;
        ViewBag.ChartData = chartData;

        // Thống kê tổng số lượng các bảng
        ViewBag.TotalProducts = await _context.Products.CountAsync();
        ViewBag.TotalOrders = await _context.OrderHeaders.CountAsync();
        ViewBag.TotalCustomers = await _context.Customers.CountAsync();

        return View();
    }
}
