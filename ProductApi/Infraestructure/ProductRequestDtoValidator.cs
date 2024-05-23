using FluentValidation;

namespace ProductApi.Infraestructure
{
    public class ProductRequestDtoValidator : AbstractValidator<ProductRequestDto>
    {
        public ProductRequestDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required.");
            RuleFor(x => x.Status).InclusiveBetween(0, 1).WithMessage("Status must be 0 or 1.");
            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).WithMessage("Stock must be greater than or equal to 0.");
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        }
    }
}
