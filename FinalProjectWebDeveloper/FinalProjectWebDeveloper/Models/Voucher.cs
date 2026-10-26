using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebDeveloper.Models;

[Table("Voucher")]
public class Voucher
{
    [Key]
    public int VoucherID { get; set; }

    [StringLength(50)]
    public string? VoucherCode { get; set; }

    public int? DiscountPercent { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsActive { get; set; } = true;

    public virtual ICollection<OrderHeader> Orders { get; set; } = new List<OrderHeader>();
}
