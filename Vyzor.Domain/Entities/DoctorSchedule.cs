using System;
using System.Collections.Generic;
using System.Text;

using Vyzor.Domain.Common;

namespace Vyzor.Domain.Entities;

public class DoctorSchedule : Entity
{
    public int DoctorId { get; set; }

    public Doctor? Doctor { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsAvailable { get; set; } = true;
}