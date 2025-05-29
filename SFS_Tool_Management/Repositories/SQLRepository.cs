using System;
using System.Collections.Generic;
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
            MessageBox.Show(_connectionString);
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
            }
            return result;
        }

        private void GetConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(string.Empty);

            //string password = Environment.GetEnvironmentVariable("DB_Password");

            //MessageBox.Show(password);

            builder["Server"] = "tcp:***REMOVED***,1443";
            builder["Initial Catalog"] = "SFS";
            builder["Persist Security Info"] = false;
            builder["User Id"] = "***REMOVED***";
            builder["Password"] = "";
            builder["MultipleActiveResultSets"] = false;
            builder["Encrypt"] = true;
            builder["TrustServerCertificate"] = false;
            builder["Connection Timeout"] = 30;

            _connectionString = builder.ConnectionString;
        }
    }
}
