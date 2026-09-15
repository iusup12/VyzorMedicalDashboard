
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Vyzor.Application.DTO.Appointment;
using Vyzor.Application.DTO.Filters;
using Vyzor.Application.Interfaces;
using Vyzor.Domain.Enums;
using Vyzor.Infrastructure.Data;
using Vyzor.Web.Authorization;

namespace Vyzor.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AppPolicies.DoctorOrAdmin)]
public class AppointmentsController : Controller
{
    private readonly IAppointmentService _appointmentService;
    private readonly AppDbContext _context;

    public AppointmentsController(
        IAppointmentService appointmentService,
        AppDbContext context)
    {
        _appointmentService = appointmentService;
        _context = context;
    }


    // =========================================================
    // INDEX
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Index(
        AppointmentFilterDTO filter,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.GetPagedAsync(
            filter,
            cancellationToken);

        return View(result);
    }


    // =========================================================
    // CREATE - GET
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Create(
        CancellationToken cancellationToken)
    {
        var model = new AppointmentEditDTO
        {
            AppointmentDate = DateTime.Now,
            Status = AppointmentStatus.Scheduled
        };

        await LoadEditListsAsync(
            model.DoctorId,
            model.PatientId,
            cancellationToken);

        return View(model);
    }


    // =========================================================
    // CREATE - POST
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        AppointmentEditDTO model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadEditListsAsync(
                model.DoctorId,
                model.PatientId,
                cancellationToken);

            return View(model);
        }

        try
        {
            // Проверяем, что выбранный врач существует
            // и активен.
            var doctorExists = await _context.Doctors
                .AnyAsync(
                    x => x.Id == model.DoctorId &&
                         x.IsActive,
                    cancellationToken);

            if (!doctorExists)
            {
                ModelState.AddModelError(
                    nameof(model.DoctorId),
                    "Selected doctor does not exist or is inactive.");

                await LoadEditListsAsync(
                    model.DoctorId,
                    model.PatientId,
                    cancellationToken);

                return View(model);
            }


            // Проверяем пациента.
            var patientExists = await _context.Patients
                .AnyAsync(
                    x => x.Id == model.PatientId,
                    cancellationToken);

            if (!patientExists)
            {
                ModelState.AddModelError(
                    nameof(model.PatientId),
                    "Selected patient does not exist.");

                await LoadEditListsAsync(
                    model.DoctorId,
                    model.PatientId,
                    cancellationToken);

                return View(model);
            }


            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);


            await _appointmentService.CreateAsync(
                model,
                userId,
                cancellationToken);


            return RedirectToAction(
                nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            await LoadEditListsAsync(
                model.DoctorId,
                model.PatientId,
                cancellationToken);

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
            return BadRequest();

        var appointment = await _appointmentService.GetForEditAsync(
            id,
            cancellationToken);

        if (appointment == null)
            return NotFound();

        Console.WriteLine("========== APPOINTMENT EDIT GET ==========");
        Console.WriteLine($"Id: {appointment.Id}");
        Console.WriteLine($"DoctorId: {appointment.DoctorId}");
        Console.WriteLine($"PatientId: {appointment.PatientId}");
        Console.WriteLine($"AppointmentDate: {appointment.AppointmentDate:O}");
        Console.WriteLine($"Status: {appointment.Status}");
        Console.WriteLine($"About: {appointment.About}");
        Console.WriteLine("==========================================");

        await LoadEditListsAsync(
            appointment.DoctorId,
            appointment.PatientId,
            cancellationToken);

        return View(appointment);
    }



    // =========================================================
    // EDIT - POST
    // =========================================================

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(
    int id,
    AppointmentEditDTO model,
    CancellationToken cancellationToken)
    {
        // ВРЕМЕННАЯ ДИАГНОСТИКА
        Console.WriteLine("========== APPOINTMENT EDIT ==========");
        Console.WriteLine($"Route id: {id}");
        Console.WriteLine($"Model.Id: {model.Id}");
        Console.WriteLine($"DoctorId: {model.DoctorId}");
        Console.WriteLine($"PatientId: {model.PatientId}");
        Console.WriteLine($"AppointmentDate: {model.AppointmentDate:O}");
        Console.WriteLine($"Status: {model.Status}");
        Console.WriteLine($"About: {model.About}");
        Console.WriteLine("======================================");

        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            await LoadEditListsAsync(
                model.DoctorId,
                model.PatientId,
                cancellationToken);

            return View(model);
        }

        try
        {
            var doctorExists = await _context.Doctors
                .AnyAsync(
                    x => x.Id == model.DoctorId &&
                         x.IsActive,
                    cancellationToken);

            if (!doctorExists)
            {
                ModelState.AddModelError(
                    nameof(model.DoctorId),
                    "Selected doctor does not exist or is inactive.");

                await LoadEditListsAsync(
                    model.DoctorId,
                    model.PatientId,
                    cancellationToken);

                return View(model);
            }

            var patientExists = await _context.Patients
                .AnyAsync(
                    x => x.Id == model.PatientId,
                    cancellationToken);

            if (!patientExists)
            {
                ModelState.AddModelError(
                    nameof(model.PatientId),
                    "Selected patient does not exist.");

                await LoadEditListsAsync(
                    model.DoctorId,
                    model.PatientId,
                    cancellationToken);

                return View(model);
            }

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            await _appointmentService.UpdateAsync(
                model,
                userId,
                cancellationToken);

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            await LoadEditListsAsync(
                model.DoctorId,
                model.PatientId,
                cancellationToken);

            return View(model);
        }
    }




    // =========================================================
    // CHANGE STATUS - AJAX
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(
        [FromBody] AppointmentStatusDTO dto,
        CancellationToken cancellationToken)
    {
        if (dto == null ||
            dto.AppointmentId <= 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid appointment data."
            });
        }


        try
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);


            await _appointmentService.ChangeStatusAsync(
                dto,
                userId,
                cancellationToken);


            return Ok(new
            {
                success = true
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    success = false,
                    message = ex.Message
                });
        }
    }


    // =========================================================
    // DELETE
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
            return BadRequest();


        try
        {
            await _appointmentService.DeleteAsync(
                id,
                cancellationToken);


            return RedirectToAction(
                nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;

            return RedirectToAction(
                nameof(Index));
        }
    }


    // =========================================================
    // LOAD DOCTORS + PATIENTS
    // =========================================================

    private async Task LoadEditListsAsync(
        int selectedDoctorId,
        int selectedPatientId,
        CancellationToken cancellationToken)
    {
        // -----------------------------------------------------
        // Doctors
        // -----------------------------------------------------

        var doctors = await _context.Doctors
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.FullName)
            .Select(x => new
            {
                x.Id,
                x.FullName
            })
            .ToListAsync(cancellationToken);


        ViewBag.Doctors = doctors
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.FullName} (ID: {x.Id})",
                Selected = x.Id == selectedDoctorId
            })
            .ToList();


        // -----------------------------------------------------
        // Patients
        // -----------------------------------------------------

        var patients = await _context.Patients
            .AsNoTracking()
            .OrderBy(x => x.FullName)
            .Select(x => new
            {
                x.Id,
                x.FullName
            })
            .ToListAsync(cancellationToken);


        ViewBag.Patients = patients
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.FullName} (ID: {x.Id})",
                Selected = x.Id == selectedPatientId
            })
            .ToList();
    }
}

