using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeSpace.ViewModels.Systems
{
    public class FunctionCreateRequestValidator : AbstractValidator<FunctionCreateRequest>
    {
        public FunctionCreateRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id value is required")
               .MaximumLength(50).WithMessage("Function Id cannot over limit 50 characters");

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name value is required")
              .MaximumLength(200).WithMessage("Name cannot over limit 200 characters");

            RuleFor(x => x.Url).NotEmpty().WithMessage("URL value is required")
             .MaximumLength(200).WithMessage("URL cannot over limit 200 characters");

            RuleFor(x => x.ParentId).MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.ParentId))
                .WithMessage("URL cannot over limit 50 characters");
        }
    }
}