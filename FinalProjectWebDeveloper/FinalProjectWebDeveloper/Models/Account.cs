using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebDeveloper.Models;

[Table("Account")]
public class Account
{
    [Key]
    public int AccountID { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    public int RoleID { get; set; }

    public bool? IsActive { get; set; } = true;

    [ForeignKey("RoleID")]
    public virtual Role? Role { get; set; }

    public virtual Customer? Customer { get; set; }
}
