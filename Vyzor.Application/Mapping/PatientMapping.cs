using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Application.DTO.Patient;
using Vyzor.Domain.Entities;

namespace Vyzor.Application.Mappings;

public static class PatientMappings
{
    public static PatientListItemDTO ToListItemDto(this Patient patient)
    {
        return new PatientListItemDTO
        {
            Id = patient.Id,
            FullName = patient.FullName,
            Email = patient.Email
        };
    }

    public static PatientDetailsDTO ToDetailsDto(this Patient patient)
    {
        return new PatientDetailsDTO
        {
            Id = patient.Id,
            FullName = patient.FullName,
            Email = patient.Email,
            Phone = patient.Phone,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            Address = patient.Address
        };
    }

    public static PatientEditDTO ToEditDto(this Patient patient)
    {
        return new PatientEditDTO
        {
            Id = patient.Id,
            UserId = patient.UserId ?? string.Empty,
            FullName = patient.FullName,
            Email = patient.Email,
            Phone = patient.Phone,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            Address = patient.Address
        };
    }

    public static void UpdateFromDto(
        this Patient patient,
        PatientEditDTO dto)
    {
        patient.FullName = dto.FullName;
        patient.Email = dto.Email;
        patient.Phone = dto.Phone;
        patient.DateOfBirth = dto.DateOfBirth;
        patient.Gender = dto.Gender;
        patient.Address = dto.Address;
        patient.UpdatedAtUtc = DateTime.UtcNow;
    }
}

