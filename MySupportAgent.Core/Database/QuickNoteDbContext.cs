using Microsoft.EntityFrameworkCore;
using MySupportAgent.Core.Database.Entities;

namespace MySupportAgent.Core.Database;

public class QuickNoteDbContext : DbContext
{
    public DbSet<QuickNoteEntity> QuickNotes { get; set; }
    
    public string DatabasePath { get; }
    
    public QuickNoteDbContext()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        DatabasePath = Path.Join(path, "MySupportAgent", "quick_notes.db");
        Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath)!);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DatabasePath}");
    }
}
