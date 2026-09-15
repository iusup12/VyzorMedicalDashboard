
using Microsoft.EntityFrameworkCore;
using Vyzor.Application.Common;
using Vyzor.Application.DTO.Appointment;
using Vyzor.Application.DTO.Filters;
using Vyzor.Application.Interfaces;
using Vyzor.Domain.Entities;
using Vyzor.Domain.Enums;
using Vyzor.Infrastructure.Data;

namespace Vyzor.Infrastructure.Services;

public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _context;
    private readonly IAuditService _auditService;

    public AppointmentService(
        AppDbContext context,
        IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    
public async Task<PagedResult<AppointmentListItemDTO>> GetPagedAsync(
    AppointmentFilterDTO filter,
    CancellationToken cancellationToken = default)
    {
        var query = _context.Appointments
            .AsNoTracking()
            .AsQueryable();

        // Doctor filter
        if (filter.DoctorId.HasValue)
        {
            query = query.Where(x =>
                x.DoctorId == filter.DoctorId.Value);
        }

        // Patient filter
        if (filter.PatientId.HasValue)
        {
            query = query.Where(x =>
                x.PatientId == filter.PatientId.Value);
        }

        // Status filter
        if (filter.Status.HasValue)
        {
            query = query.Where(x =>
                x.Status == filter.Status.Value);
        }

        // From date
        if (filter.FromDate.HasValue)
        {
            query = query.Where(x =>
                x.AppointmentDate >= filter.FromDate.Value);
        }

        // To date
        if (filter.ToDate.HasValue)
        {
            query = query.Where(x =>
                x.AppointmentDate <= filter.ToDate.Value);
        }

        // Total records after filters
        var totalCount = await query
            .CountAsync(cancellationToken);

        // Pagination
        var pageNumber = Math.Max(1, filter.PageNumber);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        // Data
        var items = await query
            .OrderByDescending(x => x.AppointmentDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AppointmentListItemDTO
            {
                Id = x.Id,

                DoctorId = x.DoctorId,

                PatientId = x.PatientId,

                PatientName = x.Patient != null
                    ? x.Patient.FullName
                    : "Unknown patient",

                DoctorName = x.Doctor != null
                    ? x.Doctor.FullName
                    : "Unknown doctor",

                AppointmentDate = x.AppointmentDate,

                Status = x.Status
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AppointmentListItemDTO>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }



    public async Task<AppointmentDetailsDTO?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AppointmentDetailsDTO
            {
                AppointmentId = x.Id,
                DoctorId = x.DoctorId,
                PatientId = x.PatientId,
                AppointmentDate = x.AppointmentDate,
                Status = x.Status,
                Price = x.Price,
                DiscountPercent = x.DiscountPercent,
                FinalPrice = x.FinalPrice
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

   
public async Task CreateAsync(
    AppointmentEditDTO dto,
    string? userId,
    CancellationToken cancellationToken = default)
    {
        var appointmentDateUtc = DateTime.SpecifyKind(
            dto.AppointmentDate,
            DateTimeKind.Local)
            .ToUniversalTime();

        var appointment = new Appointment
        {
            DoctorId = dto.DoctorId,
            PatientId = dto.PatientId,
            AppointmentDate = appointmentDateUtc,
            Status = dto.Status,
            About = dto.About
        };

        _context.Appointments.Add(appointment);

        await _context.SaveChangesAsync(
            cancellationToken);

        await _auditService.LogAsync(
            action: AuditAction.AppointmentCreated,
            entityName: nameof(Appointment),
            entityId: appointment.Id.ToString(),
            userId: userId,
            details:
                $"Appointment created. " +
                $"DoctorId={appointment.DoctorId}; " +
                $"PatientId={appointment.PatientId}; " +
                $"AppointmentDate={appointment.AppointmentDate:O}; " +
                $"Status={appointment.Status}",
            cancellationToken: cancellationToken);
    }


   

public async Task UpdateAsync(
    AppointmentEditDTO dto,
    string? userId,
    CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(
                x => x.Id == dto.Id,
                cancellationToken);

        if (appointment == null)
        {
            throw new InvalidOperationException(
                "Appointment not found.");
        }

        var doctorExists = await _context.Doctors
            .AnyAsync(
                x => x.Id == dto.DoctorId &&
                     x.IsActive,
                cancellationToken);

        if (!doctorExists)
        {
            throw new InvalidOperationException(
                "Selected doctor does not exist or is inactive.");
        }

        var patientExists = await _context.Patients
            .AnyAsync(
                x => x.Id == dto.PatientId,
                cancellationToken);

        if (!patientExists)
        {
            throw new InvalidOperationException(
                "Selected patient does not exist.");
        }

        // datetime-local приходит без timezone:
        // DateTimeKind.Unspecified.
        //
        // Считаем введённое время локальным временем пользователя
        // и переводим его в UTC перед сохранением в PostgreSQL.
        var appointmentDateUtc = DateTime.SpecifyKind(
            dto.AppointmentDate,
            DateTimeKind.Local)
            .ToUniversalTime();

        appointment.DoctorId = dto.DoctorId;
        appointment.PatientId = dto.PatientId;
        appointment.AppointmentDate = appointmentDateUtc;
        appointment.Status = dto.Status;
        appointment.About = dto.About;

        var changes = await _context.SaveChangesAsync(
            cancellationToken);

        Console.WriteLine("========== APPOINTMENT UPDATED ==========");
        Console.WriteLine($"Id: {appointment.Id}");
        Console.WriteLine($"DoctorId: {appointment.DoctorId}");
        Console.WriteLine($"PatientId: {appointment.PatientId}");
        Console.WriteLine($"AppointmentDate UTC: {appointment.AppointmentDate:O}");
        Console.WriteLine($"Status: {appointment.Status}");
        Console.WriteLine($"About: {appointment.About}");
        Console.WriteLine($"Changed entries: {changes}");
        Console.WriteLine("==========================================");

        await _auditService.LogAsync(
            action: AuditAction.AppointmentUpdated,
            entityName: nameof(Appointment),
            entityId: appointment.Id.ToString(),
            userId: userId,
            details:
                $"Appointment updated. " +
                $"DoctorId={appointment.DoctorId}; " +
                $"PatientId={appointment.PatientId}; " +
                $"AppointmentDate={appointment.AppointmentDate:O}; " +
                $"Status={appointment.Status}",
            cancellationToken: cancellationToken);
    }





    public async Task ChangeStatusAsync(
        AppointmentStatusDTO dto,
        string? userId,
        CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(
                x => x.Id == dto.AppointmentId,
                cancellationToken);

        if (appointment == null)
        {
            return;
        }

        var oldStatus = appointment.Status;

        appointment.Status = dto.Status;

        await _context.SaveChangesAsync(cancellationToken);

        if (dto.Status == AppointmentStatus.Cancelled)
        {
            await _auditService.LogAsync(
                action: AuditAction.AppointmentCancelled,
                entityName: nameof(Appointment),
                entityId: appointment.Id.ToString(),
                userId: userId,
                details:
                    $"Appointment cancelled. " +
                    $"PreviousStatus={oldStatus}; " +
                    $"DoctorId={appointment.DoctorId}; " +
                    $"PatientId={appointment.PatientId}; " +
                    $"AppointmentDate={appointment.AppointmentDate:O}",
                cancellationToken: cancellationToken);
        }
        else if (dto.Status == AppointmentStatus.Completed)
        {
            await _auditService.LogAsync(
                action: AuditAction.AppointmentCompleted,
                entityName: nameof(Appointment),
                entityId: appointment.Id.ToString(),
                userId: userId,
                details:
                    $"Appointment completed. " +
                    $"PreviousStatus={oldStatus}; " +
                    $"DoctorId={appointment.DoctorId}; " +
                    $"PatientId={appointment.PatientId}; " +
                    $"AppointmentDate={appointment.AppointmentDate:O}",
                cancellationToken: cancellationToken);
        }
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (appointment == null)
        {
            return;
        }

        _context.Appointments.Remove(appointment);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<AppointmentListItemDTO>> GetUserAppointmentsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken);

        if (patient == null)
        {
            return Enumerable.Empty<AppointmentListItemDTO>();
        }

        return await _context.Appointments
            .AsNoTracking()
            .Where(x => x.PatientId == patient.Id)
            .OrderByDescending(x => x.AppointmentDate)
            .Select(x => new AppointmentListItemDTO
            {
                Id = x.Id,
                DoctorId = x.DoctorId,
                PatientId = x.PatientId,
                AppointmentDate = x.AppointmentDate,
                Status = x.Status
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AppointmentListItemDTO>> GetDoctorAppointmentsAsync(
        int doctorId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Where(x => x.DoctorId == doctorId)
            .OrderByDescending(x => x.AppointmentDate)
            .Select(x => new AppointmentListItemDTO
            {
                Id = x.Id,
                DoctorId = x.DoctorId,
                PatientId = x.PatientId,
                AppointmentDate = x.AppointmentDate,
                Status = x.Status
            })
            .ToListAsync(cancellationToken);
    }
    
public async Task<AppointmentEditDTO?> GetForEditAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AppointmentEditDTO
            {
                Id = x.Id,
                DoctorId = x.DoctorId,
                PatientId = x.PatientId,
                AppointmentDate = x.AppointmentDate,
                Status = x.Status,
                About = x.About
            })
            .FirstOrDefaultAsync(cancellationToken);
    }


}

