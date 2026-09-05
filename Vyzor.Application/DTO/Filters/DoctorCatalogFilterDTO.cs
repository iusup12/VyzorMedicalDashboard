namespace Vyzor.Application.DTO.Filters;

public class DoctorCatalogFilterDTO
{
    public string? Search { get; set; }

    public int? SpecializationId { get; set; }

    public bool? Available { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public double? MinRating { get; set; }

    public string? Sort { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 6;
}