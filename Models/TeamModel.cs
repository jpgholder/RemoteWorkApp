using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteWork.Models;

public class Team
{   
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string TeamId { get; set; } = default!;
    [Required(ErrorMessage = "Обязательное поле")]
    [DisplayName("Название команды")]
    public string Name { get; set; } = default!;
    [DisplayName("Тимлид")]
    public string LeadId { get; set; } = default!;
    public ApplicationUser Lead { get; set; } = default!;

    public List<ApplicationUser> Members { get; set; } = new();
}