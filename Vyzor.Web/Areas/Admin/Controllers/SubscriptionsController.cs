
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Numerics;
using Vyzor.Application.Common;
using Vyzor.Application.DTO.Subscription;
using Vyzor.Application.Interfaces;
using Vyzor.Infrastructure.Services;
using Vyzor.Web.Areas.Admin.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Vyzor.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SubscriptionsController : Controller
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(
        ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }


    // =========================================================
    // INDEX
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Index(
        PagedRequest request,
        CancellationToken cancellationToken)
    {
        var plans =
            await _subscriptionService.GetPlansForAdminAsync(
                request,
                cancellationToken);

        var model = new AdminSubscriptionPlansIndexViewModel
        {
            Request = request,
            Plans = plans
        };

        return View(model);
    }


    // =========================================================
    // CREATE - GET
    // =========================================================

    [HttpGet]
    public IActionResult Create()
    {
        var model = new SubscriptionPlanDTO
        {
            DurationDays = 30,
            DiscountPercent = 0,
            Price = 0,
            IsActive = true
        };

        return View(model);
    }


    // =========================================================
    // CREATE - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        SubscriptionPlanDTO model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var userId =
                User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier
                )?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Forbid();
            }

            var id =
                await _subscriptionService.CreatePlanAsync(
                    model,
                    userId,
                    cancellationToken);

            TempData["SuccessMessage"] =
                "Subscription plan created successfully.";

            return RedirectToAction(
                nameof(Edit),
                new { id });
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(
                string.Empty,
                exception.Message);

            return View(model);
        }
    }


    // =========================================================
    // EDIT - GET
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var plan =
            await _subscriptionService.GetPlanAsync(
                id,
                cancellationToken);

        if (plan == null)
        {
            return NotFound();
        }

        return View(plan);
    }


    // =========================================================
    // EDIT - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        SubscriptionPlanDTO model,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var userId =
                User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier
                )?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Forbid();
            }

            await _subscriptionService.UpdatePlanAsync(
                model,
                userId,
                cancellationToken);

            TempData["SuccessMessage"] =
                "Subscription plan updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(
                string.Empty,
                exception.Message);

            return View(model);
        }
    }


    // =========================================================
    // DELETE / DISABLE
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var userId =
            User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier
            )?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Forbid();
        }

        await _subscriptionService.DeletePlanAsync(
            id,
            userId,
            cancellationToken);

        TempData["SuccessMessage"] =
            "Subscription plan disabled successfully.";

        return RedirectToAction(nameof(Index));
    }


    // =========================================================
    // ACTIVATE
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var userId =
            User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier
            )?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Forbid();
        }

        await _subscriptionService.ActivatePlanAsync(
            id,
            userId,
            cancellationToken);

        TempData["SuccessMessage"] =
            "Subscription plan activated successfully.";

        return RedirectToAction(nameof(Index));
    }
}

    