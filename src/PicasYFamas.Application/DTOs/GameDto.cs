using System;

namespace PicasYFamas.Application.DTOs;

public record GameDto(Guid GameId, string Status, int Attempts, DateTime CreatedAt);
