
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
            .Include(x => x.Specialization)
            .AsQueryable();


        // Поиск
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(x =>
                x.FullName.Contains(filter.Search));
        }


        // Фильтр специализации
        if (filter.SpecializationId.HasValue)
        {
            query = query.Where(x =>
                x.SpecializationId == filter.SpecializationId.Value);
        }


        // Активность
        if (filter.IsActive.HasValue)
        {
            query = query.Where(x =>
                x.IsActive == filter.IsActive.Value);
        }


        var totalCount = await query
            .CountAsync(cancellationToken);



        int pageSize = 10;


        var items = await query
            .OrderBy(x => x.FullName)

            .Skip(
                (filter.Page - 1) * pageSize
            )

            .Take(pageSize)

            .Select(x => new DoctorListItemDTO
            {
                Id = x.Id,

                FullName = x.FullName,

                SpecializationName =
                    x.Specialization != null
                    ? x.Specialization.Name
                    : "",


                AppointmentPrice =
                    x.AppointmentPrice,


                Rating =
                    x.rating,


                IsActive =
                    x.IsActive

            })

            .ToListAsync(cancellationToken);



        return new PagedResult<DoctorListItemDTO>
        {
            Items = items,

            TotalCount = totalCount,

            PageNumber = filter.Page,

            PageSize = pageSize
        };
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

                
                Rating = x.rating,


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
                    x.rating

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
        if (filter.SpecializationId.HasValue)
        {
            query = query.Where(x =>
                x.SpecializationId == filter.SpecializationId.Value);
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
                x.rating >= filter.MinRating.Value);
        }

        // Сортировка
        query = filter.Sort?.ToLower() switch
        {
            "price_asc" =>
                query.OrderBy(x => x.AppointmentPrice),

            "price_desc" =>
                query.OrderByDescending(x => x.AppointmentPrice),

            "rating" =>
                query.OrderByDescending(x => x.rating),

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

                Rating = x.rating,

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

            .Include(x => x.Specialization)

            .Where(x => x.Id == id)


            .Select(x => new DoctorEditDTO
            {

                Id = x.Id,


                FullName =
                    x.FullName,


                About =
                    x.About,


                ExperienceYears =
                    x.ExperienceYears,


                AppointmentPrice =
                    x.AppointmentPrice,


                ImageUrl =
                    x.ImageUrl,


                SpecializationName =
                    x.Specialization != null
                    ? x.Specialization.Name
                    : ""

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
                x => x.Name == dto.SpecializationName,
                cancellationToken);



        if (specialization == null)
        {
            throw new Exception(
                "Specialization not found");
        }



        var doctor = new Doctor
        {

            FullName =
                dto.FullName,


            About =
                dto.About ?? "",


            ExperienceYears =
                dto.ExperienceYears,


            AppointmentPrice =
                dto.AppointmentPrice,


            ImageUrl =
                dto.ImageUrl ?? "",


            SpecializationId =
                specialization.Id,


            IsActive =
                true

        };



        _context.Doctors.Add(doctor);


        await _context.SaveChangesAsync(
            cancellationToken);

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
                x => x.Name == dto.SpecializationName,
                cancellationToken);



        if (specialization == null)
        {
            throw new Exception(
                "Specialization not found");
        }



        doctor.FullName =
            dto.FullName;


        doctor.About =
            dto.About ?? "";


        doctor.ExperienceYears =
            dto.ExperienceYears;


        doctor.AppointmentPrice =
            dto.AppointmentPrice;


        doctor.ImageUrl =
            dto.ImageUrl ?? "";


        doctor.SpecializationId =
            specialization.Id;



        await _context.SaveChangesAsync(
            cancellationToken);

    }





    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {

        var doctor =
            await _context.Doctors

            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (doctor == null)
            return;



        _context.Doctors.Remove(doctor);



        await _context.SaveChangesAsync(
            cancellationToken);

    }

}