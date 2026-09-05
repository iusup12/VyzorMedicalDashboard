using System;
using System.Collections.Generic;
using System.Text;

using Vyzor.Application.DTO.Appointment;
using Vyzor.Domain.Entities;

namespace Vyzor.Application.Mappings;

public static class AppointmentMappings
{
    public static AppointmentListItemDTO ToListItemDTO(this Appointment a)
    {
        return new AppointmentListItemDTO
        {
            Id = a.Id,
            DoctorName = a.Doctor.FullName != null ? a.Doctor.Id.ToString() : "",
            PatientName = a.Patient.FullName != null ? a.Patient.Id.ToString() : "",
            AppointmentDate = a.AppointmentDate,
            Status = a.Status
        };
    }

    public static AppointmentDetailsDTO ToDetailsDTO(this Appointment a)
    {
        return new AppointmentDetailsDTO
        {
            AppointmentId = a.Id,
            DoctorId = a.DoctorId,
            PatientId = a.PatientId,
            AppointmentDate = a.AppointmentDate,
            Status = a.Status
        };
    }

    public static AppointmentEditDTO ToEditDTO(this Appointment a)
    {
        return new AppointmentEditDTO
        {
            Id = a.Id,
            DoctorId = a.DoctorId,
            PatientId = a.PatientId,
            AppointmentDate = a.AppointmentDate,
            About = a.About
        };
    }

    public static void ApplyTo(this AppointmentEditDTO dto, Appointment a)
    {
        a.DoctorId = dto.DoctorId;
        a.PatientId = dto.PatientId;
        a.AppointmentDate = dto.AppointmentDate;
        a.About = dto.About;
        a.UpdatedAtUtc = DateTime.UtcNow;
    }

    public static void ApplyStatus(this AppointmentStatusDTO dto, Appointment a)
    {
        a.Status = dto.Status;
        a.UpdatedAtUtc = DateTime.UtcNow;
    }
}