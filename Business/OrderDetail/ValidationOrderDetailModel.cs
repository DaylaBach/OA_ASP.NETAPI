using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Business.OrderDetail
{
    public class ValidationOrderDetailModel : AbstractValidator<OrderDetailModel>
    {
        public ValidationOrderDetailModel()
        {
            
        }
    }
}
