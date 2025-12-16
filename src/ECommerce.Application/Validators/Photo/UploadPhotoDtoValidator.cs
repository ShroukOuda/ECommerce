using Microsoft.Extensions.Options;

namespace ECommerce.Application.Validators.Photo;

public class UploadPhotoDtoValidator : AbstractValidator<UploadPhotoDTO>
{
    private readonly PhotoFileValidator _fileValidator;
    private readonly FileValidationSettings _validationSettings;
    
    public UploadPhotoDtoValidator(
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

        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("File is required")
            .SetValidator(_fileValidator);

        // Category Icon specific rules
        When(x => x.SubType == PhotoSubType.CategoryIcon, () =>
        {
            RuleFor(x => x.File.Length)
                .LessThanOrEqualTo(_validationSettings.Category.IconMaxSizeInBytes)
                .WithMessage($"Icon file size must not exceed {_validationSettings.Category.IconMaxSizeKB}KB");

            RuleFor(x => x.File.FileName)
                .Must(HaveValidIconExtension)
                .WithMessage($"Invalid icon type. Allowed extensions: {string.Join(", ", _validationSettings.Category.IconAllowedExtensions)}");
        });

        // Category Banner specific rules
        When(x => x.SubType == PhotoSubType.CategoryBanner, () =>
        {
            RuleFor(x => x.File.Length)
                .LessThanOrEqualTo(_validationSettings.Category.BannerMaxSizeInBytes)
                .WithMessage($"Banner file size must not exceed {_validationSettings.Category.BannerMaxSizeMB}MB");
        });

        // Category Thumbnail specific rules
        When(x => x.SubType == PhotoSubType.CategoryThumbnail, () =>
        {
            RuleFor(x => x.File.Length)
                .LessThanOrEqualTo(_validationSettings.Category.ThumbnailMaxSizeInBytes)
                .WithMessage($"Thumbnail file size must not exceed {_validationSettings.Category.ThumbnailMaxSizeMB}MB");
        });
        

        // Product Image specific rules
        When(x => x.Type == PhotoType.ProductImage, () =>
        {
            RuleFor(x => x.File.Length)
                .LessThanOrEqualTo(_validationSettings.Product.MaxFileSizeInBytes)
                .WithMessage($"Product image file size must not exceed {_validationSettings.Product.MaxFileSizeInMB}MB");
        });
    }

    private bool HaveValidIconExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return _validationSettings.Category.IconAllowedExtensions.Contains(extension);
    }
}