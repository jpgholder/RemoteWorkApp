using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemoteWork.Data;
using RemoteWork.Models;

namespace RemoteWork.Controllers;

[Authorize]
public class IssueController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IssueController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    // GET Issue/:id
    [Route("Issue/{id:int}")]
    public async Task<IActionResult> Index(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var issue = await _context.Issues
            .Include(i => i.Respondent)
            .Include(i => i.Team)
            .FirstOrDefaultAsync(i => i.IssueId == id && i.TeamId == user!.TeamId);
        if (issue == null)
        {
            return NotFound();
        }

        return View(issue);
    }


    [Route("Issue/Answer/{id:int}")]
    [HttpPost]
    public async Task<IActionResult> Answer(int id, [FromForm] string responseText, [FromForm] IFormFile? responseFile)
    {
        var user = await _userManager.GetUserAsync(User);
        var issue = await _context.Issues.FirstOrDefaultAsync(i => i.IssueId == id && i.TeamId == user!.TeamId);
        if (issue == null)
        {
            return NotFound();
        }
        if (issue.Status != Status.Opened)
        {
            return new ForbidResult("Задача уже закрыта");
        }
        if (issue.RespondentId != null)
        {
            return new ForbidResult("На данную задачу ответ уже был отправлен");
        }
        issue.ResponseText = responseText;
        issue.RespondentId = user!.Id;
        if (responseFile != null)
        {
            issue.ResponseFileName = responseFile.FileName;
            using var memoryStream = new MemoryStream();
            await responseFile.CopyToAsync(memoryStream);
            issue.ResponseFileData = memoryStream.ToArray();
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { id = issue.IssueId });
    }
    
    public async Task<IActionResult> GetResponseFile(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var issue = await _context.Issues
            .FirstOrDefaultAsync(i => i.IssueId == id && i.TeamId == user!.TeamId); 
        if (issue?.ResponseFileData == null)
        {
            return NotFound();
        }
        var fileData = issue.ResponseFileData;
        const string contentType = "application/octet-stream";
        var fileName = issue.ResponseFileName;
        
        return File(fileData, contentType, fileName);
    }

    // GET Create task
    public IActionResult Create()
    {
        return View();
    }

    // POST Create task
    [HttpPost, ActionName("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateIssue()
    {
        var user = await _userManager.GetUserAsync(User);
        var issue = new Issue
        {
            TeamId = user!.TeamId!,
            CreatorId = user.Id
        };
        var res = await TryUpdateModelAsync(issue, "",
            i => i.Title, i => i.Description
        );
        if (res)
        {
            _context.Add(issue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = issue.IssueId });
        }

        return View(issue);
    }
    
    // POST Finish task
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Finish(int id)
    {
        var user = await _context.Users
            .Include(u => u.Team)
            .FirstAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
        var issue = await _context.Issues
            .FirstOrDefaultAsync(i => i.IssueId == id && i.TeamId == user.TeamId);
        if (issue == null)
        {
            return NotFound();
        }

        if (user.Id != issue.CreatorId && user.Team!.LeadId != user.Id)
        {
            return new ForbidResult("У вас нет прав на завершение этой задачи");
        }

        issue.Status = Status.Finished;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { id = issue.IssueId });
    }
    
    // POST Finish task
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(int id)
    {
        var user = await _context.Users
            .Include(u => u.Team)
            .FirstAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
        var issue = await _context.Issues
            .FirstOrDefaultAsync(i => i.IssueId == id && i.TeamId == user.TeamId);
        if (issue == null)
        {
            return NotFound();
        }

        if (user.Id != issue.CreatorId && user.Team!.LeadId != user.Id)
        {
            return new ForbidResult("У вас нет прав на завершение этой задачи");
        }

        issue.Status = Status.Closed;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { id = issue.IssueId });
    }
}