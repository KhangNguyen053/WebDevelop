using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using FinalProjectWebDeveloper.Models;

namespace FinalProjectWebDeveloper.Controllers;

public class AdminOrdersController : Controller
{
    private readonly CoffeeShopDbContext _context;

    public AdminOrdersController(CoffeeShopDbContext context)
    {
        _context = context;
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

    // GET: AdminOrders
    public async Task<IActionResult> Index()
    {
        var orders = await _context.OrderHeaders
            .Include(o => o.Customer)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        return View(orders);
    }

    // GET: AdminOrders/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var order = await _context.OrderHeaders
            .Include(o => o.Customer)
            .Include(o => o.Voucher)
            .Include(o => o.Payments)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductSize)
                    .ThenInclude(ps => ps.Product)
            .FirstOrDefaultAsync(o => o.OrderID == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // POST: AdminOrders/UpdateStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var order = await _context.OrderHeaders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        order.Status = status;

        // Nếu trạng thái đổi thành "Đã thanh toán", chúng ta đồng thời cập nhật cả trạng thái thanh toán (Payment) nếu có
        if (status == "Đã thanh toán")
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderID == id);
            if (payment != null)
            {
                payment.PaymentDate = DateTime.Now; // Cập nhật ngày thanh toán thực tế
            }
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đơn hàng #{id} thành '{status}' thành công!";
        return RedirectToAction(nameof(Details), new { id = id });
    }
}
