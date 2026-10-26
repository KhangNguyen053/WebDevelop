using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebDeveloper.Models;

[Table("Role")]
public class Role
{
    [Key]
    public int RoleID { get; set; }

    [Required]
    [StringLength(50)]
    public string RoleName { get; set; } = string.Empty;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
