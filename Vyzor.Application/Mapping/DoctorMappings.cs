using System;
using Vyzor.Application.DTO.Doctor;
using Vyzor.Domain.Entities;

namespace Vyzor.Application.Mappings;

public static class DoctorMappings
{
    public static DoctorCardDTO ToCardDTO(this Doctor doctor)
    {
        return new DoctorCardDTO
        {
            Id = doctor.Id,
            FullName = doctor.FullName,

            SpecializationName = doctor.Specialization != null
                ? doctor.Specialization.Name
                : string.Empty,

            PhotoUrl = doctor.ImageUrl,

            Rating = doctor.Rating,

            AppointmentPrice = doctor.AppointmentPrice
        };
    }

    public static DoctorListItemDTO ToListItemDTO(this Doctor doctor)
    {
        return new DoctorListItemDTO
        {
            Id = doctor.Id,
            FullName = doctor.FullName,

            SpecializationName = doctor.Specialization != null
                ? doctor.Specialization.Name
                : string.Empty,

        
            Rating = doctor.Rating,

            AppointmentPrice = doctor.AppointmentPrice
        };
    }

    public static DoctorDetailsDTO ToDetailsDTO(this Doctor doctor)
    {
        return new DoctorDetailsDTO
        {
            Id = doctor.Id,

            FullName = doctor.FullName,

            SpecializationName = doctor.Specialization != null
                ? doctor.Specialization.Name
                : string.Empty,

            PhotoUrl = doctor.ImageUrl,

            Rating = doctor.Rating,

            AppointmentPrice = doctor.AppointmentPrice,

            Clinic = doctor.Clinic,

            About = doctor.About,

            ExperienceYears = doctor.ExperienceYears,

            Education = doctor.Education,

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