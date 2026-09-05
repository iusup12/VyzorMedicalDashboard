using Microsoft.AspNetCore.Mvc;
using Vyzor.Application.DTO.Appointment;
using Vyzor.Application.Interfaces;

namespace Vyzor.Web.Controllers;

public class AppointmentController : Controller
{
    private readonly IAppointmentService _appointmentService;
    private readonly IDoctorService _doctorService;

    public AppointmentController(
        IAppointmentService appointmentService,
        IDoctorService doctorService)
    {
        _appointmentService = appointmentService;
        _doctorService = doctorService;
    }


    // =========================================================
    // BOOKING PAGE
    // GET: /Appointment/Create?doctorId=1
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Create(int doctorId)
    {
        if (doctorId <= 0)
        {
            return NotFound();
        }


        var doctor = await _doctorService.GetDetailsAsync(doctorId);

        if (doctor == null)
        {
            return NotFound();
        }


        var model = new AppointmentEditDTO
        {
            DoctorId = doctorId,
            AppointmentDate = DateTime.Now.AddDays(1)
        };


        ViewBag.Doctor = doctor;

        return View(model);
    }



    // =========================================================
    // CREATE APPOINTMENT
    // POST: /Appointment/Create
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        AppointmentEditDTO dto)
    {
        if (!ModelState.IsValid)
        {
            var doctor = await _doctorService.GetDetailsAsync(dto.DoctorId);

            ViewBag.Doctor = doctor;

            return View(dto);
        }


        await _appointmentService.CreateAsync(dto);


        return RedirectToAction(
            nameof(Confirmation));
    }



    // =========================================================
    // CONFIRMATION PAGE
    // GET: /Appointment/Confirmation
    // =========================================================

    [HttpGet]
    public IActionResult Confirmation()
    {
        return View();
    }



    // =========================================================
    // DETAILS
    // GET: /Appointment/Details/1
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
        {
            return NotFound();
        }


        var appointment =
            await _appointmentService.GetByIdAsync(id);


        if (appointment == null)
        {
            return NotFound();
        }


        return View(appointment);
    }



    // =========================================================
    // USER APPOINTMENTS
    // GET: /Appointment/MyAppointments
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> MyAppointments(
        string userId)
    {
        var appointments =
            await _appointmentService
                .GetUserAppointmentsAsync(userId);


        return View(appointments);
    }



    // =========================================================
    // CANCEL / DELETE
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _appointmentService.DeleteAsync(id);


        return RedirectToAction(
            nameof(MyAppointments));
    }
}