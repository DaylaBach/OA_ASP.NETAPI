using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Category
{
    public class ValidationCategoryModel : AbstractValidator<CategoryCreateModel>
    {
        public ValidationCategoryModel()
        {
            RuleFor(x => x.CategoryName).NotEmpty().WithMessage("Thông tin trường CategoryName không được để trống");
        }
    }
}
