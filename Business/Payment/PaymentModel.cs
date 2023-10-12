using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Payment
{
    public class PaymentModel
    {
        public Guid? PaymentId { get; set; } = Guid.NewGuid();

        public string? Type { get; set; }

        public float? Amount { get; set; }

        public string? Bank { get; set; }

        public string? Image { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid? OrderId { get; set; }
    }
}
