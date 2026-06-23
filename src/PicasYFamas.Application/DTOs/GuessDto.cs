using System;

namespace PicasYFamas.Application.DTOs;

public record GuessDto(string Guess, int Picas, int Famas, int AttemptNumber, bool IsWinner = false);
