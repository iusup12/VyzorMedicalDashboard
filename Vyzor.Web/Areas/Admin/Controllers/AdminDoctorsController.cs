
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vyzor.Application.DTO.Doctor;
using Vyzor.Application.DTO.Filters;
using Vyzor.Application.Interfaces;
using Vyzor.Web.Authorization;

namespace Vyzor.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AppPolicies.DoctorOrAdmin)]
public class DoctorsController : Controller
{
    private readonly IDoctorService _doctorService;
    private readonly ISpecializationService _specializationService;

    public DoctorsController(
        IDoctorService doctorService,
        ISpecializationService specializationService)
    {
        _doctorService = doctorService;
        _specializationService = specializationService;
    }

    // ============================================================
    // INDEX
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Index(
        [FromQuery] AdminDoctorFilterDTO filter,
        CancellationToken cancellationToken)
    {
        filter.Page = filter.Page <= 0
            ? 1
            : filter.Page;

        var doctors = await _doctorService.GetPagedAsync(
            filter,
            cancellationToken);

        return View(doctors);
    }


    // ============================================================
    // CREATE - GET
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadSpecializations();

        return View(new DoctorEditDTO());
    }


    // ============================================================
    // CREATE - POST
    // ============================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        DoctorEditDTO model,
        CancellationToken cancellationToken)
    {
        // --------------------------------------------------------
        // DEBUG
        // Проверяем, действительно ли POST доходит сюда
        // --------------------------------------------------------

        Console.WriteLine();
        Console.WriteLine("======================================");
        Console.WriteLine("CREATE DOCTOR POST CALLED");
        Console.WriteLine("======================================");

        Console.WriteLine($"Id: {model.Id}");
        Console.WriteLine($"FullName: {model.FullName}");
        Console.WriteLine($"SpecializationId: {model.SpecializationId}");
        Console.WriteLine($"ExperienceYears: {model.ExperienceYears}");
        Console.WriteLine($"AppointmentPrice: {model.AppointmentPrice}");
        Console.WriteLine($"About: {model.About}");
        Console.WriteLine($"ImageUrl: {model.ImageUrl}");

        Console.WriteLine("--------------------------------------");
        Console.WriteLine("MODEL STATE:");
        Console.WriteLine("--------------------------------------");

        foreach (var state in ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                Console.WriteLine(
                    $"ERROR: {state.Key} -> {error.ErrorMessage}");

                if (error.Exception != null)
                {
                    Console.WriteLine(
                        $"EXCEPTION: {error.Exception.Message}");
                }
            }
        }

        Console.WriteLine("--------------------------------------");
        Console.WriteLine(
            $"ModelState.IsValid: {ModelState.IsValid}");
        Console.WriteLine("--------------------------------------");


        // --------------------------------------------------------
        // VALIDATION
        // --------------------------------------------------------

        if (!ModelState.IsValid)
        {
            await LoadSpecializations();

            return View(model);
        }


        // --------------------------------------------------------
        // CREATE
        // --------------------------------------------------------

        try
        {
            Console.WriteLine();
            Console.WriteLine(
                "CALLING _doctorService.CreateAsync()...");

            await _doctorService.CreateAsync(
                model,
                cancellationToken);

            Console.WriteLine(
                "_doctorService.CreateAsync() FINISHED.");

            Console.WriteLine(
                "DOCTOR SHOULD NOW BE IN DATABASE.");

            Console.WriteLine(
                "======================================");
            Console.WriteLine();


            TempData["SuccessMessage"] =
                "Doctor created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // ----------------------------------------------------
            // ERROR
            // ----------------------------------------------------

            Console.WriteLine();
            Console.WriteLine("======================================");
            Console.WriteLine("CREATE DOCTOR ERROR");
            Console.WriteLine("======================================");

            Console.WriteLine(ex.ToString());

            Console.WriteLine("======================================");
            Console.WriteLine();


            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            await LoadSpecializations();

            return View(model);
        }
    }


    // ============================================================
    // DETAILS
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Details(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return RedirectToAction(nameof(Index));
        }

        var doctor = await _doctorService.GetDetailsAsync(
            id,
            cancellationToken);

        if (doctor == null)
        {
            return NotFound();
        }

        return View(doctor);
    }


    // ============================================================
    // EDIT - GET
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return RedirectToAction(nameof(Index));
        }

        var doctor = await _doctorService.GetForEditAsync(
            id,
            cancellationToken);

        if (doctor == null)
        {
            return NotFound();
        }

        await LoadSpecializations();

        return View(doctor);
    }


    // ============================================================
    // EDIT - POST
    // ============================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        DoctorEditDTO model,
        CancellationToken cancellationToken)
    {
        // --------------------------------------------------------
        // DEBUG
        // --------------------------------------------------------

        Console.WriteLine();
        Console.WriteLine("======================================");
        Console.WriteLine("EDIT DOCTOR POST CALLED");
        Console.WriteLine("======================================");

        Console.WriteLine($"Id: {model.Id}");
        Console.WriteLine($"FullName: {model.FullName}");
        Console.WriteLine($"SpecializationId: {model.SpecializationId}");
        Console.WriteLine($"ExperienceYears: {model.ExperienceYears}");
        Console.WriteLine($"AppointmentPrice: {model.AppointmentPrice}");

        Console.WriteLine("--------------------------------------");
        Console.WriteLine(
            $"ModelState.IsValid: {ModelState.IsValid}");
        Console.WriteLine("--------------------------------------");


        // --------------------------------------------------------
        // VALIDATION
        // --------------------------------------------------------

        if (!ModelState.IsValid)
        {
            await LoadSpecializations();

            return View(model);
        }


        // --------------------------------------------------------
        // UPDATE
        // --------------------------------------------------------

        try
        {
            await _doctorService.UpdateAsync(
                model,
                cancellationToken);

            TempData["SuccessMessage"] =
                "Doctor updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("======================================");
            Console.WriteLine("EDIT DOCTOR ERROR");
            Console.WriteLine("======================================");

            Console.WriteLine(ex.ToString());

            Console.WriteLine("======================================");
            Console.WriteLine();

            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            await LoadSpecializations();

            return View(model);
        }
    }


    // ============================================================
    // DELETE
    // ============================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
     int id,
     CancellationToken cancellationToken)
    {
        if (id <= 0)
            return RedirectToAction(nameof(Index));

        try
        {
            await _doctorService.DeleteAsync(
                id,
                cancellationToken);

            TempData["SuccessMessage"] =
                "Doctor deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] =
                $"Unable to delete doctor: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }


    // ============================================================
    // SPECIALIZATIONS
    // ============================================================

    private async Task LoadSpecializations()
    {
        var specializations =
            await _specializationService.GetAllAsync();

        ViewBag.Specializations =
            specializations
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToList();
    }
}

