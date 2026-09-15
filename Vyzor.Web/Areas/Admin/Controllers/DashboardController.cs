using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vyzor.Application.Interfaces;
using Vyzor.Web.Authorization;

namespace Vyzor.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AppPolicies.DoctorOrAdmin)]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var model = await _dashboardService.GetMetricsAsync(
            cancellationToken);

        return View(model);
    }
}