using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.DataModel
{
    [Table("Roles")]
    public class Roles
    {
        [Key]
        [Column("roleId")]
        public int? RoleId { get; set; }

        [Column("roleName")]
        [StringLength(50)]
        public string? RoleName { get; set; }
    }
}