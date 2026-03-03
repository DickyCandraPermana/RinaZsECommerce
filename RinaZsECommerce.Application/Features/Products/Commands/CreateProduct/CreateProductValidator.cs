using FluentValidation;

namespace RinaZsECommerce.Application.Features.Products.Commands.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        // Aturan untuk Nama Produk
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Nama produk tidak boleh kosong.")
            .MaximumLength(200).WithMessage("Nama produk maksimal 200 karakter.")
            .MinimumLength(3).WithMessage("Nama produk minimal 3 karakter.");

        // Aturan untuk Harga
        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("Harga harus lebih besar dari 0.");

        // Aturan untuk Stok
        RuleFor(p => p.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stok tidak boleh negatif.");

        // Aturan untuk Kategori
        RuleFor(p => p.CategoryId)
            .NotEmpty().WithMessage("Kategori harus dipilih.");

        // Aturan untuk ImageUrls (Koleksi)
        RuleFor(p => p.ImageUrls)
            .Must(x => x != null && x.Count > 0).WithMessage("Minimal harus ada 1 gambar produk.")
            .ForEach(imageRule =>
            {
                imageRule.NotEmpty().WithMessage("URL Gambar tidak boleh kosong.")
                         .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                         .WithMessage("Format URL gambar tidak valid.");
            });
    }
}