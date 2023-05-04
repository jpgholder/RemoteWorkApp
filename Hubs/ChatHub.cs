using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using RemoteWork.Data;
using RemoteWork.Models;

namespace RemoteWork.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    
    public ChatHub(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task JoinTeam()
    {
        var user = await _userManager.GetUserAsync(Context.User!);
        if (user!.TeamId == null)
        {
            Context.Abort();
            return;
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, user.TeamId!);
    }

    public async Task SendMessage(string messageContent)
    {
       var user = await _userManager.GetUserAsync(Context.User!);
        if (user!.TeamId == null)
        {
            Context.Abort();
            return;
        }
        var message = new Message
        {
            Content = messageContent,
            SenderId = user.Id,
            TeamId = user.TeamId,
            SendedAt = DateTime.UtcNow
        };
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        var messageObject = new
        {
            content = message.Content,
            senderName = message.Sender!.FullName,
            senderId = message.SenderId
        };
        await Clients.Groups(user.TeamId).SendAsync("ReceiveMessage", messageObject);
    }
}