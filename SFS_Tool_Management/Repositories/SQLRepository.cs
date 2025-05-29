using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace SFS_Tool_Management.Repositories
{
    public class SQLRepository
    {
        private string? _connectionString {  get; set; }

        public SQLRepository()
        {
            GetConnectionString();
        }

        public async Task<List<T>> ExecuteQueryAsync<T>(string query, Func<SqlDataReader, T> map)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(query, conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            var result = new List<T>();
            while (await reader.ReadAsync())
            {
                result.Add(map(reader));
                ReadSingleRow((IDataRecord)reader);
            }
            return result;
        }

        private static void ReadSingleRow(IDataRecord record)
        {
            MessageBox.Show(String.Format("{0}", record[0]));
        }

        private void GetConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(string.Empty);

            builder["Server"] = "tcp:sfstool.database.windows.net,1433";
            builder["Initial Catalog"] = "SFS";
            builder["Persist Security Info"] = false;
            builder["User Id"] = "Codingon";
            builder["Password"] = "sfs2751!";
            builder["MultipleActiveResultSets"] = false;
            builder["Encrypt"] = true;
            builder["TrustServerCertificate"] = false;
            builder["Connection Timeout"] = 30;

            _connectionString = builder.ConnectionString;
        }
    }
}
