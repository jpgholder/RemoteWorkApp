using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using RemoteWork.Data;
using RemoteWork.Models;

namespace RemoteWork.Controllers;

public class TeamMember : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userManager = context.HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>()!;
        var user = userManager.GetUserAsync(context.HttpContext.User).Result!;
        var teamId = context.RouteData.Values["id"]?.ToString();
        if (user.TeamId != teamId)
            context.Result = new ForbidResult();
    }
}

[Authorize]
public class TeamController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TeamController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    public IActionResult Create()
    {
        return View();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TeamId,Name")] Team team)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return View(team);
        }

        if (user.TeamId != null)
        {
            ModelState.AddModelError(string.Empty, "Вы уже являетесь участником команды");
            return View(team);
        }

        team.LeadId = user.Id;
        _context.Add(team);
        await _context.SaveChangesAsync();

        user.TeamId = team.TeamId;
        var res = await _userManager.UpdateAsync(user);
        if (res == IdentityResult.Success)
            return RedirectToAction("Info", "Team", new { id = team.TeamId });

        foreach (var err in res.Errors)
            ModelState.AddModelError(string.Empty, err.Description!);
        return View(team);
    }

    // GET Edit
    [TeamMember]
    public async Task<IActionResult> Info(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var team = await _context.Team.Include(t => t.Lead).FirstOrDefaultAsync(t => t.TeamId == id);
        if (team == null)
            return NotFound();

        return View(team);
    }

    // POST Edit
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost, ActionName("Info")]
    [TeamMember]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(string id)
    {
        var team = await _context.Team.Include(t => t.Lead).FirstOrDefaultAsync(t => t.TeamId == id);
        if (id != team?.TeamId)
        {
            return NotFound();
        }
        // var user = await _userManager.GetUserAsync(User);
        if (team.LeadId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            return Forbid("Вы не являетесь тимлидом данной команды");
        if (!await TryUpdateModelAsync(team, "", t => t.Name)) return View(team);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TeamExists(team.TeamId))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Info));
    }

    // POST User exit from team
    [HttpPost]
    [TeamMember]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Quit(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var team = await _context.Team.Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.TeamId == id);
        if (team == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        user!.TeamId = null;
        if (user.Id == team.LeadId)
        {
            var newLeadId = team.Members.FirstOrDefault(m => m.Id != user.Id)?.Id;
            if (newLeadId != null)
                team.LeadId = newLeadId;
            else
                _context.Team.Remove(team);
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }

    // POST Delete
    [HttpPost]
    [TeamMember]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var team = await _context.Team.FindAsync(id);
        if (team == null)
            return NotFound();
        if (team.LeadId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            return Forbid("Вы не являетесь тимлидом данной команды");
        _context.Team.Remove(team);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }

    private bool TeamExists(string id)
    {
        return (_context.Team?.Any(e => e.TeamId == id)).GetValueOrDefault();
    }
}