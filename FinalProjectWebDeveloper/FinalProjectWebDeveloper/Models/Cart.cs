using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebDeveloper.Models;

[Table("Cart")]
public class Cart
{
    [Key]
    public int CartID { get; set; }

    public int? CustomerID { get; set; }

    [ForeignKey("CustomerID")]
    public virtual Customer? Customer { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
