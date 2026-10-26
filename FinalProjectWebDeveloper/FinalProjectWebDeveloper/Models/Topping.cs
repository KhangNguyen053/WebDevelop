using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebDeveloper.Models;

[Table("Topping")]
public class Topping
{
    [Key]
    public int ToppingID { get; set; }

    [Required]
    [StringLength(100)]
    public string ToppingName { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public virtual ICollection<ProductTopping> ProductToppings { get; set; } = new List<ProductTopping>();
}
