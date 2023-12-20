using Dotnet.Homeworks.Data.DatabaseContext;
using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.Homeworks.DataAccess.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _dbContext;

    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Product>().ToListAsync(cancellationToken);
    }

    public Task DeleteProductByGuidAsync(Guid id, CancellationToken cancellationToken)
    {
        _dbContext.Set<Product>().Remove(new Product() { Id = id });
        return Task.CompletedTask;
    }

    public Task UpdateProductAsync(Product product, CancellationToken cancellationToken)
    {
        _dbContext.Set<Product>().Update(product);
        return Task.CompletedTask;
    }

    public Task<Guid> InsertProductAsync(Product product, CancellationToken cancellationToken)
    {
        var result = _dbContext.Set<Product>().Add(product);
        return Task.FromResult(result.Entity.Id);
    }
}