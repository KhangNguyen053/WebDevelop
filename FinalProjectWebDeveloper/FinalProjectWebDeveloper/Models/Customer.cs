using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebDeveloper.Models;

[Table("Customer")]
public class Customer
{
    [Key]
    public int CustomerID { get; set; }

    [StringLength(100)]
    public string? FullName { get; set; }

    [StringLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(255)]
    public string? Address { get; set; }

    public int? AccountID { get; set; }

    [ForeignKey("AccountID")]
    public virtual Account? Account { get; set; }

    public virtual Cart? Cart { get; set; }
    public virtual ICollection<OrderHeader> Orders { get; set; } = new List<OrderHeader>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
