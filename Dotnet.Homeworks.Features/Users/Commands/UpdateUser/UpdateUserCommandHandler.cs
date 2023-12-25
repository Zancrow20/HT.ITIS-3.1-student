using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Features.Users.Commands.CreateUser;
using Dotnet.Homeworks.Features.Users.Commands.DeleteUser;
using Dotnet.Homeworks.Features.Users.Decorators;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : CqrsDecorator<UpdateUserCommand, Result>, ICommandHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IUserRepository userRepository, 
        IEnumerable<IValidator<UpdateUserCommand>> validators,
        IPermissionCheck<IClientRequest> permissionCheck,
        IUnitOfWork unitOfWork) : base(permissionCheck, validators)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    public override async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var baseRes = await base.Handle(request, cancellationToken);
            if (baseRes.IsFailure) 
                return baseRes;
            
            await _userRepository.UpdateUserAsync(request.User, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new Result(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new Result(false, e.Message);
        }
    }
}