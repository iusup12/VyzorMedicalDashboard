using Vyzor.Domain.Common;
using Vyzor.Domain.Entities;

public class Specialization : Entity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

  

    public ICollection<Doctor> Doctors { get; set; }
        = new List<Doctor>();
}