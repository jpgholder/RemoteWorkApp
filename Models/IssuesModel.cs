using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteWork.Models;

public class Issue
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IssueId { get; set; }

    public string TeamId { get; set; } = default!;
    public Team? Team { get; set; }
    public string CreatorId { get; set; } = default!;
    public string? RespondentId { get; set; }
    public ApplicationUser? Respondent { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;

    public string? FileName { get; set; }
    public byte[]? FileData { get; set; }
    
}