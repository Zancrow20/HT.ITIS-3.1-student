using System.Net.Mail;
using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.CreateUser;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public CreateUserValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Email)
            .EmailAddress()
            .Must(IsCorrectEmailAddress)
            .WithMessage("Invalid email");
        RuleFor(x => x.Email)
            .NotEmpty()
            .MustAsync(BeUnique)
            .WithMessage("Such email already exists");
    }

    private bool IsCorrectEmailAddress(string email)
    {
        try {
            var mail = new MailAddress(email);
            return mail.Address == email;
        } catch(FormatException)
        {
            return false;
        }
    }

    private async Task<bool> BeUnique(string email, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetUsersAsync(cancellationToken);
        var result = users.FirstOrDefault(x => x.Email == email);
        return result is null;
    }
}