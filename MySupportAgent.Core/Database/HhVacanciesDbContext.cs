using Microsoft.EntityFrameworkCore;
using MySupportAgent.Core.Database.Entities;

namespace MySupportAgent.Core.Database;

public class HhVacanciesDbContext : DbContext
{
    public DbSet<VacancyEntity> Vacancies { get; set; }

    public string DatabasePath { get; }

    public HhVacanciesDbContext()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        DatabasePath = Path.Join(path, "MySupportAgent", "hh_vacancies.db");
        Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath)!);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DatabasePath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VacancyEntity>(entity =>
        {
            entity.ToTable("Vacancies");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.Url).IsRequired();
            entity.Property(e => e.SavedAt).IsRequired();
            entity.HasIndex(e => e.Url).IsUnique();
        });
    }
}
