using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeSpace.ViewModels.Systems
{
    public class UserPasswordChangeRequestValidator : AbstractValidator<UserPasswordChangeRequest>
    {
        public UserPasswordChangeRequestValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty()
                 .WithMessage(string.Format(Messages.Required, "Mật khẩu cũ"));

            RuleFor(x => x.NewPassword).NotEmpty()
                .WithMessage(string.Format(Messages.Required, "Mật khẩu mới"));

            RuleFor(x => x.UserId).NotEmpty()
                .WithMessage(string.Format(Messages.Required, "Người dùng"));

            RuleFor(x => x).Custom((x, context) =>
            {
                if (x.CurrentPassword == x.NewPassword)
                {
                    context.AddFailure("Mật khẩu mới không được trùng mật khẩu cũ");
                }
            });

            RuleFor(x => x.NewPassword)
               .MinimumLength(8).WithMessage(string.Format(Messages.MinLength, "Mật khẩu mới", 8));

            RuleFor(x => x.NewPassword).Matches(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")
               .When(x => x.NewPassword != null)
               .WithMessage("Mật khẩu chưa đủ độ phức tạp");
        }
    }
}