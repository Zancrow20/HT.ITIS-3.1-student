using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;
using Dotnet.Homeworks.Shared.Dto;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.Homeworks.Features.UserManagement.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, GetAllUsersDto>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(
        IUserRepository userRepository
    )
    {
        _userRepository = userRepository;
    }
    
    public async Task<Result<GetAllUsersDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var users = (await _userRepository.GetUsersAsync(cancellationToken));
            var usersDto = users.ToList()
                .ConvertAll(ConvertToUserDto).AsEnumerable();
            return new Result<GetAllUsersDto>(new GetAllUsersDto(usersDto!), true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new Result<GetAllUsersDto>(new GetAllUsersDto(Enumerable.Empty<GetUserDto>()), 
                false, e.Message);
        }
        
        GetUserDto? ConvertToUserDto(User user) => new(user.Id, user.Name, user.Email);
    }
}