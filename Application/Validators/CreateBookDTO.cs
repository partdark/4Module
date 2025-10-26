

using Application.DTO;
using FluentValidation;

namespace Application.Validator
{
    
    public class CreateBookDTOValidator : AbstractValidator<CreateBookDTO>
    {
        public CreateBookDTOValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Empty title");
            RuleFor(x => x.Year)
                .GreaterThan(1900).WithMessage("Year must greate than 1900")
                .LessThanOrEqualTo(DateTime.Now.Year);

           

        }

    }
}

