using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySupportAgent.Core.Database.Entities;

[Table("Vacancies")]
public class VacancyEntity
{
    [Key]
    public long Id { get; init; }

    [Required]
    [Column(TypeName = "TEXT")]
    [MaxLength(500)]
    public required string Title { get; init; }

    [Required]
    [Column(TypeName = "TEXT")]
    [MaxLength(2000)]
    public required string Url { get; init; }

    [Required]
    public DateTime SavedAt { get; init; }
}
