
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vyzor.Infrastructure.Interfaces;

namespace Vyzor.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class TechnicalLogsController : Controller
{
    private readonly ITechnicalLogService _technicalLogService;

    public TechnicalLogsController(
        ITechnicalLogService technicalLogService)
    {
        _technicalLogService = technicalLogService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var logs = await _technicalLogService.GetAllAsync(
            cancellationToken);

        return View(logs);
    }
}

