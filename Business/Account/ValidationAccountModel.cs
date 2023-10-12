using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Account
{
    public class ValidationAccountModel : AbstractValidator<AccountCreateModel>
    {
        public ValidationAccountModel()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Thông tin trường UserName không được để trống");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Thông tin trường Email không được để trống");
            RuleFor(x => x.Birthday).NotEmpty().WithMessage("Thông tin trường Birthday không được để trống");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Thông tin trường Phone không được để trống");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Thông tin trường Address không được để trống");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Thông tin trường Password không được để trống");
        }
    }
}
