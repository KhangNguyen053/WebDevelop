using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebDeveloper.Models;

[Table("Review")]
public class Review
{
    [Key]
    public int ReviewID { get; set; }

    public int? CustomerID { get; set; }

    public int? ProductID { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(500)]
    public string? Comment { get; set; }

    public DateTime? ReviewDate { get; set; } = DateTime.Now;

    [ForeignKey("CustomerID")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("ProductID")]
    public virtual Product? Product { get; set; }
}
