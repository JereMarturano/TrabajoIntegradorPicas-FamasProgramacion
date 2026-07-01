using FluentValidation;

namespace PicasYFamas.Application.Commands.RegisterPlayer;

public class RegisterPlayerCommandValidator : AbstractValidator<RegisterPlayerCommand>
{
    public RegisterPlayerCommandValidator()
    {
        RuleFor(x => x.Lastname)
            .NotEmpty().WithMessage("Lastname is required.");

        RuleFor(x => x.Firstname)
            .NotEmpty().WithMessage("Firstname is required.");

        RuleFor(x => x.Age)
            .GreaterThan(0).WithMessage("Age must be a positive number.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
    }
}
