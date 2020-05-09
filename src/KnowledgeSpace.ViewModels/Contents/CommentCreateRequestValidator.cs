using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeSpace.ViewModels.Contents
{
    public class CommentCreateRequestValidator : AbstractValidator<CommentCreateRequest>
    {
        public CommentCreateRequestValidator()
        {
            RuleFor(x => x.KnowledgeBaseId).GreaterThan(0)
                 .WithMessage("Mã bài đăng không đúng");

            RuleFor(x => x.Content).NotEmpty().WithMessage("Chưa nhập nội dung");

            RuleFor(x => x.CaptchaCode).NotEmpty()
              .WithMessage("Nhập mã xác nhận");
        }
    }
}