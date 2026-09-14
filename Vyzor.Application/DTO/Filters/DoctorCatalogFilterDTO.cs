using Vyzor.Application.Common;

namespace Vyzor.Application.DTO.Filters;

public class DoctorCatalogFilterDTO : PagedRequest
{
    public string? Search { get; set; }

    public List<int> SpecializationIds { get; set; } = new();

    public bool? Available { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public double? MinRating { get; set; }

    public string? Sort { get; set; }
}