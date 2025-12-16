using Microsoft.Extensions.Options;

namespace ECommerce.Application.Validators.Photo;

public class UploadPhotosDtoValidator : AbstractValidator<UploadPhotosDTO>
{
    private readonly PhotoFileValidator _fileValidator;
    private readonly FileValidationSettings _validationSettings;
    
    public UploadPhotosDtoValidator(
        PhotoFileValidator fileValidator,
        IOptions<FileValidationSettings> validationSettings)
    {
        _fileValidator = fileValidator;
        _validationSettings = validationSettings.Value;

        RuleFor(x => x.EntityId)
            .GreaterThan(0)
            .WithMessage("EntityId must be greater than 0");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid photo type");

        RuleFor(x => x.Files)
            .NotNull()
            .WithMessage("Files are required")
            .NotEmpty()
            .WithMessage("At least one file must be provided");

        RuleFor(x => x.Files.Count)
            .LessThanOrEqualTo(10)
            .WithMessage("Maximum 10 files can be uploaded at once");

        // Validate each file
        RuleForEach(x => x.Files)
            .SetValidator(_fileValidator);

        // Product-specific rules
        When(x => x.Type == PhotoType.ProductImage, () =>
        {
            RuleFor(x => x.Files.Count)
                .LessThanOrEqualTo(_validationSettings.Product.MaxImagesPerProduct)
                .WithMessage($"Maximum {_validationSettings.Product.MaxImagesPerProduct} images can be uploaded per product");
        });

        // Category-specific rules
        When(x => x.Type == PhotoType.CategoryMedia, () =>
        {
            RuleFor(x => x.Files.Count)
                .LessThanOrEqualTo(_validationSettings.Category.MaxImagesPerCategory)
                .WithMessage($"Maximum {_validationSettings.Category.MaxImagesPerCategory} images can be uploaded per category");
        });
        
    }
}