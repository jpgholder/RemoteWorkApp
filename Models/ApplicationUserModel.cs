using Microsoft.AspNetCore.Identity;
namespace RemoteWork.Models;

public class ApplicationUser: IdentityUser
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    public string? TeamId { get; set; }
    public Team? Team { get; set; }
}