using System;
using System.Collections.Generic;
using PicasYFamas.Domain.Enums;
using PicasYFamas.Domain.Exceptions;
using PicasYFamas.Domain.ValueObjects;

namespace PicasYFamas.Domain.Entities;

public class Game
{
    public Guid Id { get; private set; }
    public string SecretNumberValue { get; private set; } = default!; // Stored as string for EF Core
    public GameStatus Status { get; private set; }
    public int MaxAttempts { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<Guess> _guesses = new();
    public IReadOnlyCollection<Guess> Guesses => _guesses.AsReadOnly();

    private Game() { } // EF Core

    public Game(Guid userId, int maxAttempts = 10)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        var secretNumber = SecretNumber.GenerateRandom();
        SecretNumberValue = secretNumber.Value;
        Status = GameStatus.InProgress;
        MaxAttempts = maxAttempts;
        CreatedAt = DateTime.UtcNow;
    }

    public Guess MakeGuess(string guessedNumber)
    {
        if (Status != GameStatus.InProgress)
            throw new DomainException($"Cannot make a guess. Game is already {Status}.");

        var number = SecretNumber.Create(guessedNumber); // Validate input

        if (_guesses.Count >= MaxAttempts)
        {
            Status = GameStatus.Lost;
            throw new DomainException("Maximum attempts reached. Game over.");
        }

        int picas = 0;
        int famas = 0;

        for (int i = 0; i < 4; i++)
        {
            if (number.Value[i] == SecretNumberValue[i])
            {
                famas++;
            }
            else if (SecretNumberValue.Contains(number.Value[i]))
            {
                picas++;
            }
        }

        var attemptNumber = _guesses.Count + 1;
        var guess = new Guess(number.Value, picas, famas, attemptNumber);
        _guesses.Add(guess);

        if (famas == 4)
        {
            Status = GameStatus.Won;
        }
        else if (attemptNumber >= MaxAttempts)
        {
            Status = GameStatus.Lost;
        }

        return guess;
    }

    public void Cancel()
    {
        if (Status == GameStatus.InProgress)
        {
            Status = GameStatus.Cancelled;
        }
    }
}
