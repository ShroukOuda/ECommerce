namespace E_Commerece.Infrastructure.Repositories;

public class PhotoRepository : GenericRepository<Photo>, IPhotoRepository
{
    public PhotoRepository(AppDbContext context) : base(context)
    {
    }
}