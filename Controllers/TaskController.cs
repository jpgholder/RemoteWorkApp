using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemoteWork.Data;
using RemoteWork.Models;

namespace RemoteWork.Controllers;

[Authorize]
public class TaskController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TaskController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Task
    public async Task<IActionResult> Index()
    {
        return _context.Issues != null
            ? View(await _context.Issues.ToListAsync())
            : Problem("Entity set 'ApplicationDbContext.Issues'  is null.");
    }

    // GET: Task/Details/5
    public async Task<IActionResult> Details(int? id)
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

        return View(issue);
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
            return RedirectToAction(nameof(Index));
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

    // GET: Task/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Issues == null)
        {
            return NotFound();
        }

        var issue = await _context.Issues.FindAsync(id);
        if (issue == null)
        {
            return NotFound();
        }

        return View(issue);
    }

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

        return View(issue);
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

        return View(issue);
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