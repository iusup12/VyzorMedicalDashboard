
using Microsoft.EntityFrameworkCore;
using Vyzor.Application.Common;
using Vyzor.Application.DTO.Doctor;
using Vyzor.Application.DTO.Filters;
using Vyzor.Application.Interfaces;
using Vyzor.Domain.Entities;
using Vyzor.Infrastructure.Data;


namespace Vyzor.Infrastructure.Services;

public class DoctorService : IDoctorService
{
    private readonly AppDbContext _context;


    public DoctorService(AppDbContext context)
    {
        _context = context;
    }


    public async Task<PagedResult<DoctorListItemDTO>> GetPagedAsync(
    AdminDoctorFilterDTO filter,
    CancellationToken cancellationToken = default)
    {
        var query = _context.Doctors
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(x =>
                x.FullName.Contains(filter.Search));
        }

        if (filter.SpecializationId.HasValue)
        {
            query = query.Where(x =>
                x.SpecializationId == filter.SpecializationId.Value);
        }

        if (filter.IsActive.HasValue)
        {
            query = query.Where(x =>
                x.IsActive == filter.IsActive.Value);
        }

        var totalCount = await query
            .CountAsync(cancellationToken);

        var pageNumber = Paging.PageNumber(filter);
        var pageSize = Paging.PageSize(filter);

        var items = await query
            .OrderBy(x => x.FullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new DoctorListItemDTO
            {
                Id = x.Id,

                FullName = x.FullName,

                SpecializationName =
                    x.Specialization != null
                        ? x.Specialization.Name
                        : "",

                AppointmentPrice = x.AppointmentPrice,

                Rating = x.Rating,

                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return Paging.Result(
            items,
            filter,
            totalCount);
    }




    public async Task<IEnumerable<DoctorCardDTO>> GetCatalogAsync(
        CancellationToken cancellationToken = default)
    {

        return await _context.Doctors

            .Include(x => x.Specialization)

            .Where(x => x.IsActive)

            .Select(x => new DoctorCardDTO
            {
                Id = x.Id,

                FullName = x.FullName,


                SpecializationName =
                    x.Specialization != null
                    ? x.Specialization.Name
                    : "",


                PhotoUrl = x.ImageUrl,

                
                Rating = x.Rating,


                AppointmentPrice =
                    x.AppointmentPrice

            })

            .ToListAsync(cancellationToken);
    }





    public async Task<DoctorDetailsDTO?> GetDetailsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {


        return await _context.Doctors

            .Include(x => x.Specialization)

            .Where(x => x.Id == id)

            .Select(x => new DoctorDetailsDTO
            {

                Id = x.Id,
                Clinic=x.Clinic,
                FullName = x.FullName,


                About = x.About,



                Education = x.Education,


                ExperienceYears =
                    x.ExperienceYears,


                AppointmentPrice =
                    x.AppointmentPrice,


                SpecializationId =
                    x.SpecializationId,


                SpecializationName =
                    x.Specialization != null
                    ? x.Specialization.Name
                    : "",


                PhotoUrl =
                    x.ImageUrl,


                Rating =
                    x.Rating

            })


            .FirstOrDefaultAsync(cancellationToken);
    }





    public async Task<DoctorProfileDTO?> GetProfileAsync(
        int id,
        CancellationToken cancellationToken = default)
    {

        return await _context.Doctors

            .Include(x => x.Specialization)

            .Where(x => x.Id == id)


            .Select(x => new DoctorProfileDTO
            {

                Id = x.Id,
                Clinic = x.Clinic,

                FullName =
                    x.FullName,


                Email =
                    x.Email,


                PhoneNumber =
                    x.PhoneNumber,


                About =
                    x.About,


                Education =
                    x.Education,


                ExperienceYears =
                    x.ExperienceYears,


                AppointmentPrice =
                    x.AppointmentPrice,


                SpecializationName =
                    x.Specialization != null
                    ? x.Specialization.Name
                    : "",


                ImageUrl =
                    x.ImageUrl,


                IsActive =
                    x.IsActive

            })

            .FirstOrDefaultAsync(cancellationToken);

    }


    public async Task<PagedResult<DoctorCardDTO>> GetCatalogPagedAsync(
    DoctorCatalogFilterDTO filter,
    CancellationToken cancellationToken = default)
    {
        var query = _context.Doctors
            .AsNoTracking()
            .Where(x => x.IsActive);

        // Поиск
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(x =>
                x.FullName.Contains(filter.Search));
        }

        // Специализация
        
        if (filter.SpecializationIds.Count > 0)
        {
            query = query.Where(x =>
                filter.SpecializationIds.Contains(x.SpecializationId));
        }
        // Цена от
        if (filter.MinPrice.HasValue)
        {
            query = query.Where(x =>
                x.AppointmentPrice >= filter.MinPrice.Value);
        }

        // Цена до
        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(x =>
                x.AppointmentPrice <= filter.MaxPrice.Value);
        }

        // Рейтинг
        if (filter.MinRating.HasValue)
        {
            query = query.Where(x =>
                x.Rating >= filter.MinRating.Value);
        }

        // Сортировка
        query = filter.Sort?.ToLower() switch
        {
            "price_asc" =>
                query.OrderBy(x => x.AppointmentPrice),

            "price_desc" =>
                query.OrderByDescending(x => x.AppointmentPrice),

            "rating" =>
                query.OrderByDescending(x => x.Rating),

            "name" =>
                query.OrderBy(x => x.FullName),

            _ =>
                query.OrderBy(x => x.FullName)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var pageNumber = Paging.PageNumber(filter);
        var pageSize = Paging.PageSize(filter);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new DoctorCardDTO
            {
                Id = x.Id,

                FullName = x.FullName,

                SpecializationName =
                    x.Specialization != null
                        ? x.Specialization.Name
                        : "",

                PhotoUrl = x.ImageUrl,

                Rating = x.Rating,

                AppointmentPrice = x.AppointmentPrice
            })
            .ToListAsync(cancellationToken);

        return Paging.Result(
            items,
            filter,
            totalCount);
    }


    public async Task<DoctorEditDTO?> GetForEditAsync(
 int id,
 CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
        .AsNoTracking()
        .Where(x => x.Id == id)
        .Select(x => new DoctorEditDTO
        {
            Id = x.Id,
            FullName = x.FullName,
            About = x.About,
            ExperienceYears = x.ExperienceYears,
            AppointmentPrice = x.AppointmentPrice,
            ImageUrl = x.ImageUrl,
            SpecializationId = x.SpecializationId
        })
        .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task CreateAsync(
DoctorEditDTO dto,
CancellationToken cancellationToken = default)
    {
        var specialization =
        await _context.Specializations
        .FirstOrDefaultAsync(
        x => x.Id == dto.SpecializationId,
        cancellationToken);


if (specialization == null)
            throw new Exception("Specialization not found.");

        var doctor = new Doctor
        {
            FullName = dto.FullName,
            About = dto.About ?? string.Empty,

            ExperienceYears = dto.ExperienceYears,

            AppointmentPrice = dto.AppointmentPrice,

            ImageUrl = dto.ImageUrl ?? string.Empty,

            SpecializationId = dto.SpecializationId,

         
            Clinic = string.Empty,
            Email = string.Empty,

            
            Rating = 0,
            IsActive = true
        };

        _context.Doctors.Add(doctor);

        await _context.SaveChangesAsync(cancellationToken);

}




    public async Task UpdateAsync(
     DoctorEditDTO dto,
     CancellationToken cancellationToken = default)
    {
        var doctor =
            await _context.Doctors
                .FirstOrDefaultAsync(
                    x => x.Id == dto.Id,
                    cancellationToken);

        if (doctor == null)
            return;

        var specialization =
            await _context.Specializations
                .FirstOrDefaultAsync(
                    x => x.Id == dto.SpecializationId,
                    cancellationToken);

        if (specialization == null)
        {
            throw new Exception("Specialization not found.");
        }

        doctor.FullName = dto.FullName;
        doctor.About = dto.About ?? string.Empty;
        doctor.ExperienceYears = dto.ExperienceYears;
        doctor.AppointmentPrice = dto.AppointmentPrice;
        doctor.ImageUrl = dto.ImageUrl ?? string.Empty;

        doctor.SpecializationId = dto.SpecializationId;

        await _context.SaveChangesAsync(cancellationToken);
    }





    public async Task DeleteAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (doctor == null)
            throw new Exception("Doctor not found.");

        doctor.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);
    }

}