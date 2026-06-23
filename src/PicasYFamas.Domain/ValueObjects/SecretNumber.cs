using System;
using System.Linq;
using PicasYFamas.Domain.Exceptions;

namespace PicasYFamas.Domain.ValueObjects;

public sealed record SecretNumber
{
    public string Value { get; }

    private SecretNumber(string value)
    {
        Value = value;
    }

    public static SecretNumber Create(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new DomainException("Number cannot be empty.");

        if (number.Length != 4)
            throw new DomainException("Number must contain exactly 4 digits.");

        if (!number.All(char.IsDigit))
            throw new DomainException("Number must contain only digits.");

        if (number.Distinct().Count() != 4)
            throw new DomainException("Number must contain 4 unique digits.");

        return new SecretNumber(number);
    }

    public static SecretNumber GenerateRandom()
    {
        var random = new Random();
        var digits = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            .OrderBy(x => random.Next())
            .Take(4)
            .ToArray();

        return new SecretNumber(string.Join("", digits));
    }
}
