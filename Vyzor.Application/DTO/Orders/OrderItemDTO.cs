using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.Orders
{
    internal class OrderItemDTO
    {
        public int AppointmentId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal AppointmentPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}

