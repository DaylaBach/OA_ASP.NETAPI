using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.DataModel
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [Column("productId")]
        public Guid? ProductId { get; set; } = Guid.NewGuid();

        [Column("productName")]
        [StringLength(50)]
        public string? ProductName { get; set; }

        [Column("quantity")]
        public int? Quantity { get; set; }

        [Column("price")]
        public float? Price { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Column("salePrice")]
        public float? SalePrice { get; set; }

        [Column("status")]
        public bool Status { get; set; } = true;

        [Column("created_date")]
        public DateTime? CreatedDate { get; set; }

        [Column("updatedDate")]
        public DateTime? UpdatedDate { get; set; }

        [Column("image")]
        public string? Image { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        /// <summary>
        /// khóa ngoại tham chiếu đến bảng category
        /// </summary>
        [Column("categoryId")]
        public Guid? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }
}