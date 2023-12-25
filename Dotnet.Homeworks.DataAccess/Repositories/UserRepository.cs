using Dotnet.Homeworks.Data.DatabaseContext;
using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.Homeworks.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task<IQueryable<User>> GetUsersAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_dbContext.Set<User>().AsQueryable());
    }

    public async Task<User?> GetUserByGuidAsync(Guid guid, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == guid, cancellationToken: cancellationToken);
    }

    public async Task DeleteUserByGuidAsync(Guid guid, CancellationToken cancellationToken)
    {
        await _dbContext.Set<User>().Where(u => u.Id == guid).ExecuteDeleteAsync(cancellationToken: cancellationToken);
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Set<User>()
            .Where(u => u.Id == user.Id)
            .ExecuteUpdateAsync(setProps => setProps
                    .SetProperty(u => u.Name, user.Name)
                    .SetProperty(u => u.Email, user.Email), cancellationToken: cancellationToken);
    }

    public Task<Guid> InsertUserAsync(User user, CancellationToken cancellationToken)
    {
        var res = _dbContext.Set<User>().Add(user);
        return Task.FromResult(res.Entity.Id);
    }
}