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
        var issue = await _context.Issues.Include(i => i.Respondent)
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

    // GET Create task
    public IActionResult Create()
    {
        return View();
    }

    // POST: Create task
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

    // if (file.Length == 0)
    // {
    //     ModelState.AddModelError(string.Empty, "Файл не выбран");
    //     return View(issue);
    // }
    // issue.PerformerId = user!.Id;
    // issue.FileName = file.FileName;
    // using var memoryStream = new MemoryStream();
    // await file.CopyToAsync(memoryStream);
    // issue.FileData = memoryStream.ToArray();

    // POST: Task/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IssueId,FileName,FileData")] Issue issue)
    {
        if (id != issue.IssueId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(issue);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueExists(issue.IssueId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Task/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Issues == null)
        {
            return NotFound();
        }

        var issue = await _context.Issues
            .FirstOrDefaultAsync(m => m.IssueId == id);
        if (issue == null)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Task/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Issues == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Issues'  is null.");
        }

        var issue = await _context.Issues.FindAsync(id);
        if (issue != null)
        {
            _context.Issues.Remove(issue);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool IssueExists(int id)
    {
        return (_context.Issues?.Any(e => e.IssueId == id)).GetValueOrDefault();
    }
}