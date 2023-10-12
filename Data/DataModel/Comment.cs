using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.DataModel
{
    [Table("Comments")]
    public class Comment
    {
        [Key]
        [Column("commentId")]
        public Guid? CommentId { get; set; }

        [Column("question")]
        public string? Question { get; set; }

        [Column("answer")]
        public string? Answer { get; set; }

        [Column("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [Column("updatedDate")]
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// khó ngoại tham chiếu đến bảng account
        /// </summary>
        [Column("accountId")]
        public Guid? AccountId { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account? Account { get; set; }

        /// <summary>
        /// khóa ngoại tham chiếu đến bảng product
        /// </summary>
        [Column("productId")]
        public Guid? ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}