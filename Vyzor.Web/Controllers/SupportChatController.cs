using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vyzor.Application.Interfaces;
using Vyzor.Web.Models.SupportChat;

namespace Vyzor.Web.Controllers;

[Authorize]
public class SupportChatController : Controller
{
    private readonly ISupportChatService _chatService;

    public SupportChatController(
        ISupportChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Challenge();

        var messages = await _chatService.GetMessagesAsync(
            userId,
            cancellationToken);

        var chatId = await _chatService.GetChatIdAsync(
            userId,
            cancellationToken);

        var model = new SupportChatViewModel
        {
            ChatId = chatId,
            Messages = messages.ToList()
        };

        return View(model);
    }
}