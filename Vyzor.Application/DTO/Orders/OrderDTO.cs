using System;
using System.Collections.Generic;
using System.Text;

namespace Vyzor.Application.DTO.Orders
{
    internal class OrderDTO
    {
        
        public IReadOnlyList<OrderItemDTO> Items { get; set; } = Array.Empty<OrderItemDTO>();
        public decimal ItemsTotal { get; set; }
       
    }
}
