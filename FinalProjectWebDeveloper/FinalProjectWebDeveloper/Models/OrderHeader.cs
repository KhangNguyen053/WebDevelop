using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebDeveloper.Models;

[Table("OrderHeader")]
public class OrderHeader
{
    [Key]
    public int OrderID { get; set; }

    public int? CustomerID { get; set; }

    public int? VoucherID { get; set; }

    public DateTime? OrderDate { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Mới"; // Mới, Đã vận chuyển, Đã thanh toán

    [ForeignKey("CustomerID")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("VoucherID")]
    public virtual Voucher? Voucher { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
