
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vyzor.Application.Interfaces;

namespace Vyzor.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Patient")]
public class AppointmentCartController : Controller
{
    private readonly IAppointmentCartService _appointmentCartService;

    public AppointmentCartController(
        IAppointmentCartService appointmentCartService)
    {
        _appointmentCartService = appointmentCartService;
    }

    // =========================================================
    // CART PAGE
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId == null)
            return Challenge();

        var items = await _appointmentCartService.GetItemsAsync(
            userId,
            cancellationToken);

        var summary = await _appointmentCartService.GetSummaryAsync(
            userId,
            cancellationToken);

        ViewBag.CartSummary = summary;

        return View(items);
    }


    // =========================================================
    // ADD TO CART
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(
        int doctorId,
        DateTime appointmentDate,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "Please log in."
            });
        }

        if (doctorId <= 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid doctor."
            });
        }

        // datetime-local приходит как Unspecified.
        // Сначала считаем его локальным временем пользователя,
        // затем переводим в UTC для PostgreSQL.
        if (appointmentDate <= DateTime.Now)
        {
            return BadRequest(new
            {
                success = false,
                message = "Appointment date must be in the future."
            });
        }

        appointmentDate = DateTime.SpecifyKind(
            appointmentDate,
            DateTimeKind.Local);

        appointmentDate = appointmentDate.ToUniversalTime();

        var item = await _appointmentCartService.AddAsync(
            userId,
            doctorId,
            appointmentDate,
            cancellationToken);

        if (item == null)
        {
            return Conflict(new
            {
                success = false,
                message = "This appointment is already in your cart."
            });
        }

        var count = await _appointmentCartService.GetCountAsync(
            userId,
            cancellationToken);

        var summary = await _appointmentCartService.GetSummaryAsync(
            userId,
            cancellationToken);

        return Json(new
        {
            success = true,
            message = "Appointment added to cart.",

            item = new
            {
                id = item.Id,
                doctorId = item.DoctorId,
                doctorName = item.DoctorName,
                doctorImageUrl = item.DoctorImageUrl,
                specializationName = item.SpecializationName,
                appointmentDate = item.AppointmentDate,
                price = item.Price
            },

            count,

            summary = new
            {
                itemsCount = summary.ItemsCount,
                subTotal = summary.SubTotal,
                discountPercent = summary.DiscountPercent,
                discountAmount = summary.DiscountAmount,
                total = summary.Total,
                subscriptionName = summary.SubscriptionName
            }
        });
    }


    // =========================================================
    // REMOVE FROM CART
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(
        int id,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "Please log in."
            });
        }

        if (id <= 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid cart item."
            });
        }

        var removed = await _appointmentCartService.RemoveAsync(
            userId,
            id,
            cancellationToken);

        if (!removed)
        {
            return NotFound(new
            {
                success = false,
                message = "Appointment was not found."
            });
        }

        var count = await _appointmentCartService.GetCountAsync(
            userId,
            cancellationToken);

        var summary = await _appointmentCartService.GetSummaryAsync(
            userId,
            cancellationToken);

        return Json(new
        {
            success = true,
            message = "Appointment removed from cart.",

            id,
            count,

            summary = new
            {
                itemsCount = summary.ItemsCount,
                subTotal = summary.SubTotal,
                discountPercent = summary.DiscountPercent,
                discountAmount = summary.DiscountAmount,
                total = summary.Total,
                subscriptionName = summary.SubscriptionName
            }
        });
    }


    // =========================================================
    // CART COUNT
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Count(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId == null)
        {
            return Json(new
            {
                count = 0
            });
        }

        var count = await _appointmentCartService.GetCountAsync(
            userId,
            cancellationToken);

        return Json(new
        {
            count
        });
    }


    // =========================================================
    // CART SUMMARY
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Summary(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "Please log in."
            });
        }

        var summary = await _appointmentCartService.GetSummaryAsync(
            userId,
            cancellationToken);

        return Json(new
        {
            success = true,

            itemsCount = summary.ItemsCount,
            subTotal = summary.SubTotal,
            discountPercent = summary.DiscountPercent,
            discountAmount = summary.DiscountAmount,
            total = summary.Total,
            subscriptionName = summary.SubscriptionName
        });
    }


    // =========================================================
    // PAY & BOOK
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (userId == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "Please log in."
            });
        }

        try
        {
            var result = await _appointmentCartService.PayAsync(
                userId,
                cancellationToken);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.Message
                });
            }

            return Json(new
            {
                success = true,
                message = result.Message,
                count = result.Count,
                subtotal = result.Subtotal,
                discount = result.Discount,
                total = result.Total
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                success = false,
                message = exception.Message
            });
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    success = false,
                    message = "An error occurred while processing the payment."
                });
        }
    }


    // =========================================================
    // USER ID
    // =========================================================

    private string? GetUserId()
    {
        return User.FindFirstValue(
            ClaimTypes.NameIdentifier);
    }
}

