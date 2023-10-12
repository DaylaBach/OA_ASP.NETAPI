using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataModel
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        [Key]
        [Column("id")]
        public Guid? Id { get; set; } = Guid.NewGuid();

        [Column("Price")]
        public float? price { get; set; }

        [Column("quantity")]
        public int? Quantity { get; set; }

        /// <summary>
        /// khó ngoại tham chiếu đến bảng account
        /// </summary>
        [Column("orderId")]
        public Guid? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        /// <summary>
        /// khóa ngoại tham chiếu đến bảng product
        /// </summary>
        [Column("productId")]
        public Guid? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
