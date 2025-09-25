
namespace Business.Dtos;
public class SessionDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public DateTime Date { get; set; }
    public string? Intensity { get; set; }
}