using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Data.Contexts;
public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<SessionEntity> Sessions { get; set; } = null!;
    public DbSet<BookingEntity> Bookings { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        var adminRoleId = new Guid("d290f1ee-6c54-4b01-90e6-d701748f0851");
        var userRoleId = new Guid("c4a760a8-5b63-4d3c-9e0f-1f2e3d4c5b6a");

        builder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid> { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole<Guid> { Id = userRoleId, Name = "User", NormalizedName = "USER" }
        );

        builder.Entity<BookingEntity>(b =>
        {
            b.HasOne(x => x.Session)
            .WithMany(s => s.Bookings)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<BookingEntity>()
            .HasIndex(x => new { x.UserId, x.SessionId })
            .IsUnique();

        builder.Entity<SessionEntity>()
            .HasIndex(s => s.Date);

    }
}
