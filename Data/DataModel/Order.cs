using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataModel
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        [Column("orderId")]
        public Guid? OrderId { get; set; } = Guid.NewGuid();

        [Column("totalPrice")]
        public float? TotalPrice { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Column("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [Column("updatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [Column("state")]
        public int? State { get; set; } = 0;

        [Column("cancellation_reason")]
        public string? CancellationReason { get; set; }

        [Column("feedback")]
        public string? Feedback { get; set; }

        /// <summary>
        /// khó ngoại tham chiếu đến bảng account
        /// </summary>
        [Column("accountId")]
        public Guid? AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account? Account { get; set; }

    }
}
