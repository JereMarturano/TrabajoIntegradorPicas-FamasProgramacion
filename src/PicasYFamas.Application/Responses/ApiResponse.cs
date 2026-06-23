using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PicasYFamas.Application.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public List<ApiError> Errors { get; set; } = new();

    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };

    public static ApiResponse<T> Error(List<ApiError> errors) => new() { Success = false, Errors = errors };
    public static ApiResponse<T> Error(string code, string message) => new() { Success = false, Errors = new List<ApiError> { new ApiError(code, message) } };
}
