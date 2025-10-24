using _4Module.DTO;
using FluentValidation;

namespace _4Module.Validator
{
    
    public class CreateBookDTOValidator : AbstractValidator<CreateBookDTO>
    {
        public CreateBookDTOValidator()
        {
            RuleFor(x => x.title)
                .NotEmpty()
                .WithMessage("Empty title");
            RuleFor(x => x.year)
                .GreaterThan(1900).WithMessage("Year must greate than 1900")
                .LessThanOrEqualTo(DateTime.Now.Year);

            RuleFor(x => x.AutorId)
                .NotEmpty()
                .WithMessage("Empty author");

        }

    }
}

