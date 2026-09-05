using Vyzor.Application.Common;
using Microsoft.EntityFrameworkCore;
using Vyzor.Application.Common;
using Vyzor.Application.DTO.Appointment;
using Vyzor.Application.DTO.Filters;
using Vyzor.Application.Interfaces;
using Vyzor.Domain.Entities;
using Vyzor.Infrastructure.Data;

namespace Vyzor.Infrastructure.Services;

public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _context;

    public AppointmentService(AppDbContext context)
    {
        _context = context;
    }


    public async Task<PagedResult<AppointmentListItemDTO>> GetPagedAsync(
        AppointmentFilterDTO filter,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Appointments
            .AsQueryable();


        if (filter.DoctorId.HasValue)
        {
            query = query.Where(x =>
                x.DoctorId == filter.DoctorId.Value);
        }


        if (filter.PatientId.HasValue)
        {
            query = query.Where(x =>
                x.PatientId == filter.PatientId.Value);
        }


        if (filter.Status.HasValue)
        {
            query = query.Where(x =>
                x.Status == filter.Status.Value);
        }


        if (filter.FromDate.HasValue)
        {
            query = query.Where(x =>
                x.AppointmentDate >= filter.FromDate.Value);
        }


        if (filter.ToDate.HasValue)
        {
            query = query.Where(x =>
                x.AppointmentDate <= filter.ToDate.Value);
        }


        var totalCount = await query
            .CountAsync(cancellationToken);


        var items = await query
            .OrderByDescending(x => x.AppointmentDate)
            .Skip((filter.Page - 1) * 10)
            .Take(10)
            .Select(x => new AppointmentListItemDTO
            {
                Id = x.Id,
                DoctorId = x.DoctorId,
                PatientId = x.PatientId,
                AppointmentDate = x.AppointmentDate,
                Status = x.Status
            })
            .ToListAsync(cancellationToken);


        return new PagedResult<AppointmentListItemDTO>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.Page,
            PageSize = 10
        };
    }



    public async Task<AppointmentDetailsDTO?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
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
        CancellationToken cancellationToken = default)
    {
        var appointment = new Appointment
        {
            DoctorId = dto.DoctorId,
            PatientId = dto.PatientId,
            AppointmentDate = dto.AppointmentDate,
            Status = dto.Status,
            About = dto.About
        };


        _context.Appointments.Add(appointment);

        await _context.SaveChangesAsync(cancellationToken);
    }




    public async Task UpdateAsync(
        AppointmentEditDTO dto,
        CancellationToken cancellationToken = default)
    {
        var appointment =
            await _context.Appointments
                .FirstOrDefaultAsync(
                    x => x.Id == dto.Id,
                    cancellationToken);


        if (appointment == null)
        {
            return;
        }


        appointment.DoctorId = dto.DoctorId;
        appointment.PatientId = dto.PatientId;
        appointment.AppointmentDate = dto.AppointmentDate;
        appointment.Status = dto.Status;
        appointment.About = dto.About;


        await _context.SaveChangesAsync(cancellationToken);
    }




    public async Task ChangeStatusAsync(
        AppointmentStatusDTO dto,
        CancellationToken cancellationToken = default)
    {
        var appointment =
            await _context.Appointments
                .FirstOrDefaultAsync(
                    x => x.Id == dto.AppointmentId,
                    cancellationToken);


        if (appointment == null)
        {
            return;
        }


        appointment.Status = dto.Status;


        await _context.SaveChangesAsync(cancellationToken);
    }





    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var appointment =
            await _context.Appointments
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
        var patient =
            await _context.Patients
                .FirstOrDefaultAsync(
                    x => x.UserId == userId,
                    cancellationToken);


        if (patient == null)
        {
            return Enumerable.Empty<AppointmentListItemDTO>();
        }



        return await _context.Appointments
            .Where(x =>
                x.PatientId == patient.Id)
            .OrderByDescending(x =>
                x.AppointmentDate)
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
            .Where(x =>
                x.DoctorId == doctorId)
            .OrderByDescending(x =>
                x.AppointmentDate)
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
}