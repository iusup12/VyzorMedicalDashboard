using Vyzor.Application.DTO.Doctor;


namespace Vyzor.Application.DTO.Appointment;


public class AppointmentCreateViewModel
{

    public DoctorDetailsDTO Doctor { get; set; } = null!;


    public AppointmentEditDTO Appointment { get; set; } = null!;

}