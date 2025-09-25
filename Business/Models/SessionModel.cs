
using System.ComponentModel.DataAnnotations.Schema;

namespace Business.Models;
public class SessionModel
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    [Column(TypeName = "datetime2")]
    public DateTime Date { get; set; }
    public string? Intensity { get; set; }
}