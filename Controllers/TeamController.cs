using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using RemoteWork.Data;
using RemoteWork.Models;

namespace RemoteWork.Controllers;

[Authorize]
public class TeamController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private ApplicationUser _user = default!;
    public TeamController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        _user = (await _userManager.GetUserAsync(User))!;
        await base.OnActionExecutionAsync(context, next);
    }
    
    private bool TeamExists(string id)
    {
        return (_context.Teams?.Any(e => e.TeamId == id)).GetValueOrDefault();
    }
    
    
    // GET Create
    public IActionResult Create()
    {
        return View();
    }
    
    // POST Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name")] Team team)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user!.TeamId != null)
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
            return RedirectToAction("Info", "Team", null);

        foreach (var err in res.Errors)
            ModelState.AddModelError(string.Empty, err.Description);
        return View(team);
    }

    // GET Info
    [HttpGet("Team/")]
    public async Task<IActionResult> Info()
    {
        if (_user.TeamId == null)
            return RedirectToAction("Join", "Team", null);
        
        var team = await _context.Teams.Include(t => t.Lead)
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.TeamId == _user.TeamId);
        return View(team);
    }
    
    // POST Update
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost("Team/"), ActionName("Info")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update()
    {
        if (_user.TeamId == null)
            return RedirectToAction("Join", "Team", null);
        
        var team = await _context.Teams.FirstOrDefaultAsync(t => t.TeamId == _user.TeamId);
        if (team!.LeadId != _user.Id)
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Quit()
    {
        if (_user.TeamId == null)
            return RedirectToAction("Join", "Team", null);
        
        var team = await _context.Teams.Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.TeamId == _user!.TeamId);
        _user.TeamId = null;
        if (_user.Id == team!.LeadId)
        {
            var newLeadId = team.Members?.FirstOrDefault(m => m.Id != _user.Id)?.Id;
            if (newLeadId != null)
                team.LeadId = newLeadId;
            else
                _context.Teams.Remove(team);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }

    // POST Delete
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete()
    {
        if (_user.TeamId == null)
            return RedirectToAction("Join", "Team", null);
        
        var team = await _context.Teams.FindAsync(_user.TeamId);
        if (team!.LeadId != _user.Id)
            return Forbid("Вы не являетесь тимлидом данной команды");
        _context.Teams.Remove(team);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }
    
    // GET Join team
    public IActionResult Join()
    {
        return View();
    }
    
    // POST Join team
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Join([Bind("TeamId")] Team team)
    {
        if (_user.TeamId != null)
        {
            ModelState.AddModelError(string.Empty, "Вы уже являетесь участником команды");
            return View(team);
        }

        if (!TeamExists(team.TeamId))
        {
            ModelState.AddModelError(string.Empty, "Команды с таким ID не найдено");
            return View(team);
        }
        
        _user.TeamId = team.TeamId;
        var res = await _userManager.UpdateAsync(_user);
        if (res == IdentityResult.Success)
            return RedirectToAction("Index", "Home", null);

        foreach (var err in res.Errors)
            ModelState.AddModelError(string.Empty, err.Description);
        return View(team);
    }
}