using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Application.Common;
namespace Vyzor.Application.DTO.Filters
{
    public class PatientFilterDTO : PagedRequest
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
