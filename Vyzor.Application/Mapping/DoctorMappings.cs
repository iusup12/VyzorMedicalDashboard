
using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Application.DTO.Doctor;
using Vyzor.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Vyzor.Application.Mappings;

public static class DoctorMappings
{
    public static DoctorCardDTO ToCardDTO(this Doctor doctor)
    {
        return new DoctorCardDTO
        {
            Id = doctor.Id,
            SpecializationName = doctor.Specialization != null
                ? doctor.Specialization.Name
                : string.Empty
        };
    }

    public static DoctorListItemDTO ToListItemDTO(this Doctor doctor)
    {
        return new DoctorListItemDTO
        {
            Id = doctor.Id,
            SpecializationName = doctor.Specialization != null
                ? doctor.Specialization.Name
                : string.Empty
        };
    }

    public static DoctorDetailsDTO ToDetailsDTO(this Doctor doctor)
    {
        return new DoctorDetailsDTO
        {
            Id = doctor.Id,
            SpecializationName = doctor.Specialization != null
                ? doctor.Specialization.Name
                : string.Empty,
            Clinic=doctor.Clinic
        };
    }

    public static DoctorEditDTO ToEditDTO(this Doctor doctor)
    {
        return new DoctorEditDTO
        {
            Id = doctor.Id,
            SpecializationId = doctor.SpecializationId
        };
    }

    public static void ApplyTo(this DoctorEditDTO dto, Doctor doctor)
    {
        doctor.SpecializationId = dto.SpecializationId;
        doctor.UpdatedAtUtc = DateTime.UtcNow;
    }
}