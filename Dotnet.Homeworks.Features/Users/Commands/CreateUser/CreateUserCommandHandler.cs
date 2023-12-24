using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Features.Users.Decorators;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : CqrsDecorator<CreateUserCommand, Result<CreateUserDto>>,
    ICommandHandler<CreateUserCommand, CreateUserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IEnumerable<IValidator<CreateUserCommand>> validators,
        IPermissionCheck<IClientRequest> permissionCheck) : base(permissionCheck, validators)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    
    public override async Task<Result<CreateUserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var baseRes = await base.Handle(request, cancellationToken);
            if (baseRes.IsFailure) return baseRes;
            
            var user = new User
            {
                Email = request.Email,
                Name = request.Name
            };
            var res = await _userRepository.InsertUserAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new Result<CreateUserDto>(new CreateUserDto(res), true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new Result<CreateUserDto>(new CreateUserDto(Guid.Empty),false, e.Message);
        }
    }
}