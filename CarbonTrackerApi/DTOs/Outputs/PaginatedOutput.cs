namespace CarbonTrackerApi.DTOs.Outputs;

public record PaginatedOutput<T>
(
    List<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);