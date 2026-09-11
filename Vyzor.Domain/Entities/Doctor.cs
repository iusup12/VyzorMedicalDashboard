using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Vyzor.Domain.Common;

namespace Vyzor.Domain.Entities;

public class Doctor : Entity
{
    public string FullName { get; set; } = string.Empty;

    public string About { get; set; } = string.Empty;

    public int ExperienceYears { get; set; }
    public int rating;


    public string ImageUrl { get; set; } = string.Empty;

    public int SpecializationId { get; set; }
    public string SpecializationName { get; set; } = string.Empty;


    public Specialization? Specialization { get; set; }

    public ICollection<Appointment> Appointments { get; set; }
        = new List<Appointment>();

    public ICollection<Review> Reviews { get; set; }
        = new List<Review>();

    public ICollection<DoctorSchedule> Schedules { get; set; }
        = new List<DoctorSchedule>();

    

    public string? Education { get; set; }


    public decimal AppointmentPrice { get; set; }
    public string Clinic {get; set; }


    

   
   




 
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }

    



   




    public bool IsActive { get; set; }
}
