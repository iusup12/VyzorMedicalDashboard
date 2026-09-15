
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vyzor.Application.DTO.Specialization;
using Vyzor.Application.Interfaces;

namespace Vyzor.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SpecializationsController : Controller
{
    private readonly ISpecializationService _specializationService;

    public SpecializationsController(
        ISpecializationService specializationService)
    {
        _specializationService = specializationService;
    }


    // =========================================================
    // INDEX
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var specializations =
            await _specializationService.GetAllAsync();

        return View(specializations);
    }


    // =========================================================
    // CREATE - GET
    // =========================================================

    [HttpGet]
    public IActionResult Create()
    {
        var model = new SpecializationEditDTO
        {
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
        SpecializationEditDTO model)
    {
        if (!ModelState.IsValid)
        {
            foreach (var item in ModelState)
            {
                foreach (var error in item.Value.Errors)
                {
                    Console.WriteLine(
                        $"ModelState error: {item.Key} - {error.ErrorMessage}"
                    );
                }
            }

            return View(model);
        }

        await _specializationService.CreateAsync(model);

        TempData["SuccessMessage"] =
            "Specialization created successfully.";

        return RedirectToAction(nameof(Index));
    }


    // =========================================================
    // EDIT - GET
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var specialization =
            await _specializationService.GetByIdAsync(id);

        if (specialization == null)
        {
            return NotFound();
        }

        return View(specialization);
    }


    // =========================================================
    // EDIT - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        SpecializationEditDTO model)
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
            foreach (var item in ModelState)
            {
                foreach (var error in item.Value.Errors)
                {
                    Console.WriteLine(
                        $"ModelState error: {item.Key} - {error.ErrorMessage}"
                    );
                }
            }

            return View(model);
        }

        var specialization =
            await _specializationService.GetByIdAsync(id);

        if (specialization == null)
        {
            return NotFound();
        }

        await _specializationService.UpdateAsync(model);

        TempData["SuccessMessage"] =
            "Specialization updated successfully.";

        return RedirectToAction(nameof(Index));
    }


    // =========================================================
    // DELETE
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        await _specializationService.DeleteAsync(id);

        TempData["SuccessMessage"] =
            "Specialization deleted successfully.";

        return RedirectToAction(nameof(Index));
    }
}

