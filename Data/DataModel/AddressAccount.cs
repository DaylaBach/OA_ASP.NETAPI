using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.DataModel
{
    [Table("AddressAccount")]
    public class AddressAccount
    {
        [Key]
        [Column("address_id")]
        public Guid? AddressId { get; set; } = Guid.NewGuid();

        [Column("address")]
        public string? Address { get; set; }

        [Column("image")]
        public string? Image { get; set; }

        [Column("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [Column("updatedDate")]
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// khóa ngoại tham chiếu đến bảng Account
        /// </summary>
        [Column("accountId")]
        public Guid? AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account? Account { get; set; }
    }
}