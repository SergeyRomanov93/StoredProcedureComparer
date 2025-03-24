namespace StoredProcedureComparer.Helpers
{
    public static class LocalRepositoryHelper
    {
        public static List<string>? GetLocalStoredProceduresAndFunctions(string? folderPath)
        {
            List<string>? definitions = new List<string>();

            if (!Directory.Exists(folderPath))
            {
                throw new Exception($"Папка {folderPath} не существует.");
            }

            var files = Directory.EnumerateFiles(folderPath, "*.sql", SearchOption.AllDirectories).ToArray();

            var filesContents = files.Select(f => File.ReadAllText(f).Trim().Replace("\r\n", "\n").Replace("\t", ""));
            definitions = filesContents.Where(fc => fc.Contains("CREATE") || fc.Contains("ALTER")).ToList();

            return definitions;
        }
    }
}
