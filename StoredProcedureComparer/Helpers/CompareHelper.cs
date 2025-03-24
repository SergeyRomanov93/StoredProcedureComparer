using System.Text.RegularExpressions;

namespace StoredProcedureComparer.Helpers
{
    public static class CompareHelper
    {
        //public static List<string?> CompareCode(string? fileContent, string? serverContent)
        //{
        //    var differences = new List<string?>();
        //    string[]? fileLines = fileContent?.Split('\n');
        //    string[]? serverLines = serverContent?.Split('\n');

        //    for (int i = 0; i < Math.Max(fileLines.Length, serverLines.Length); i++)
        //    {
        //        string? fileLine = i < fileLines.Length ? fileLines[i].Trim() : null;
        //        string? serverLine = i < serverLines.Length ? serverLines[i].Trim() : null;

        //        if (fileLine != serverLine && !string.IsNullOrWhiteSpace(fileLine) && !string.IsNullOrWhiteSpace(serverLine))
        //        {
        //            // Проверка на наличие даты в формате yy-MM-dd, yy.MM.dd, yyyy.MM.dd, yyyy-MM-dd
        //            var datePattern = @"\b(\d{2}([.-])\d{2}\2\d{2,4})\b";

        //            if (Regex.IsMatch(fileLine, datePattern) || Regex.IsMatch(serverLine, datePattern))
        //            {
        //                differences.Add($"Файл: {fileLine},\n Сервер: {serverLine}");
        //            }
        //            else
        //            {
        //                differences.Add($"Файл: {fileLine},\n Сервер: {serverLine}");
        //            }
        //        }
        //    }

        //    return differences;
        //}

        public static List<string?> CompareContentsResult(List<string> localContents, List<string> serverContents)
        {
            var result = new List<string?>();

            var matches = localContents.Intersect(serverContents).ToHashSet();
            var matchesCount = matches.Count;

            var localSorted = localContents.OrderByDescending(item => matches.Contains(item)).ToList();
            var serverSorted = serverContents.OrderByDescending(item => matches.Contains(item)).ToList();
            // TODO...
            return result;
        }
    }
}
