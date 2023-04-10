using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemoteWork.Data;
using RemoteWork.Models;

namespace RemoteWork.Controllers;

public class HomeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public HomeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return View();
        }

        var user = await _userManager.GetUserAsync(User);
        Team? team = null;
        if (user?.TeamId != null)
        {
            team = await _context.Teams
                .Include(t => t.Issues)
                .Include(t => t.Messages)
                 !.ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(t => t.TeamId == user.TeamId);
        }
        var tuple = new Tuple<ApplicationUser?, Team?>(user, team);
        return View(tuple);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}