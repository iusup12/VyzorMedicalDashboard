using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Vyzor.Application.DTO.Orders
{
    internal class UpdateOrderDto
    {
        [Range(1, int.MaxValue)]
        public int AppointmentId { get; set; }
    }
}
