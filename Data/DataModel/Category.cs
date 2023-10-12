using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataModel
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        [Column("category_id")]
        public Guid? CategoryId { get; set; } = Guid.NewGuid();

        [Column("categoryName")]
        public string? CategoryName { get; set; }

        [Column("status")]
        public bool  Status { get; set; }

        [Column("image")]
        public string? Image { get; set; }

        [Column("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [Column("updatedDate")]
        public DateTime? UpdatedDate { get; set; }
    }
}
