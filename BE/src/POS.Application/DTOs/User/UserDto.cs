namespace POS.Application.DTOs.User;

public record UserDto(
    Guid Id,
    string Username,
    string FullName,
    string Email,
    string PhoneNumber,
    string Role,
    bool IsActive
);