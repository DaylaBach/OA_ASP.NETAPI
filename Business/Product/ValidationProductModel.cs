using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Product
{
    public class ValidationProductModel : AbstractValidator<ProductCreateModel>
    {
        public ValidationProductModel()
        {
            RuleFor(x => x.ProductName).NotEmpty().WithMessage("Thông tin trường ProductName không được để trống");
            RuleFor(x => x.Price).NotEmpty().WithMessage("Thông tin trường Price không được để trống");
            RuleFor(x => x.SalePrice).NotEmpty().WithMessage("Thông tin trường SalePrice không được để trống");
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Thông tin trường CategoryId không được để trống");

        }
    }
}
