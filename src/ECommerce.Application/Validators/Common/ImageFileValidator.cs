using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.Validators.Common;

public class ImageFileValidator : AbstractValidator<IFormFile>
{
    private readonly FileValidationSettings _validationSettings;

    public ImageFileValidator(
        IOptions<FileValidationSettings> validationSettings)
    {
        _validationSettings = validationSettings.Value;
        
        RuleFor(file => file)
            .NotNull()
            .WithMessage("File is required");

        RuleFor(file => file.Length)
            .NotEmpty()
            .WithMessage("File cannot be empty")
            .LessThanOrEqualTo(_validationSettings.MaxFileSizeInBytes)
            .WithMessage($"File size must not exceed {_validationSettings.MaxFileSizeInMB}MB");

        RuleFor(file => file.FileName)
            .NotEmpty()
            .WithMessage("File name is required")
            .Must(HaveValidExtension)
            .WithMessage($"Invalid file type. Allowed extensions: {string.Join(", ", _validationSettings.AllowedExtensions)}");

        RuleFor(file => file.ContentType)
            .Must(BeValidImageContentType)
            .WithMessage("File must be a valid image (image/jpeg, image/png, image/gif, image/webp)");
    }
    
    private bool HaveValidExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return _validationSettings.IsAllowedExtension(extension);
    }

    private bool BeValidImageContentType(string contentType)
    {
        if (string.IsNullOrEmpty(contentType))
            return false;

        var validContentTypes = new[]
        {
            "image/jpeg",
            "image/jpg",
            "image/png",
            "image/webp",
            "image/svg+xml"
        };

        return validContentTypes.Contains(contentType.ToLowerInvariant());
    }
}