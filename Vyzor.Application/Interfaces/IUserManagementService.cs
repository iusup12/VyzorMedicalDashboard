

using Vyzor.Application.Common;
using Vyzor.Application.DTO.Filters;
using Vyzor.Application.DTO.User;

namespace Vyzor.Application.Interfaces;

public interface IUserManagementService
{
    Task<PagedResult<UserEditDTO>> GetPagedAsync(
        UserFilterDTO filter,
        CancellationToken cancellationToken = default);

    Task<UserEditDTO?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task CreateAsync(
        UserEditDTO dto,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        UserEditDTO dto,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string id,
        CancellationToken cancellationToken = default);
}
