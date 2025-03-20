using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        // Загрузка конфигурации
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = configuration.GetConnectionString("SqlServer");
        string folderPath = configuration["Settings:FolderPath"];

        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine($"Папка {folderPath} не существует.");
            return;
        }

        // Получение списка файлов
        var files = Directory.GetFiles(folderPath, "*.sql");

        // Получение текущих определений хранимок/функций с сервера
        var serverDefinitions = GetServerStoredProcedures(connectionString);

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
                var differences = CompareCode(fileContent, serverContent);
                Console.WriteLine($"    Количество отличающихся строк: {differences.Count}");

                foreach (var diff in differences)
                {
                    Console.WriteLine($"    Строка: {diff}");
                }
            }

            Console.WriteLine();
        }
    }

    static Dictionary<string, string> GetServerStoredProcedures(string connectionString)
    {
        var definitions = new Dictionary<string, string>();

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Запрос для получения всех хранимых процедур и функций
            string query = @"
                SELECT 
                    o.name AS ProcedureName,
                    m.definition AS Definition
                FROM sys.sql_modules m
                INNER JOIN sys.objects o ON m.object_id = o.object_id
                WHERE o.type IN ('P', 'FN')"; // P - процедуры, FN - функции

            using (var command = new SqlCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = reader["ProcedureName"].ToString();
                    string definition = reader["Definition"].ToString();
                    definitions[name] = definition.Trim().Replace("\r\n", "\n").Replace("\t", "");
                }
            }
        }

        return definitions;
    }

    static List<string> CompareCode(string fileContent, string serverContent)
    {
        var differences = new List<string>();
        var fileLines = fileContent.Split('\n');
        var serverLines = serverContent.Split('\n');

        for (int i = 0; i < Math.Max(fileLines.Length, serverLines.Length); i++)
        {
            string fileLine = i < fileLines.Length ? fileLines[i].Trim() : null;
            string serverLine = i < serverLines.Length ? serverLines[i].Trim() : null;

            if (fileLine != serverLine && !string.IsNullOrWhiteSpace(fileLine) && !string.IsNullOrWhiteSpace(serverLine))
            {
                // Проверка на наличие даты в формате yy-MM-dd, yy.MM.dd, yyyy.MM.dd, yyyy-MM-dd
                var datePattern = @"\b(\d{2}([.-])\d{2}\2\d{2,4})\b";
                if (Regex.IsMatch(fileLine, datePattern) || Regex.IsMatch(serverLine, datePattern))
                {
                    differences.Add($"Файл: {fileLine}, Сервер: {serverLine}");
                }
                else
                {
                    differences.Add($"Файл: {fileLine}, Сервер: {serverLine}");
                }
            }
        }

        return differences;
    }
}