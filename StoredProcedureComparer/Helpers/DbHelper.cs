﻿using Microsoft.Data.SqlClient;

namespace StoredProcedureComparer.Helpers
{
    public static class DbHelper
    {
        public static List<string> GetServerStoredProceduresAndFunctions(string? connectionString)
        {
            Dictionary<string, string?> definitions = new Dictionary<string, string?>();
            var contents = new List<string>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Запрос для получения всех хранимых процедур и функций
                string query = @"
                SELECT 
                    o.name AS ProcedureName,
                    m.definition AS ProcedureDefinition
                FROM 
                    sys.sql_modules m
                INNER JOIN 
                    sys.objects o ON m.object_id = o.object_id
                WHERE 
                    o.type = 'P' -- 'P' означает хранимую процедуру
                OR o.type = 'FN' -- 'FN' означает функцию
                ORDER BY 
                    o.name;";

                using (SqlCommand? command = new SqlCommand(query, connection))
                using (SqlDataReader? reader = command?.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader["ProcedureName"].ToString();
                        string? definition = reader?["ProcedureDefinition"].ToString();
                        definitions[name] = definition?.Trim().Replace("\r\n", "\n").Replace("\t", "");

                        contents = definitions.Values.Where(d => d.Contains("CREATE") || d.Contains("ALTER")).ToList();
                    }
                }
            }

            return contents;
        }
    }
}
