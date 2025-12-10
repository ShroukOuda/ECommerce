using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.DTO;

    public record AddProductDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public int CategoryId { get; set; }
        public IFormFileCollection Photos { get; set; } 
    }


    public record UpdateProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public IFormFileCollection? NewPhotos { get; set; }
        public List<int>? PhotosToDelete { get; set; }
        
        
    }

    public record GetProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public List<PhotoDTO> Photos { get; set; }
    }
   