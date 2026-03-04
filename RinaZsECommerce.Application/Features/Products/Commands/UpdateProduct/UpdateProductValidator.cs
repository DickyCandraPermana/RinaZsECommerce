using System;
using FluentValidation;

namespace RinaZsECommerce.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
  public UpdateProductValidator()
  {
    // Rule hanya jalan JIKA properti tidak null
    RuleFor(p => p.Name)
        .NotEmpty().MinimumLength(3).MaximumLength(200)
        .When(p => p.Name != null);

    RuleFor(p => p.Price)
        .GreaterThan(0)
        .When(p => p.Price != null);

    RuleFor(p => p.Stock)
        .GreaterThanOrEqualTo(0)
        .When(p => p.Stock != null);

    RuleFor(p => p.ImageUrls)
        .Must(x => x!.Count > 0)
        .When(p => p.ImageUrls != null)
        .WithMessage("Jika ingin update gambar, minimal harus ada 1.");
  }
}