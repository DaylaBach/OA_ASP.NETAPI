using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Comment
{
    public class CommentModel
    {
        public Guid? CommentId { get; set; } = Guid.NewGuid();

        public string? Question { get; set; }

        public string? Answer { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// khó ngoại tham chiếu đến bảng account
        /// </summary>
        public Guid? AccountId { get; set; }

        public virtual Data.DataModel.Account? Account { get; set; }

        /// <summary>
        /// khóa ngoại tham chiếu đến bảng product
        /// </summary>
        public Guid? ProductId { get; set; }

        public virtual Data.DataModel.Product? Product { get; set; }
    }
}
