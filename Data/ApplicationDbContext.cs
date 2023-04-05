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

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Team)
            .WithMany(t => t.Members)
            .HasForeignKey(u => u.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Team>()
            .HasOne(t => t.Lead)
            .WithOne()
            .HasForeignKey<Team>(t => t.LeadId);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Team)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Issue>()
            .HasOne(m => m.Team)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
        
        // modelBuilder.Entity<Team>()
        //     .HasMany(t => t.Issues)
        //     .WithOne(i => i.Team!)
        //     .HasForeignKey(i => i.TeamId)
        //     .OnDelete(DeleteBehavior.Cascade);
    }
}