using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace  RemoteWork.Models;

public class Message
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string MessageId { get; set; } = default!;
    public string TeamId { get; set; } = default!;
    public Team? Team { get; set; }
    
    public string SenderId { get; set; } = default!;
    public ApplicationUser? Sender { get; set; }
    
    public string Content { get; set; } = default!;
}