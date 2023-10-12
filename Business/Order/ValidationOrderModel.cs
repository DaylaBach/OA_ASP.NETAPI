using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Business.Order
{
    public class ValidationOrderModel : AbstractValidator<OrderCreateModel>
    {
        public ValidationOrderModel() 
        {
            RuleFor(x => x.OrderId).NotEmpty().WithMessage("Thông tin trường OrderId không được để trống");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Thông tin trường CategoryName không được để trống");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Thông tin trường CategoryName không được để trống");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Thông tin trường CategoryName không được để trống");
            RuleFor(x => x.TotalPrice).NotEmpty().WithMessage("Thông tin trường CategoryName không được để trống");
            RuleFor(x => x.AccountId).NotEmpty().WithMessage("Thông tin trường CategoryName không được để trống");
        }
    }
}
