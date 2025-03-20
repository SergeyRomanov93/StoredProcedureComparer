using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoredProcedureComparer.Helpers
{
    public static class CompareHelper
    {
        public static List<string?> CompareCode(string? fileContent, string? serverContent)
        {
            var differences = new List<string?>();
            string[]? fileLines = fileContent?.Split('\n');
            string[]? serverLines = serverContent?.Split('\n');

            for (int i = 0; i < Math.Max(fileLines.Length, serverLines.Length); i++)
            {
                string? fileLine = i < fileLines.Length ? fileLines[i].Trim() : null;
                string? serverLine = i < serverLines.Length ? serverLines[i].Trim() : null;

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
}
