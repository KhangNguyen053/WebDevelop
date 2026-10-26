using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalProjectWebDeveloper.Models;

namespace FinalProjectWebDeveloper.Controllers;

public class AccountController : Controller
{
    private readonly CoffeeShopDbContext _context;

    public AccountController(CoffeeShopDbContext context)
    {
        _context = context;
    }

    // GET: Account/Register
    public IActionResult Register()
    {
        return View();
    }

    // POST: Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string username, string password, string fullName, string email, string phone, string address)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Tên đăng nhập và mật khẩu không được để trống.");
            return View();
        }

        // Kiểm tra xem tên đăng nhập đã tồn tại chưa
        var existingAccount = await _context.Accounts.AnyAsync(a => a.Username == username);
        if (existingAccount)
        {
            ModelState.AddModelError("", "Tên đăng nhập đã tồn tại.");
            return View();
        }

        try
        {
            // Tạo Account mới (Mặc định RoleID = 2 là User)
            var newAccount = new Account
            {
                Username = username,
                PasswordHash = password, // Lưu dưới dạng text trơn để đồng bộ dữ liệu mẫu SQL
                RoleID = 2,
                IsActive = true
            };
            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            // Tạo Customer tương ứng
            var newCustomer = new Customer
            {
                FullName = fullName,
                Email = email,
                Phone = phone,
                Address = address,
                AccountID = newAccount.AccountID
            };
            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Đã có lỗi xảy ra trong quá trình đăng ký: " + ex.Message);
            return View();
        }
    }

    // GET: Account/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Tên đăng nhập và mật khẩu không được để trống.");
            return View();
        }

        var account = await _context.Accounts
            .Include(a => a.Role)
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Username == username && a.PasswordHash == password);

        if (account == null || account.IsActive == false)
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng, hoặc tài khoản đã bị khóa.");
            return View();
        }

        // Lưu thông tin đăng nhập vào Session
        HttpContext.Session.SetInt32("AccountId", account.AccountID);
        HttpContext.Session.SetString("Username", account.Username);
        HttpContext.Session.SetInt32("RoleId", account.RoleID);
        HttpContext.Session.SetString("RoleName", account.Role?.RoleName ?? "User");

        if (account.RoleID == 1) // Admin
        {
            HttpContext.Session.SetString("FullName", "Quản trị viên");
            return RedirectToAction("Index", "Admin");
        }
        else // Khách hàng
        {
            if (account.Customer != null)
            {
                HttpContext.Session.SetInt32("CustomerId", account.Customer.CustomerID);
                HttpContext.Session.SetString("FullName", account.Customer.FullName ?? account.Username);
                
                // Đồng bộ giỏ hàng từ Session vào Database
                await CartController.MigrateCartAsync(HttpContext, _context, account.Customer.CustomerID);
            }
            else
            {
                HttpContext.Session.SetString("FullName", account.Username);
            }
            return RedirectToAction("Index", "Home");
        }
    }

    // GET: Account/Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
