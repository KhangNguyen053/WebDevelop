using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebDeveloper.Models;

[Table("ProductTopping")]
public class ProductTopping
{
    public int ProductID { get; set; }
    public int ToppingID { get; set; }

    [ForeignKey("ProductID")]
    public virtual Product? Product { get; set; }

    [ForeignKey("ToppingID")]
    public virtual Topping? Topping { get; set; }
}
