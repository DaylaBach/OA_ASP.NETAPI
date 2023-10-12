using Data.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Order
{
    public class OrderModel
    {
        public float? TotalPrice { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? State { get; set; } = 0;

        public Guid? AccountId { get; set; }
    }

    public class OrderCreateModel
    {
        public Guid? OrderId { get; set; } = Guid.NewGuid();

        public float? TotalPrice { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? State { get; set; } = 0;

        public string? CancellationReason { get; set; }

        public string? Feedback { get; set; }

        public Guid? AccountId { get; set; }

        public virtual Data.DataModel.Account? Account { get; set; }

        public virtual List<Data.DataModel.OrderDetail>? ListOrderDetail { get; set; }

        public virtual List<Data.DataModel.Product>? ListProduct { get; set; }
    }
}
