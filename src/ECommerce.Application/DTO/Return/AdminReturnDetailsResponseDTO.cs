namespace ECommerce.Application.DTO.Return;
public class AdminReturnDetailsResponseDTO : ReturnDetailsResponseDTO
{
    public string UserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string UserEmail { get; set; } = string.Empty;
}