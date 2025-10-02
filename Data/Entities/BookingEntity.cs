using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class BookingEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(Session))]
    public string SessionId { get; set; } = null!;

    public virtual UserEntity User { get; set; } = null!;
    public virtual SessionEntity Session { get; set; } = null!;
}
