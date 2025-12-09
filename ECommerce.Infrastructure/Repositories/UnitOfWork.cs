using System.ComponentModel.DataAnnotations;
using E_Commerece.Core.Interfaces;
using E_Commerece.Infrastructure.Data;

namespace E_Commerece.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    public ICategoryRepository CategoryRepository { get; }
    public IProductRepository ProductRepository { get; }
    public IPhotoRepository PhotoRepository { get; }
    
    public UnitOfWork(AppDbContext context)
    {
        _context = context; 
        CategoryRepository = new CategoryRepository(_context);
        ProductRepository = new ProductRepository(_context);
        PhotoRepository = new PhotoRepository(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}