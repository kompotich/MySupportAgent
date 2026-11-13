using System.ComponentModel;
using MySupportAgent.Core.Database;
using MySupportAgent.Core.Database.Entities;
using MySupportAgent.Core.Mappers;

namespace MySupportAgent.Core.Tools;

public static class QuickNoteTools
{
    [Description("Получить список всех быстрых заметок.")]
    public static string GetAllQuickNotes()
    {
        using var db = new QuickNoteDbContext();

        var notes = db.QuickNotes
            .ToList()
            .Select(QuickNoteMapper.ToModel)
            .ToList();
        
        if (notes.Count == 0)
        {
            return "Нет быстрых заметок.";
        }

        var notesList = string.Join(Environment.NewLine, notes.Select(n => $"№ '{n.Id}' - '{n.Title}'"));
        return $"Список быстрых заметок:{Environment.NewLine}{notesList}";
    }

    [Description("Получить быструю заметку по идентификатору.")]
    public static string GetQuickNoteById(
        [Description("Идентификатор быстрой заметки.")]
        int quickNoteId)
    {
        using var db = new QuickNoteDbContext();

        var quickNoteEntity = db.QuickNotes.FirstOrDefault(n => n.Id == quickNoteId);
        var quickNote = quickNoteEntity is null ? null : QuickNoteMapper.ToModel(quickNoteEntity);

        return quickNote == null 
            ? "Не удалось найти быструю заметку по идентификатору."
            : $"№ '{quickNote.Id}' - '{quickNote.Title}{Environment.NewLine}{quickNote.Content}'";
    }

    [Description("Получить быструю заметку по названию.")]
    public static string GetQuickNoteByTitle(
        [Description("Получить быструю заметку по азванию.")]
        string quickNoteTitle)
    {
        if (string.IsNullOrWhiteSpace(quickNoteTitle))
        {
            return "Не указано название быстрой заметки.";
        }

        using var db = new QuickNoteDbContext();

        var quickNoteEntity = db.QuickNotes.FirstOrDefault(x =>
            x.Title.Equals(quickNoteTitle, StringComparison.CurrentCultureIgnoreCase));
        var quickNote = quickNoteEntity is null ? null : QuickNoteMapper.ToModel(quickNoteEntity);
        
        return quickNote == null
            ? "Не удалось найти быструю заметку по названию."
            : $"№ '{quickNote.Id}' - '{quickNote.Title}{Environment.NewLine}{quickNote.Content}'";
    }

    [Description("Создать быструю заметку.")]
    public static string CreateQuickNote(
        [Description("Название быстрой заметки.")]
        string quickNoteTitle, 
        [Description("Содержание быстрой заметки.")]
        string quickNoteContent)
    {
        if (string.IsNullOrWhiteSpace(quickNoteTitle))
        {
            return "Название быстрой заметки не может быть пустым.";
        }

        if (string.IsNullOrWhiteSpace(quickNoteContent))
        {
            return "Содержание быстрой заметки не может быть пустым.";
        }

        using var db = new QuickNoteDbContext();

        var maxNumber = db.QuickNotes.Any() ? db.QuickNotes.Max(n => n.Id) : 0;
        var quickNoteEntity = new QuickNoteEntity
        {
            Id = maxNumber + 1,
            Title = quickNoteTitle,
            Content = quickNoteContent
        };
        
        db.QuickNotes.Add(quickNoteEntity);
        db.SaveChanges();
        
        return $"Быстрая заметка '{quickNoteTitle}' № '{quickNoteEntity.Id}' создана.";
    }

    [Description("Удалить все быстрые заметки.")]
    public static string DeleteAllQuickNotes()
    {
        using var db = new QuickNoteDbContext();

        var quickNoteEntities = db.QuickNotes.ToList();
        
        if (quickNoteEntities.Count == 0)
        {
            return "Нет быстрых заметок для удаления.";
        }

        db.QuickNotes.RemoveRange(quickNoteEntities);
        db.SaveChanges();
        
        return $"Удалено {quickNoteEntities.Count} быстрых заметок.";
    }

    [Description("Удалить быструю заметку по идентификатору.")]
    public static string DeleteQuickNoteById(
        [Description("Идентификатор быстрой заметки.")]
        long id)
    {
        using var db = new QuickNoteDbContext();

        var quickNoteEntity = db.QuickNotes.FirstOrDefault(n => n.Id == id);

        if (quickNoteEntity == null)
        {
            return "Не удалось найти быструю заметку по идентификатору.";
        }

        db.QuickNotes.Remove(quickNoteEntity);
        db.SaveChanges();
        
        return $"Быстрая заметка '{quickNoteEntity.Title}' № '{quickNoteEntity.Id}' удалена.";
    }

    [Description("Удалить быструю заметку по названию.")]
    public static string DeleteQuickNoteByTitle(
        [Description("Название быстрой заметки.")]
        string quickNoteTitle)
    {
        if (string.IsNullOrWhiteSpace(quickNoteTitle))
        {
            return "Не указано название быстрой заметки.";
        }

        using var db = new QuickNoteDbContext();

        var quickNoteEntity = db.QuickNotes.FirstOrDefault(x =>
            x.Title.Equals(quickNoteTitle, StringComparison.CurrentCultureIgnoreCase));

        if (quickNoteEntity == null)
        {
            return "Не удалось найти быструю заметку по названию.";
        }

        db.QuickNotes.Remove(quickNoteEntity);
        db.SaveChanges();
        
        return $"Быстрая заметка '{quickNoteEntity.Title}' № '{quickNoteEntity.Id}' удалена.";
    }
}