using System;
using System.Linq;
using FluentAssertions;
using PicasYFamas.Domain.Entities;
using PicasYFamas.Domain.Enums;
using PicasYFamas.Domain.Exceptions;
using PicasYFamas.Domain.ValueObjects;
using Xunit;

namespace PicasYFamas.UnitTests.Domain;

public class GameTests
{
    [Fact]
    public void MakeGuess_WithCorrectNumber_ShouldWinGame()
    {
        // Arrange
        var game = new Game();
        var secretNumber = game.SecretNumberValue;

        // Act
        var guess = game.MakeGuess(secretNumber);

        // Assert
        guess.Famas.Should().Be(4);
        guess.Picas.Should().Be(0);
        game.Status.Should().Be(GameStatus.Won);
    }

    [Fact]
    public void MakeGuess_WithExceedingMaxAttempts_ShouldLoseGame()
    {
        // Arrange
        var game = new Game(maxAttempts: 2);
        
        // Act
        var generateWrongGuess = () => 
        {
            var wrongDigits = "0123456789".Where(c => !game.SecretNumberValue.Contains(c)).Take(4).ToArray();
            return new string(wrongDigits);
        };
        var wrongNumber = generateWrongGuess();

        game.MakeGuess(wrongNumber);
        game.MakeGuess(wrongNumber);

        // Assert
        game.Status.Should().Be(GameStatus.Lost);
        var action = () => game.MakeGuess(wrongNumber);
        action.Should().Throw<DomainException>().WithMessage("*already Lost*");
    }

    [Fact]
    public void MakeGuess_WithInvalidFormat_ShouldThrowDomainException()
    {
        // Arrange
        var game = new Game();

        // Act & Assert
        var action1 = () => game.MakeGuess("123");
        var action2 = () => game.MakeGuess("1123");
        var action3 = () => game.MakeGuess("abcd");

        action1.Should().Throw<DomainException>().WithMessage("*exactly 4 digits*");
        action2.Should().Throw<DomainException>().WithMessage("*4 unique digits*");
        action3.Should().Throw<DomainException>().WithMessage("*only digits*");
    }
}
