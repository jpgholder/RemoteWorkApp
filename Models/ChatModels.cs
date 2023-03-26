using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteWork.Models;

public class Chat
{   
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string ChatId { get; set; } = default!;
    public string TeamId { get; set; } = default!;
    public List<Message> Messages { get; set; } = new();
}

public class Message
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string MessageId { get; set; } = default!;
    public string ChatId { get; set; } = default!;
    
    public string SenderId { get; set; } = default!;
    public ApplicationUser Sender { get; set; } = default!;
    
    public string Content { get; set; } = default!;
}