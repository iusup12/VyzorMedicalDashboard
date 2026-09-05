using Vyzor.Application.Common;
using Microsoft.EntityFrameworkCore;
using Vyzor.Application.Common;
using Vyzor.Application.DTO.Filters;
using Vyzor.Application.DTO.Patient;
using Vyzor.Application.Interfaces;
using Vyzor.Domain.Entities;
using Vyzor.Infrastructure.Data;


namespace Vyzor.Infrastructure.Services;


public class PatientService : IPatientService
{
    private readonly AppDbContext _context;


    public PatientService(AppDbContext context)
    {
        _context = context;
    }



    public async Task<PagedResult<PatientListItemDTO>> GetListAsync(
        PatientFilterDTO filter,
        CancellationToken cancellationToken = default)
    {

        var query = _context.Patients
            .AsQueryable();



        if (!string.IsNullOrWhiteSpace(filter.FullName))
        {
            query = query.Where(x =>
                x.FullName.Contains(filter.FullName));
        }



        if (!string.IsNullOrWhiteSpace(filter.Email))
        {
            query = query.Where(x =>
                x.Email.Contains(filter.Email));
        }



        if (!string.IsNullOrWhiteSpace(filter.Phone))
        {
            query = query.Where(x =>
                x.Phone != null &&
                x.Phone.Contains(filter.Phone));
        }



        var totalCount =
            await query.CountAsync(cancellationToken);



        var items = await query
            .OrderBy(x => x.FullName)
            .Skip(
                (filter.PageNumber - 1)
                * filter.PageSize)
            .Take(filter.PageSize)
            .Select(x => new PatientListItemDTO
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email
            })
            .ToListAsync(cancellationToken);



        return new PagedResult<PatientListItemDTO>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };

    }



    public async Task<PatientDetailsDTO?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {

        return await _context.Patients
            .Where(x => x.Id == id)
            .Select(x => new PatientDetailsDTO
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                Phone = x.Phone,
                DateOfBirth = x.DateOfBirth,
                Gender = x.Gender,
                Address = x.Address
            })
            .FirstOrDefaultAsync(cancellationToken);

    }



    public async Task<PatientEditDTO> CreateAsync(
        PatientEditDTO dto,
        CancellationToken cancellationToken = default)
    {

        var patient = new Patient
        {
            UserId = dto.UserId,
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Address = dto.Address,
            CreatedAtUtc = DateTime.UtcNow
        };


        _context.Patients.Add(patient);


        await _context.SaveChangesAsync(cancellationToken);


        dto.Id = patient.Id;


        return dto;

    }





    public async Task<PatientEditDTO?> UpdateAsync(
        PatientEditDTO dto,
        CancellationToken cancellationToken = default)
    {


        var patient =
            await _context.Patients
            .FirstOrDefaultAsync(
                x => x.Id == dto.Id,
                cancellationToken);



        if (patient == null)
            return null;



        patient.FullName = dto.FullName;
        patient.Email = dto.Email;
        patient.Phone = dto.Phone;
        patient.DateOfBirth = dto.DateOfBirth;
        patient.Gender = dto.Gender;
        patient.Address = dto.Address;

        patient.UpdatedAtUtc =
            DateTime.UtcNow;



        await _context.SaveChangesAsync(
            cancellationToken);



        return dto;

    }




    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {


        var patient =
            await _context.Patients
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);



        if (patient == null)
            return false;



        _context.Patients.Remove(patient);



        await _context.SaveChangesAsync(
            cancellationToken);



        return true;

    }

}