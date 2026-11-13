using Microsoft.Agents.AI;
using MySupportAgent.Core.Agents;
using MySupportAgent.Core.Database;

namespace MySupportAgent.Console;

internal static class Program
{
    private static async Task Main()
    {
        await using (var db = new QuickNoteDbContext())
        {
            await db.Database.EnsureCreatedAsync();
        }

        await using (var db = new HhVacanciesDbContext())
        {
            await db.Database.EnsureCreatedAsync();
        }

        var agent = new MainAgent().CreateAgent();
        
        
        System.Console.WriteLine("MySupportAgent запущен.");
        System.Console.WriteLine("'exit' для выхода.\n");

        while (true)
        {
            System.Console.Write("Я: ");

            var input = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            try
            {
                var response = await agent.RunAsync(input);

                System.Console.WriteLine($"Агент: {response}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
