using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebDeveloper.Models;

[Table("ProductSize")]
public class ProductSize
{
    [Key]
    public int ProductSizeID { get; set; }

    public int? ProductID { get; set; }

    [Required]
    [StringLength(20)]
    public string SizeName { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [ForeignKey("ProductID")]
    public virtual Product? Product { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
