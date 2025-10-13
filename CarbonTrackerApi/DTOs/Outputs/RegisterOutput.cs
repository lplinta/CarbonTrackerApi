namespace CarbonTrackerApi.DTOs.Outputs;

public record RegisterOutput
(
    int Id,
    string Username,
    string Email,
    string Role
);
