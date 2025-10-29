namespace E_Commerece.Core.DTO;

    public record ProductDTO 
    (string Name, float Price, string Description, int CategoryId);

    public record UpdateProductDTO
    (int Id, string Name, float Price, string Description, int CategoryId);

    public record GetProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public List<PhotoDTO> Photos { get; set; }
    }
   