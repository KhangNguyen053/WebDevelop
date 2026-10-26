using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebDeveloper.Models;

[Table("Product")]
public class Product
{
    [Key]
    public int ProductID { get; set; }

    [Required]
    [StringLength(200)]
    public string ProductName { get; set; } = string.Empty;



    [StringLength(255)]
    public string? ImageURL { get; set; }

    public int? CategoryID { get; set; }

    public bool? IsAvailable { get; set; } = true;

    [ForeignKey("CategoryID")]
    public virtual Category? Category { get; set; }

    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<ProductTopping> ProductToppings { get; set; } = new List<ProductTopping>();
}
