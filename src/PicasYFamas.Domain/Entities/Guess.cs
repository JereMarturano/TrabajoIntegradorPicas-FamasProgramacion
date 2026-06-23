using System;

namespace PicasYFamas.Domain.Entities;

public class Guess
{
    public Guid Id { get; private set; }
    public string Number { get; private set; } = default!;
    public int Picas { get; private set; }
    public int Famas { get; private set; }
    public int AttemptNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Guess() { } // EF Core

    internal Guess(string number, int picas, int famas, int attemptNumber)
    {
        Id = Guid.NewGuid();
        Number = number;
        Picas = picas;
        Famas = famas;
        AttemptNumber = attemptNumber;
        CreatedAt = DateTime.UtcNow;
    }
}
