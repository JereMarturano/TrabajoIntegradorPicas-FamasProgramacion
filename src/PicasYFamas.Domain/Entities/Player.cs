using System;
using System.Collections.Generic;
using PicasYFamas.Domain.Enums;
using PicasYFamas.Domain.Exceptions;

namespace PicasYFamas.Domain.Entities;

public class Player
{
    public Guid Id { get; private set; }
    public string Lastname { get; private set; } = default!;
    public string Firstname { get; private set; } = default!;
    public int Age { get; private set; }
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    private readonly List<Game> _games = new();
    public IReadOnlyCollection<Game> Games => _games.AsReadOnly();

    private Player() { } // EF Core

    public Player(string lastname, string firstname, int age, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(lastname))
            throw new DomainException("Lastname is required.");
        if (string.IsNullOrWhiteSpace(firstname))
            throw new DomainException("Firstname is required.");
        if (age <= 0)
            throw new DomainException("Age must be a positive number.");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("PasswordHash is required.");

        Id = Guid.NewGuid();
        Lastname = lastname;
        Firstname = firstname;
        Age = age;
        Email = email.ToLowerInvariant();
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }

    public Game? GetActiveGame()
    {
        foreach (var game in _games)
        {
            if (game.Status == GameStatus.InProgress)
                return game;
        }
        return null;
    }
}
