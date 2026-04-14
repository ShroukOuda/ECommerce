using ECommerce.Application.DTO.ProductImages;
using ECommerce.Application.Validators.Common;

namespace ECommerce.Application.Validators.ProductImage;

public class UploadProductImageDTOValidator : AbstractValidator<UploadProductImageDTO>
{
    private readonly ImageFileValidator _imageFileValidator;
    private readonly FileValidationSettings _validationSettings;

    public UploadProductImageDTOValidator(
        ImageFileValidator imageFileValidator,
        IOptions<FileValidationSettings> validationSettings
    )
    {
        _imageFileValidator = imageFileValidator;
        _validationSettings = validationSettings.Value;
        
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required");
        
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("File is required")
            .SetValidator(_imageFileValidator);
        
        RuleFor(x => x.File.Length)
            .LessThanOrEqualTo(_validationSettings.ProductImage.MaxFileSizeInBytes)
            .WithMessage($"Product photo size must not exceed {_validationSettings.ProductImage.MaxFileSizeInMB}MB");
        
        RuleFor(x => x.AltText)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.AltText))
            .WithMessage("Alt text cannot exceed 200 characters");
       
    }
}