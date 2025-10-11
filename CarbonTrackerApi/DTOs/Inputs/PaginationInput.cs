using System.ComponentModel.DataAnnotations;

namespace CarbonTrackerApi.DTOs.Inputs;

public class PaginationInput
{
    [Range(1, int.MaxValue, ErrorMessage = "O número da página deve ser maior ou igual a 1.")]
    public int PageNumber { get; set; }

    [Range(1, 100, ErrorMessage = "O tamanho da página deve ser entre 1 e 100.")]
    public int PageSize { get; set; }

    public PaginationInput() { }

    public PaginationInput(int pageNumber = 1, int pageSize = 10)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}