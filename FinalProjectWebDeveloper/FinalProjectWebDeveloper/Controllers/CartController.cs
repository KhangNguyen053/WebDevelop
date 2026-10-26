using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using FinalProjectWebDeveloper.Models;

namespace FinalProjectWebDeveloper.Controllers;

public class CartController : Controller
{
    private readonly CoffeeShopDbContext _context;

    public CartController(CoffeeShopDbContext context)
    {
        _context = context;
    }

    // GET: Cart
    public async Task<IActionResult> Index()
    {
        var cartItems = await GetCartItemsAsync();
        ViewBag.TotalAmount = cartItems.Sum(i => i.TotalPrice);
        return View(cartItems);
    }

    // POST: Cart/AddToCart
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(int productSizeId, int quantity = 1)
    {
        if (quantity < 1) quantity = 1;

        int? customerId = HttpContext.Session.GetInt32("CustomerId");

        if (customerId.HasValue)
        {
            // Đã đăng nhập -> Lưu vào Database
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerID == customerId.Value);

            if (cart == null)
            {
                cart = new Cart { CustomerID = customerId.Value };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartID == cart.CartID && ci.ProductSizeID == productSizeId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartID = cart.CartID,
                    ProductSizeID = productSizeId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }
            await _context.SaveChangesAsync();
        }
        else
        {
            // Chưa đăng nhập -> Lưu vào Session
            var sessionCart = GetSessionCart();
            var item = sessionCart.FirstOrDefault(i => i.ProductSizeID == productSizeId);

            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                var size = await _context.ProductSizes
                    .Include(ps => ps.Product)
                    .FirstOrDefaultAsync(ps => ps.ProductSizeID == productSizeId);

                if (size != null)
                {
                    sessionCart.Add(new CartItemViewModel
                    {
                        ProductSizeID = size.ProductSizeID,
                        ProductID = size.ProductID ?? 0,
                        ProductName = size.Product?.ProductName ?? "Sản phẩm",
                        SizeName = size.SizeName,
                        UnitPrice = size.Price,
                        Quantity = quantity,
                        ImageURL = size.Product?.ImageURL
                    });
                }
            }
            SaveSessionCart(sessionCart);
        }

        TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng thành công!";
        return RedirectToAction(nameof(Index));
    }

    // POST: Cart/UpdateQuantity
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(int productSizeId, int quantity)
    {
        if (quantity < 1) quantity = 1;

        int? customerId = HttpContext.Session.GetInt32("CustomerId");

        if (customerId.HasValue)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.CustomerID == customerId.Value);

            if (cart != null)
            {
                var item = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartID == cart.CartID && ci.ProductSizeID == productSizeId);

                if (item != null)
                {
                    item.Quantity = quantity;
                    await _context.SaveChangesAsync();
                }
            }
        }
        else
        {
            var sessionCart = GetSessionCart();
            var item = sessionCart.FirstOrDefault(i => i.ProductSizeID == productSizeId);
            if (item != null)
            {
                item.Quantity = quantity;
                SaveSessionCart(sessionCart);
            }
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Cart/RemoveFromCart
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromCart(int productSizeId)
    {
        int? customerId = HttpContext.Session.GetInt32("CustomerId");

        if (customerId.HasValue)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.CustomerID == customerId.Value);

            if (cart != null)
            {
                var item = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartID == cart.CartID && ci.ProductSizeID == productSizeId);

                if (item != null)
                {
                    _context.CartItems.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
        }
        else
        {
            var sessionCart = GetSessionCart();
            var item = sessionCart.FirstOrDefault(i => i.ProductSizeID == productSizeId);
            if (item != null)
            {
                sessionCart.Remove(item);
                SaveSessionCart(sessionCart);
            }
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Cart/Checkout
    public async Task<IActionResult> Checkout()
    {
        int? customerId = HttpContext.Session.GetInt32("CustomerId");
        if (!customerId.HasValue)
        {
            TempData["ErrorMessage"] = "Bạn phải đăng nhập để tiến hành thanh toán.";
            return RedirectToAction("Login", "Account");
        }

        var cartItems = await GetCartItemsAsync();
        if (!cartItems.Any())
        {
            TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
            return RedirectToAction("Index", "Product");
        }

        var customer = await _context.Customers.FindAsync(customerId.Value);
        ViewBag.Customer = customer;
        ViewBag.TotalAmount = cartItems.Sum(i => i.TotalPrice);

        return View(cartItems);
    }

    // POST: Cart/Checkout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessCheckout(string fullName, string phone, string address, string paymentMethod, string? voucherCode)
    {
        int? customerId = HttpContext.Session.GetInt32("CustomerId");
        if (!customerId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var cartItems = await GetCartItemsAsync();
        if (!cartItems.Any())
        {
            return RedirectToAction("Index", "Product");
        }

        decimal subtotal = cartItems.Sum(i => i.TotalPrice);
        decimal finalTotal = subtotal;
        Voucher? appliedVoucher = null;

        // Kiểm tra Voucher
        if (!string.IsNullOrWhiteSpace(voucherCode))
        {
            appliedVoucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.VoucherCode == voucherCode && v.IsActive == true 
                                     && v.StartDate <= DateTime.Now && v.EndDate >= DateTime.Now);
            if (appliedVoucher != null)
            {
                decimal discount = subtotal * (appliedVoucher.DiscountPercent ?? 0) / 100;
                finalTotal = subtotal - discount;
            }
        }

        // Bắt đầu Transaction
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Cập nhật thông tin nhận hàng của khách hàng nếu thay đổi
            var customer = await _context.Customers.FindAsync(customerId.Value);
            if (customer != null)
            {
                customer.FullName = fullName;
                customer.Phone = phone;
                customer.Address = address;
                _context.Customers.Update(customer);
            }

            // Tạo OrderHeader
            var order = new OrderHeader
            {
                CustomerID = customerId.Value,
                VoucherID = appliedVoucher?.VoucherID,
                OrderDate = DateTime.Now,
                TotalAmount = finalTotal,
                Status = paymentMethod == "Cash" ? "Mới" : "Đã thanh toán" // Nếu là chuyển khoản/momo coi như paid luôn
            };
            _context.OrderHeaders.Add(order);
            await _context.SaveChangesAsync();

            // Tạo OrderDetails
            foreach (var item in cartItems)
            {
                var detail = new OrderDetail
                {
                    OrderID = order.OrderID,
                    ProductSizeID = item.ProductSizeID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };
                _context.OrderDetails.Add(detail);
            }
            await _context.SaveChangesAsync();

            // Tạo Payment
            var payment = new Payment
            {
                OrderID = order.OrderID,
                PaymentMethod = paymentMethod == "Cash" ? "Tiền mặt (COD)" : (paymentMethod == "MoMo" ? "Ví MoMo" : "Chuyển khoản"),
                PaymentDate = DateTime.Now,
                Amount = finalTotal
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Xóa giỏ hàng
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CustomerID == customerId.Value);
            if (cart != null)
            {
                var dbCartItems = _context.CartItems.Where(ci => ci.CartID == cart.CartID);
                _context.CartItems.RemoveRange(dbCartItems);
                await _context.SaveChangesAsync();
            }

            // Xóa session giỏ hàng nếu có
            HttpContext.Session.Remove("SessionCart");

            await transaction.CommitAsync();

            ViewBag.OrderID = order.OrderID;
            ViewBag.Total = finalTotal;
            return View("OrderSuccess");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            TempData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình đặt hàng. Vui lòng thử lại.";
            return RedirectToAction(nameof(Checkout));
        }
    }

    // AJAX Check Voucher
    [HttpGet]
    public async Task<IActionResult> CheckVoucher(string code)
    {
        var voucher = await _context.Vouchers
            .FirstOrDefaultAsync(v => v.VoucherCode == code && v.IsActive == true 
                                 && v.StartDate <= DateTime.Now && v.EndDate >= DateTime.Now);
        if (voucher == null)
        {
            return Json(new { success = false, message = "Mã voucher không hợp lệ hoặc đã hết hạn." });
        }
        return Json(new { success = true, discountPercent = voucher.DiscountPercent });
    }

    // Helper: Đồng bộ Session Cart vào Database Cart khi đăng nhập
    public static async Task MigrateCartAsync(HttpContext httpContext, CoffeeShopDbContext dbContext, int customerId)
    {
        var sessionCartJson = httpContext.Session.GetString("SessionCart");
        if (string.IsNullOrEmpty(sessionCartJson)) return;

        try
        {
            var sessionCart = JsonSerializer.Deserialize<List<CartItemViewModel>>(sessionCartJson);
            if (sessionCart == null || !sessionCart.Any()) return;

            var cart = await dbContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerID == customerId);

            if (cart == null)
            {
                cart = new Cart { CustomerID = customerId };
                dbContext.Carts.Add(cart);
                await dbContext.SaveChangesAsync();
            }

            foreach (var item in sessionCart)
            {
                var existingItem = await dbContext.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartID == cart.CartID && ci.ProductSizeID == item.ProductSizeID);

                if (existingItem != null)
                {
                    existingItem.Quantity += item.Quantity;
                }
                else
                {
                    dbContext.CartItems.Add(new CartItem
                    {
                        CartID = cart.CartID,
                        ProductSizeID = item.ProductSizeID,
                        Quantity = item.Quantity
                    });
                }
            }
            await dbContext.SaveChangesAsync();
            httpContext.Session.Remove("SessionCart"); // Xóa session cart sau khi đồng bộ
        }
        catch
        {
            // Bỏ qua lỗi đồng bộ
        }
    }

    // Helper: Lấy giỏ hàng hiện tại (cả từ DB hoặc Session)
    private async Task<List<CartItemViewModel>> GetCartItemsAsync()
    {
        int? customerId = HttpContext.Session.GetInt32("CustomerId");

        if (customerId.HasValue)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.CustomerID == customerId.Value);

            if (cart == null) return new List<CartItemViewModel>();

            return await _context.CartItems
                .Where(ci => ci.CartID == cart.CartID)
                .Include(ci => ci.ProductSize)
                    .ThenInclude(ps => ps.Product)
                .Select(ci => new CartItemViewModel
                {
                    CartItemID = ci.CartItemID,
                    ProductSizeID = ci.ProductSizeID ?? 0,
                    ProductID = ci.ProductSize != null ? ci.ProductSize.ProductID ?? 0 : 0,
                    ProductName = ci.ProductSize != null && ci.ProductSize.Product != null ? ci.ProductSize.Product.ProductName : "Sản phẩm",
                    SizeName = ci.ProductSize != null ? ci.ProductSize.SizeName : "",
                    UnitPrice = ci.ProductSize != null ? ci.ProductSize.Price : 0,
                    Quantity = ci.Quantity,
                    ImageURL = ci.ProductSize != null && ci.ProductSize.Product != null ? ci.ProductSize.Product.ImageURL : ""
                }).ToListAsync();
        }
        else
        {
            return GetSessionCart();
        }
    }

    // Helper: Đọc giỏ hàng từ Session
    private List<CartItemViewModel> GetSessionCart()
    {
        var json = HttpContext.Session.GetString("SessionCart");
        if (string.IsNullOrEmpty(json))
        {
            return new List<CartItemViewModel>();
        }
        try
        {
            return JsonSerializer.Deserialize<List<CartItemViewModel>>(json) ?? new List<CartItemViewModel>();
        }
        catch
        {
            return new List<CartItemViewModel>();
        }
    }

    // Helper: Lưu giỏ hàng vào Session
    private void SaveSessionCart(List<CartItemViewModel> cart)
    {
        var json = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString("SessionCart", json);
    }
}
