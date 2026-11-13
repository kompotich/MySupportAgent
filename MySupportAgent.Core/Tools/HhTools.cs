using System.ComponentModel;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using MySupportAgent.Core.Configuration;
using MySupportAgent.Core.Database;
using MySupportAgent.Core.Database.Entities;
using MySupportAgent.Core.Models;
using MySupportAgent.Core.Models.Params;

namespace MySupportAgent.Core.Tools;

public static class HhTools
{
    private static readonly HhUrlsSettings HhUrlsSettings = Config.Load().HhUrls;
    private static readonly HttpClient HttpClient = new();

    [Description("Получает список вакансий с hh.ru. Возвращает названия вакансий и ссылки на их карточки.")]
    public static async Task<IEnumerable<VacancyItem>> GetVacancies(
        [Description("Тип резюме: CSharpDeveloper, SQLDeveloper или SystemAnalyst.")]
        HhVacancyType vacancyType)
    {
        var result = new List<VacancyItem>();

        var searchUrl = GetSearchUrl(vacancyType);
        var currentPage = 0;

        while (true)
        {
            var url = searchUrl.Replace("{page}", currentPage.ToString());
            
            var response = await HttpClient.GetAsync(url);
            
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var vacancyNodes = htmlDocument.DocumentNode.SelectNodes("//ol[contains(@class, 'vacancies-list')]");

            if (vacancyNodes == null || !vacancyNodes.Nodes().Any())
            {
                break;
            }

            foreach (var node in vacancyNodes.Nodes())
            {
                var titleNode = node.SelectSingleNode(".//span[@data-qa='serp-item__title-text']");
                var linkNode = node.SelectSingleNode("..//a[@data-qa='serp-item__title']");

                if (titleNode == null || linkNode == null)
                {
                    continue;
                }

                var title = titleNode.InnerText;
                var link = linkNode.GetAttributeValue("href", string.Empty);

                result.Add(new VacancyItem(title, link));
            }

            currentPage++;
        }

        return [.. result.DistinctBy(v => v.Url)];
    }

    [Description("Сохраняет список вакансий в БД. Уже существующие вакансии (по ссылке) пропускаются.")]
    public static async Task<int> SaveVacancies(IEnumerable<VacancyItem> vacancies)
    {
        ArgumentNullException.ThrowIfNull(vacancies);

        var items = vacancies
            .Where(v => !string.IsNullOrWhiteSpace(v.Url))
            .DistinctBy(v => v.Url)
            .ToList();

        if (items.Count == 0)
        {
            return 0;
        }

        await using var db = new HhVacanciesDbContext();
        await using var transaction = await db.Database.BeginTransactionAsync();

        var now = DateTime.UtcNow;
        var savedCount = 0;

        foreach (var item in items)
        {
            savedCount += await db.Database.ExecuteSqlInterpolatedAsync($@"
INSERT OR IGNORE INTO Vacancies (Title, Url, SavedAt)
VALUES ({item.Title}, {item.Url}, {now})");
        }

        await transaction.CommitAsync();

        return savedCount;
    }

    private static string GetSearchUrl(HhVacancyType vacancyType)
    {
        return vacancyType switch
        {
            HhVacancyType.CSharpDeveloper =>
                HhUrlsSettings.CSharpDeveloper,
            HhVacancyType.SqlDeveloper =>
                HhUrlsSettings.SqlDeveloper,
            HhVacancyType.SystemAnalyst =>
                HhUrlsSettings.SystemAnalyst,
            HhVacancyType.QAAutomationEngineer =>
                HhUrlsSettings.QAAutomationEngineer,
            HhVacancyType.SupportEngineer =>
                HhUrlsSettings.SupportEngineer,
            _ => throw new NotImplementedException(),
        } ?? throw new InvalidOperationException($"Неизвестный тип вакансии: {vacancyType}");
    }
}