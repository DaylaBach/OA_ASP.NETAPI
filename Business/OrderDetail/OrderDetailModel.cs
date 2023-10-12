using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.OrderDetail
{
    public class OrderDetailModel
    {
        public float? price { get; set; }

        public int? Quantity { get; set; }

        public virtual Data.DataModel.Product? Product { get; set; }
    }

    public class OrderDetailCreateModel
    {
        public Guid? Id { get; set; } = Guid.NewGuid();

        public float? price { get; set; }

        public int? Quantity { get; set; }

        /// <summary>
        /// khó ngoại tham chiếu đến bảng account
        /// </summary>
        public Guid? OrderId { get; set; }
        public virtual Data.DataModel.Order? Order { get; set; }

        /// <summary>
        /// khóa ngoại tham chiếu đến bảng product
        /// </summary>
        public Guid? ProductId { get; set; }
        public virtual Data.DataModel.Product? Product { get; set; }
    }
}
