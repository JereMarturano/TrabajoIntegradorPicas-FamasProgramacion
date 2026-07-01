using System;

namespace PicasYFamas.Application.DTOs;

public record AuthResponseDto(Guid PlayerId, string Token);
