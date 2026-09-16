
using Microsoft.EntityFrameworkCore;
using Vyzor.Application.DTO.AppointmentCart;
using Vyzor.Application.Interfaces;
using Vyzor.Domain.Entities;
using Vyzor.Domain.Enums;
using Vyzor.Infrastructure.Data;

namespace Vyzor.Infrastructure.Services;

public class AppointmentCartService : IAppointmentCartService
{
    private readonly AppDbContext _context;
    private readonly ISubscriptionService _subscriptionService;

    public AppointmentCartService(
        AppDbContext context,
        ISubscriptionService subscriptionService)
    {
        _context = context;
        _subscriptionService = subscriptionService;
    }


   
public async Task<IReadOnlyList<AppointmentCartItemDTO>> GetItemsAsync(
    string userId,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Array.Empty<AppointmentCartItemDTO>();

        return await _context.AppointmentCartItems
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.AppointmentDate)
            .Select(x => new AppointmentCartItemDTO
            {
                Id = x.Id,
                DoctorId = x.DoctorId,
                DoctorName = x.Doctor != null
                    ? x.Doctor.FullName
                    : string.Empty,
                DoctorImageUrl = x.Doctor != null
                    ? x.Doctor.ImageUrl
                    : null,
                SpecializationName =
                    x.Doctor != null &&
                    x.Doctor.Specialization != null
                        ? x.Doctor.Specialization.Name
                        : string.Empty,
                AppointmentDate = x.AppointmentDate,
                Price = x.Price
            })
            .ToListAsync(cancellationToken);
    }





    public async Task<AppointmentCartItemDTO?> AddAsync(
        string userId,
        int doctorId,
        DateTime appointmentDate,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return null;

        if (doctorId <= 0)
            return null;

        var doctor = await _context.Doctors
            .AsNoTracking()
            .Include(x => x.Specialization)
            .FirstOrDefaultAsync(
                x =>
                    x.Id == doctorId &&
                    x.IsActive,
                cancellationToken);

        if (doctor == null)
            return null;


        // Проверяем дубликат.
        var alreadyExists =
            await _context.AppointmentCartItems
                .AnyAsync(
                    x =>
                        x.UserId == userId &&
                        x.DoctorId == doctorId &&
                        x.AppointmentDate == appointmentDate,
                    cancellationToken);

        if (alreadyExists)
            return null;


        var cartItem = new AppointmentCartItem
        {
            UserId = userId,

            DoctorId = doctorId,

            AppointmentDate = appointmentDate,

            Price = doctor.AppointmentPrice,

            CreatedAtUtc = DateTime.UtcNow
        };

        _context.AppointmentCartItems.Add(cartItem);

        await _context.SaveChangesAsync(
            cancellationToken);


        return new AppointmentCartItemDTO
        {
            Id = cartItem.Id,

            DoctorId = doctor.Id,

            DoctorName = doctor.FullName,

            DoctorImageUrl = doctor.ImageUrl,

            SpecializationName =
                doctor.Specialization?.Name
                ?? string.Empty,

            AppointmentDate =
                cartItem.AppointmentDate,

            Price = cartItem.Price
        };
    }


    public async Task<bool> RemoveAsync(
        string userId,
        int cartItemId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return false;

        if (cartItemId <= 0)
            return false;



        var cartItem =
            await _context.AppointmentCartItems
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == cartItemId &&
                        x.UserId == userId,
                    cancellationToken);

        if (cartItem == null)
            return false;


        _context.AppointmentCartItems.Remove(
            cartItem);

        await _context.SaveChangesAsync(
            cancellationToken);

        return true;
    }



    public async Task<int> GetCountAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return 0;

        return await _context.AppointmentCartItems
            .CountAsync(
                x => x.UserId == userId,
                cancellationToken);
    }



    
public async Task<AppointmentCartSummaryDTO> GetSummaryAsync(
    string userId,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return new AppointmentCartSummaryDTO();
        }

        var items = await _context.AppointmentCartItems
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.Price)
            .ToListAsync(cancellationToken);

        var subTotal = items.Sum();

        var subscription =
            await _subscriptionService.GetCurrentUserSubscriptionAsync(
                userId,
                cancellationToken);

        var discountPercent =
            subscription?.DiscountPercent ?? 0m;

        if (discountPercent < 0m)
            discountPercent = 0m;

        if (discountPercent > 100m)
            discountPercent = 100m;

        var discountAmount =
            Math.Round(
                subTotal * discountPercent / 100m,
                2,
                MidpointRounding.AwayFromZero);

        var total =
            Math.Round(
                subTotal - discountAmount,
                2,
                MidpointRounding.AwayFromZero);

        return new AppointmentCartSummaryDTO
        {
            ItemsCount = items.Count,

            SubTotal = subTotal,

            DiscountPercent = discountPercent,

            DiscountAmount = discountAmount,

            Total = total,

            SubscriptionName = subscription?.PlanName
        };
    }



  

    public async Task<AppointmentCartPaymentResult> PayAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return new AppointmentCartPaymentResult
            {
                Success = false,
                Message = "User was not found."
            };
        }



        var patient = await _context.Patients
            .FirstOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken);

        if (patient == null)
        {
            return new AppointmentCartPaymentResult
            {
                Success = false,
                Message = "Patient profile was not found."
            };
        }



        var cartItems =
            await _context.AppointmentCartItems
                .Include(x => x.Doctor)
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.AppointmentDate)
                .ToListAsync(cancellationToken);


        if (cartItems.Count == 0)
        {
            return new AppointmentCartPaymentResult
            {
                Success = false,
                Message = "Your appointment cart is empty."
            };
        }


   

        var subscription =
            await _subscriptionService
                .GetCurrentUserSubscriptionAsync(
                    userId,
                    cancellationToken);


        var discountPercent =
            subscription?.DiscountPercent ?? 0m;


        if (discountPercent < 0m)
            discountPercent = 0m;

        if (discountPercent > 100m)
            discountPercent = 100m;


        

        var subtotal =
            cartItems.Sum(x => x.Price);


        var discountAmount =
            Math.Round(
                subtotal * discountPercent / 100m,
                2,
                MidpointRounding.AwayFromZero);


        var total =
            Math.Round(
                subtotal - discountAmount,
                2,
                MidpointRounding.AwayFromZero);


        

        await using var transaction =
            await _context.Database
                .BeginTransactionAsync(
                    cancellationToken);


        try
        {
   


            foreach (var cartItem in cartItems)
            {
                if (cartItem.Doctor == null)
                {
                    throw new InvalidOperationException(
                        "Doctor for appointment was not found.");
                }


                var price =
                    cartItem.Price;


                // Скидка для конкретной записи.
                var itemDiscountAmount =
                    Math.Round(
                        price * discountPercent / 100m,
                        2,
                        MidpointRounding.AwayFromZero);


                var finalPrice =
                    Math.Round(
                        price - itemDiscountAmount,
                        2,
                        MidpointRounding.AwayFromZero);


        
                var appointment = new Appointment
                {
                    PatientId = patient.Id,

                    PatientName =
                        patient.FullName,

                    DoctorId =
                        cartItem.DoctorId,

                    DoctorName =
                        cartItem.Doctor.FullName,

                    AppointmentDate =
                        DateTime.SpecifyKind(
                            cartItem.AppointmentDate,
                            DateTimeKind.Utc),

                    Status =
                        AppointmentStatus.Scheduled,

                    Price =
                        price,

                    DiscountPercent =
                        discountPercent,

                    FinalPrice =
                        finalPrice
                };


                _context.Appointments.Add(
                    appointment);
            }


       

            _context.AppointmentCartItems
                .RemoveRange(cartItems);


     
            await _context.SaveChangesAsync(
                cancellationToken);



            await transaction.CommitAsync(
                cancellationToken);


            return new AppointmentCartPaymentResult
            {
                Success = true,

                Message =
                    "Appointments were successfully booked.",

                Count =
                    cartItems.Count,

                Subtotal =
                    subtotal,

                Discount =
                    discountAmount,

                Total =
                    total
            };
        }
        catch
        {
            

            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }
}

