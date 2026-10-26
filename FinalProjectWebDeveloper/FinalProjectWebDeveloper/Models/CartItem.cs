using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebDeveloper.Models;

[Table("CartItem")]
public class CartItem
{
    [Key]
    public int CartItemID { get; set; }

    public int? CartID { get; set; }

    public int? ProductSizeID { get; set; }

    public int Quantity { get; set; }

    [ForeignKey("CartID")]
    public virtual Cart? Cart { get; set; }

    [ForeignKey("ProductSizeID")]
    public virtual ProductSize? ProductSize { get; set; }
}
