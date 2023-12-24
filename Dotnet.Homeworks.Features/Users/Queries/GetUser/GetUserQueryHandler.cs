using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Features.Users.Commands.DeleteUser;
using Dotnet.Homeworks.Features.Users.Decorators;
using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Queries.GetUser;

public class GetUserQueryHandler : CqrsDecorator<GetUserQuery, Result<GetUserDto>>,
    IQueryHandler<GetUserQuery, GetUserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(
        IUserRepository userRepository,
        IEnumerable<IValidator<GetUserQuery>> validators,
        IPermissionCheck<IClientRequest> permissionCheck) : base(permissionCheck, validators)
    {
        _userRepository = userRepository;
    }

    public override async Task<Result<GetUserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var baseRes = await base.Handle(request, cancellationToken);
            if (baseRes.IsFailure) return baseRes;
            
            var user = await _userRepository.GetUserByGuidAsync(request.Guid, cancellationToken);
            if (user == null)
                return new Result<GetUserDto>(new GetUserDto(Guid.Empty, string.Empty, string.Empty),
                    false);
            return new Result<GetUserDto>(new GetUserDto(user.Id, user.Name, user.Email), true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new Result<GetUserDto>(new GetUserDto(Guid.Empty, string.Empty, string.Empty),
                false, e.Message);
        }
    }
}