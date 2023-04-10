using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RemoteWork.Models;

namespace RemoteWork.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Team> Teams { get; set; } = default!;
    public DbSet<Message> Messages { get; set; } = default!;
    public DbSet<Issue> Issues { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Team>()
            .HasOne(t => t.Lead)
            .WithOne(u => u.Team)
            .HasForeignKey<Team>(t => t.LeadId);
        
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Team)
            .WithMany(t => t.Messages)
            .HasForeignKey(m => m.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Issue>()
            .HasOne(m => m.Team)
            .WithMany(t => t.Issues)
            .HasForeignKey(i => i.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}