using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Comment
{
    public class ValidationCommentModel : AbstractValidator<CommentModel>
    {
        public ValidationCommentModel()
        {
            RuleFor(x => x.Question).NotEmpty().WithMessage("Thông tin trường Question không được để trống");
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("Thông tin trường ProductId không được để trống");
            RuleFor(x => x.AccountId).NotEmpty().WithMessage("Thông tin trường AccountId không được để trống");
        }
    }
}
