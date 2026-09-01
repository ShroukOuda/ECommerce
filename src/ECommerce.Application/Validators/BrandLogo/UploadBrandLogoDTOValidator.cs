using ECommerce.Application.DTO.BrandLogos;
using ECommerce.Application.Validators.Common;

namespace ECommerce.Application.Validators.CategoryImage;

public class UploadBrandLogoDTOValidator : AbstractValidator<UploadBrandLogoDTO>
{
     private readonly ImageFileValidator _fileValidator;
    private readonly FileValidationSettings _validationSettings;

    public UploadBrandLogoDTOValidator(
        ImageFileValidator fileValidator,
        IOptions<FileValidationSettings> validationSettings)
    {
        _fileValidator = fileValidator;
        _validationSettings = validationSettings.Value;
        

        RuleFor(x => x.SubType)
            .IsInEnum()
            .WithMessage("Invalid photo subtype");
        
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("File is required")
            .SetValidator(_fileValidator);
        
        When(x => x.SubType == ImageSubType.Icon, () =>
        {
            RuleFor(x => x.File.Length)
                .LessThanOrEqualTo(_validationSettings.IconMaxSizeInBytes)
                .WithMessage($"Icon must not exceed {_validationSettings.IconMaxSizeKB}KB");

            RuleFor(x => x.File.FileName)
                .Must(HaveValidIconExtension)
                .WithMessage($"Icon must be: {string.Join(", ", _validationSettings.IconAllowedExtensions)}");
        });
        
        When(x => x.SubType == ImageSubType.Banner, () =>
        {
            RuleFor(x => x.File.Length)
                .LessThanOrEqualTo(_validationSettings.BannerMaxSizeInBytes)
                .WithMessage($"Banner must not exceed {_validationSettings.BannerMaxSizeMB}MB");
        });

        When(x => x.SubType == ImageSubType.Thumbnail, () =>
        {
            RuleFor(x => x.File.Length)
                .LessThanOrEqualTo(_validationSettings.ThumbnailMaxSizeInBytes)
                .WithMessage($"Thumbnail must not exceed {_validationSettings.ThumbnailMaxSizeMB}MB");
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
        return _validationSettings.IconAllowedExtensions.Contains(extension);
    }
}