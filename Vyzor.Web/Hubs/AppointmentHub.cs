using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Vyzor.Web.Authorization;

namespace Vyzor.Web.Hubs;

[Authorize]
public class AppointmentHub : Hub
{
    public const string DoctorsGroup = "appointments:doctors";

    public static string UserGroup(string userId)
        => $"user:{userId}";

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrWhiteSpace(userId))
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                UserGroup(userId));
        }

        if (Context.User?.IsInRole(AppRoles.Doctor) == true ||
            Context.User?.IsInRole(AppRoles.Admin) == true)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                DoctorsGroup);
        }

        await base.OnConnectedAsync();
    }
}