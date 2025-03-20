using Microsoft.Extensions.Configuration;
using StoredProcedureComparer.Helpers;

class Program
{
    static void Main(string[] args)
    {
        // Загрузка конфигурации
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        string? connectionString = configuration.GetConnectionString("SqlServer");
        string? folderPath = "C:\\Users\\svsil\\Documents\\SQL Server Management Studio\\StoredProcedures"; //Путь к папке с хранимыми процедурами

        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine($"Папка {folderPath} не существует.");
            return;
        }

        // Получение списка файлов
        var files = Directory.GetFiles(folderPath, "*.sql");

        // Получение текущих определений хранимок/функций с сервера
        var serverDefinitions = DbHelper.GetServerStoredProcedures(connectionString);

        // Сравнение и формирование отчёта
        foreach (var file in files)
        {
            string fileName = Path.GetFileName(file);
            string fileContent = File.ReadAllText(file).Trim().Replace("\r\n", "\n").Replace("\t", "");

            // Извлечение имени хранимки/функции из файла (предполагается, что имя совпадает с именем файла без расширения)
            string procedureName = Path.GetFileNameWithoutExtension(fileName);

            Console.WriteLine($"Проверка файла: {fileName}");
            Console.WriteLine($"Имя процедуры/функции: {procedureName}");

            if (!serverDefinitions.ContainsKey(procedureName))
            {
                Console.WriteLine($"  Процедура/функция {procedureName} отсутствует на сервере.");
                continue;
            }

            string serverContent = serverDefinitions[procedureName].Trim().Replace("\r\n", "\n").Replace("\t", "");

            if (fileContent == serverContent)
            {
                Console.WriteLine("  Отличий в коде нет.");
            }
            else
            {
                Console.WriteLine("  Есть отличия в коде:");
                var differences = CompareHelper.CompareCode(fileContent, serverContent);
                Console.WriteLine($"    Количество отличающихся строк: {differences.Count}");

                foreach (var diff in differences)
                {
                    Console.WriteLine($"    Строка: {diff}");
                }
            }

            Console.WriteLine();
        }
    }
}