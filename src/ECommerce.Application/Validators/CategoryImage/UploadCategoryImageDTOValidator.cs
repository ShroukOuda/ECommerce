using ECommerce.Application.DTO.CategoryImages;
using ECommerce.Application.Validators.Common;

namespace ECommerce.Application.Validators.CategoryImage;

public class UploadCategoryImageDTOValidator : AbstractValidator<UploadCategoryImageDTO>
{
     private readonly ImageFileValidator _fileValidator;
    private readonly FileValidationSettings _validationSettings;

    public UploadCategoryImageDTOValidator(
        ImageFileValidator fileValidator,
        IOptions<FileValidationSettings> validationSettings)
    {
        _fileValidator = fileValidator;
        _validationSettings = validationSettings.Value;
        
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("CategoryId must be greater than 0");

        RuleFor(x => x.SubType)
            .IsInEnum()
            .WithMessage("Invalid photo subtype");
        
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("File is required")
            .SetValidator(_fileValidator);
        
        When(x => x.SubType == ImageSubType.CategoryIcon, () =>
        {
            RuleFor(x => x.File.Length)
                .LessThanOrEqualTo(_validationSettings.CategoryImage.IconMaxSizeInBytes)
                .WithMessage($"Icon must not exceed {_validationSettings.CategoryImage.IconMaxSizeKB}KB");

            RuleFor(x => x.File.FileName)
                .Must(HaveValidIconExtension)
                .WithMessage($"Icon must be: {string.Join(", ", _validationSettings.CategoryImage.IconAllowedExtensions)}");
        });
        
        When(x => x.SubType == ImageSubType.CategoryBanner, () =>
        {
            RuleFor(x => x.File.Length)
                .LessThanOrEqualTo(_validationSettings.CategoryImage.BannerMaxSizeInBytes)
                .WithMessage($"Banner must not exceed {_validationSettings.CategoryImage.BannerMaxSizeMB}MB");
        });

        When(x => x.SubType == ImageSubType.CategoryThumbnail, () =>
        {
            RuleFor(x => x.File.Length)
                .LessThanOrEqualTo(_validationSettings.CategoryImage.ThumbnailMaxSizeInBytes)
                .WithMessage($"Thumbnail must not exceed {_validationSettings.CategoryImage.ThumbnailMaxSizeMB}MB");
        });
        
        RuleFor(x => x.AltText)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.AltText))
            .WithMessage("Alt text cannot exceed 200 characters");
    }
    private bool HaveValidIconExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;
        
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return _validationSettings.CategoryImage.IconAllowedExtensions.Contains(extension);
    }
}