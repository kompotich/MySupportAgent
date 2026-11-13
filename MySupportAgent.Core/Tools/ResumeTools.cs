using System.ComponentModel;
using System.Text;
using MySupportAgent.Core.Configuration;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace MySupportAgent.Core.Tools;

public static class ResumeTools
{
    private static readonly string ResumeDirectory = Config.Load().ResumeDirectory;

    [Description("Возвращает список PDF-файлов резюме доступных для анализа.")]
    public static string ListResumeFiles()
    {
        if (!Directory.Exists(ResumeDirectory))
        {
            return "Каталог с резюме не найден.";
        }

        var files = Directory.GetFiles(ResumeDirectory, "*.pdf", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .ToList();

        if (files.Count == 0)
        {
            return "В каталоге с резюме не найдено PDF-файлов.";
        }

        return "Доступные PDF-файлы резюме:\n" + string.Join("\n", files);
    }

    [Description("Возвращает полный текст резюме.")]
    public static string GetResumeText([Description("Имя файла резюме[.")] string resumeName)
    {
        var file = Path.Combine(ResumeDirectory, $"{resumeName}.pdf");
        var result = new StringBuilder();

        try
        {
            using var document = PdfDocument.Open(file);

            foreach (var page in document.GetPages())
            {
                result.AppendLine(ContentOrderTextExtractor.GetText(page));
            }
        }
        catch (Exception ex)
        {
            result.AppendLine($"Не удалось обработать {Path.GetFileName(file)}: {ex.Message}");
        }

        return result.ToString();
    }

    [Description(
        "Ищет текстовое совпадение во всех PDF-резюме и возвращает фрагменты с именами файлов и номерами страниц.")]
    public static string SearchResumeByQuery(
        [Description("Текст или ключевые слова для поиска")]
        string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return "Поисковый запрос пуст.";
        }

        if (!Directory.Exists(ResumeDirectory))
        {
            return "Каталог с резюме не найден.";
        }

        var result = new StringBuilder();

        var files = Directory.GetFiles(ResumeDirectory, "*.pdf", SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            try
            {
                using var document = PdfDocument.Open(file);

                foreach (var page in document.GetPages())
                {
                    var text = ContentOrderTextExtractor.GetText(page);

                    var lines = text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in lines)
                    {
                        if (!line.Contains(query, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        result.AppendLine($"Файл: {Path.GetFileName(file)}");
                        result.AppendLine($"Страница: {page.Number}");
                        result.AppendLine($"Фрагмент: {line.Trim()}");
                        result.AppendLine();
                    }
                }
            }
            catch (Exception ex)
            {
                result.AppendLine($"Не удалось обработать {Path.GetFileName(file)}: {ex.Message}");
            }
        }

        return result.Length == 0 ? "Совпадений не найдено." : result.ToString();
    }
}