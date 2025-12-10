namespace E_Commerece.Application.Mapping;

public class PhotoMapping : Profile
{
    public PhotoMapping()
    {
        CreateMap<Photo, PhotoDTO>().ReverseMap();
    }
   
}