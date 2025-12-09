namespace E_Commerece.Core.DTO;

public record AddCategoryDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public record UpdateCategoryDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
   