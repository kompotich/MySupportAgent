using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySupportAgent.Core.Database.Entities;

public class QuickNoteEntity
{
    public long Id { get; init; }

    [Column(TypeName = "TEXT")]
    [MaxLength(255)]
    public required string Title { get; init; }

    [Column(TypeName = "TEXT")]
    [MaxLength(1000)]
    public required string Content { get; init; }
}