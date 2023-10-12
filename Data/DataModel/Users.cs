using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.DataModel
{
    [Table("Users")]
    public class Users
    {
        [Key]
        [Column("user_id")]
        public Guid? Id { get; set; }

        [Column("userName")]
        [StringLength(50)]
        public string? UserName { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Column("email")]
        [StringLength(50)]
        public string? Email { get; set; }

        [Column("phone")]
        [StringLength(50)]
        public string? Phone { get; set; }

        [Column("avatar")]
        public string? Avatar { get; set; }

        [Column("birtday")]
        public string? Birthday { get; set; }

        [Column("password")]
        public string? Password { get; set; }

        [Column("state")]
        public bool Sate { get; set; } = true;

        [Column("countError")]
        public int CountError { get; set; }

        [Column("timelock")]
        public DateTime? TimeLock { get; set; }

        [Column("islock")]
        public bool IsLock { get; set; }

        [Column("tokenChange_password")]
        public string? TokenChangePassword { get; set; }

        [Column("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [Column("updatedDate")]
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// khóa ngoại tham chiếu đến bảng Roles
        /// </summary>
        [Column("roleId")]
        public int? RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Roles? Roles { get; set; }
    }
}