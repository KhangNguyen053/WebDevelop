using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebDeveloper.Models;

[Table("OrderDetail")]
public class OrderDetail
{
    [Key]
    public int OrderDetailID { get; set; }

    public int? OrderID { get; set; }

    public int? ProductSizeID { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [ForeignKey("OrderID")]
    public virtual OrderHeader? OrderHeader { get; set; }

    [ForeignKey("ProductSizeID")]
    public virtual ProductSize? ProductSize { get; set; }
}
