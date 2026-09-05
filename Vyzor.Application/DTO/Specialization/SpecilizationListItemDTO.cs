using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.Specialization;

public class SpecializationListItemDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    

    public bool IsActive { get; set; }
}