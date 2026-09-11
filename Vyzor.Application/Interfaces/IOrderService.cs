using System;
using System.Collections.Generic;
using System.Text;
using Vyzor.Application.DTO.Orders;

namespace Vyzor.Application.Interfaces
{
    internal interface IOrderService
    {
        Task<OrderDTO> GetOrderAsync(string userId, CancellationToken cancellationToken = default);

        Task<OrderDTO> AddAppointmentAsync(
            string userId,
            int productId,
            
            CancellationToken cancellationToken = default);

       
        Task<OrderDTO> RemoveAppointmentAsync(
            string userId,
            int productId,
            CancellationToken cancellationToken = default);

        Task<int> GetTotalQuantityAsync(string userId, CancellationToken cancellationToken = default);

        Task ClearAsync(string userId, CancellationToken cancellationToken = default);
    }
}
