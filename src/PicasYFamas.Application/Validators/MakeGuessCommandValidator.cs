using FluentValidation;
using PicasYFamas.Application.Commands.MakeGuess;
using System.Linq;

namespace PicasYFamas.Application.Validators;

public class MakeGuessCommandValidator : AbstractValidator<MakeGuessCommand>
{
    public MakeGuessCommandValidator()
    {
        RuleFor(x => x.GameId).NotEmpty().WithMessage("Game ID is required.");

        RuleFor(x => x.AttemptedNumber)
            .NotEmpty().WithMessage("Number is required.")
            .Length(4).WithMessage("Number must contain exactly 4 digits.")
            .Must(n => n.All(char.IsDigit)).WithMessage("Number must contain only digits.")
            .Must(n => n.Distinct().Count() == 4).WithMessage("Number must contain 4 unique digits.");
    }
}
