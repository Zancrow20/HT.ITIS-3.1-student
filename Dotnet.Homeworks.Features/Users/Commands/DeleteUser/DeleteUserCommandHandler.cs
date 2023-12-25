using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Features.Users.Decorators;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : CqrsDecorator<DeleteUserCommand,Result>, ICommandHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(
        IUserRepository userRepository,
        IEnumerable<IValidator<DeleteUserCommand>> validators,
        IPermissionCheck<IClientRequest> permissionCheck,
        IUnitOfWork unitOfWork) : base(permissionCheck, validators)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    
    public override async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var baseRes = await base.Handle(request, cancellationToken);
            if (baseRes.IsFailure) return baseRes;
            
            var user = await _userRepository.GetUserByGuidAsync(request.Guid, cancellationToken);
            if (user is null)
                return new Result(false,"User doesn't exist");
            await _userRepository.DeleteUserByGuidAsync(request.Guid, cancellationToken);
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