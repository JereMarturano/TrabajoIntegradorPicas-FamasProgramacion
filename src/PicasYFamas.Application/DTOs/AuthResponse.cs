using System;

namespace PicasYFamas.Application.DTOs;

public record AuthResponse(Guid Id, string Username, string Email, string Token);
