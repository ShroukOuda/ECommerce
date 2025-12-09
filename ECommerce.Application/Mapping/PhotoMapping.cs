using AutoMapper;
using E_Commerece.Core.DTO;
using E_Commerece.Core.Entites.Product;

namespace E_Commerece.Application.Mapping;

public class PhotoMapping : Profile
{
    public PhotoMapping()
    {
        CreateMap<Photo, PhotoDTO>().ReverseMap();
    }
   
}