using ECommerce.Application.DTO.Common;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.DTO.ProductImages;

public class UploadProductImageDTO : UploadImageDTO
{ 
    public bool IsMain { get; set; }
}