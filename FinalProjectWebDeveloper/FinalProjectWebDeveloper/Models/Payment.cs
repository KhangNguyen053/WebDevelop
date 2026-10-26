using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebDeveloper.Models;

[Table("Payment")]
public class Payment
{
    [Key]
    public int PaymentID { get; set; }

    public int? OrderID { get; set; }

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    public DateTime? PaymentDate { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [ForeignKey("OrderID")]
    public virtual OrderHeader? OrderHeader { get; set; }
}
