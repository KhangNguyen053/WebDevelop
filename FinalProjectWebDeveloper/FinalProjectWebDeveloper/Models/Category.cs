using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebDeveloper.Models;

[Table("Category")]
public class Category
{
    [Key]
    public int CategoryID { get; set; }

    [Required]
    [StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
