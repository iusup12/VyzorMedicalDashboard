
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vyzor.Application.DTO.Appointment;
using Vyzor.Application.Interfaces;
using Vyzor.Domain.Enums;
using Vyzor.Infrastructure.Data;

namespace Vyzor.Web.Controllers;

[Authorize]
public class AppointmentController : Controller
{
    private readonly IAppointmentService _appointmentService;
    private readonly IDoctorService _doctorService;
    private readonly AppDbContext _context;

    public AppointmentController(
        IAppointmentService appointmentService,
        IDoctorService doctorService,
        AppDbContext context)
    {
        _appointmentService = appointmentService;
        _doctorService = doctorService;
        _context = context;
    }

    
    [HttpGet]
    public async Task<IActionResult> Create(
        int doctorId,
        CancellationToken cancellationToken)
    {
        if (doctorId <= 0)
        {
            return BadRequest();
        }

        var doctor = await _doctorService.GetDetailsAsync(doctorId);

        if (doctor == null)
        {
            return NotFound();
        }

        ViewBag.Doctor = doctor;

        var model = new AppointmentEditDTO
        {
            DoctorId = doctorId,
            AppointmentDate = DateTime.Now,
            Status = AppointmentStatus.Scheduled
        };

        return View(model);
    }

    // POST: /Appointment/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        AppointmentEditDTO model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var doctor = await _doctorService.GetDetailsAsync(
                model.DoctorId);

            ViewBag.Doctor = doctor;

            return View(model);
        }

        // Получаем ID текущего авторизованного пользователя
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        // Находим пациента, связанного с текущим пользователем
        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken);

        if (patient == null)
        {
            ModelState.AddModelError(
                string.Empty,
                "Профиль пациента не найден.");

            var doctor = await _doctorService.GetDetailsAsync(
                model.DoctorId);

            ViewBag.Doctor = doctor;

            return View(model);
        }

  
        model.PatientId = patient.Id;

        model.Status = AppointmentStatus.Scheduled;

        model.AppointmentDate = DateTime.SpecifyKind(
            model.AppointmentDate,
            DateTimeKind.Local);

        model.AppointmentDate =
            model.AppointmentDate.ToUniversalTime();

        await _appointmentService.CreateAsync(
            model,
            userId,
            cancellationToken);

        return RedirectToAction(
            "Index",
            "Profile");
    }
}

