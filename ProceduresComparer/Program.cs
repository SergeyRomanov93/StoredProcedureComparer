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

        Console.Write("Введите путь к папку с файлами SQL: ");
        string? folderPath = Console.ReadLine();

        string? connectionString = configuration.GetConnectionString("SqlServer");

        // Получение текущих определений хранимок/функций из локального репозитория
        var localDefinitions = LocalRepositoryHelper.GetLocalStoredProceduresAndFunctions(folderPath);

        // Получение текущих определений хранимок/функций с сервера
        var serverDefinitions = DbHelper.GetServerStoredProceduresAndFunctions(connectionString);
        var e = CompareHelper.CompareContentsResult(localDefinitions, serverDefinitions);
        ;
        // Сравнение и формирование отчёта
    //    foreach (var file in localDefinitions)
    //    {
    //        string fileName = Path.GetFileName(file.Key);
    //        string fileContent = File.ReadAllText(file.Value).Trim().Replace("\r\n", "\n").Replace("\t", "");

    //        // Извлечение имени хранимки/функции из файла (предполагается, что имя совпадает с именем файла без расширения)
    //        string procedureName = Path.GetFileNameWithoutExtension(fileName);

    //        Console.WriteLine($"Проверка файла: {fileName}");
    //        Console.WriteLine($"Имя процедуры/функции: {procedureName}");

    //        if (!serverDefinitions.ContainsKey(procedureName))
    //        {
    //            Console.WriteLine($"  Процедура/функция {procedureName} отсутствует на сервере.");
    //            continue;
    //        }

    //        string serverContent = serverDefinitions[procedureName].Trim().Replace("\r\n", "\n").Replace("\t", "");

    //        if (fileContent == serverContent)
    //        {
    //            Console.WriteLine("  Отличий в коде нет.");
    //        }
    //        else
    //        {
    //            Console.WriteLine("  Есть отличия в коде:");
    //            var differences = CompareHelper.CompareCode(fileContent, serverContent);
    //            Console.WriteLine($"    Количество отличающихся строк: {differences.Count}");

    //            foreach (var diff in differences)
    //            {
    //                Console.WriteLine($"    Строка: {diff}");
    //            }
    //        }

    //        Console.WriteLine();
    //    }
    }
}