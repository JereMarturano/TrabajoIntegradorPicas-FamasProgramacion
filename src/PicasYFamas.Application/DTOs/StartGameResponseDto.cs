using System;

namespace PicasYFamas.Application.DTOs;

public record StartGameResponseDto(Guid GameId, Guid PlayerId, DateTime CreatedAt);
