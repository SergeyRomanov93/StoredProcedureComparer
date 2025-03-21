namespace StoredProcedureComparer.Helpers
{
    public static class LocalRepositoryHelper
    {
        public static Dictionary<string, string?> GetLocalStoredProceduresAndFunctions(string? folderPath, out Dictionary<string, string?> doubles)
        {
            doubles = new Dictionary<string, string?>();
            Dictionary<string, string?> definitions = new Dictionary<string, string?>();

            if (!Directory.Exists(folderPath))
            {
                throw new Exception($"Папка {folderPath} не существует.");
            }

            var files = Directory.EnumerateFiles(folderPath, "*.sql", SearchOption.AllDirectories).ToArray();


            foreach (var file in files.Where(f => f.Contains("CP_") || f.Contains("fn_") 
                                                                    || f.Contains("[dbo].[cp_") 
                                                                    || f.Contains("[dbo].[fn_")
                                                                    || f.Contains("cp_")))
            {
                var fileName = Path.GetFileName(file);
                var fileContent = File.ReadAllText(file).Trim().Replace("\r\n", "\n").Replace("\t", "");

                if (definitions.Keys.Contains(fileName))
                {
                    doubles.Add(fileName, fileContent);
                    continue;
                }

                definitions.Add(fileName, fileContent);
            }

            return definitions;
        }
    }
}
