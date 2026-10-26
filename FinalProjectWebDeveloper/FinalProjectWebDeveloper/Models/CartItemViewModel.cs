namespace FinalProjectWebDeveloper.Models;

public class CartItemViewModel
{
    public int CartItemID { get; set; }
    public int ProductSizeID { get; set; }
    public int ProductID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SizeName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string? ImageURL { get; set; }

    public decimal TotalPrice => UnitPrice * Quantity;
}
