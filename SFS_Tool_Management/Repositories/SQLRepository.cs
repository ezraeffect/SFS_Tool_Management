using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.IO.Packaging;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace SFS_Tool_Management.Repositories
{
    public class SQLRepository
    {

        public async Task<List<T>> ExecuteQueryAsync<T>(string query, Func<SqlDataReader, T> map)
        {
            using var conn = new SqlConnection(BuildConnectionString());
            using var cmd = new SqlCommand(query, conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            var result = new List<T>();
            while (await reader.ReadAsync())
            {
                result.Add(map(reader));

                // Only USE DEBUG
                // ReadMultiRow((IDataRecord)reader);
            }
            return result;
        }

        public void InsertTestQuery()
        {
            SqlCommand cmd2 = new SqlCommand();
            var conn = new SqlConnection(BuildConnectionString());
            MessageBox.Show(BuildConnectionString());
            cmd2.Connection = conn;
            conn.Open();



            SqlParameter paraId = new SqlParameter("UserID", SqlDbType.VarChar, 30);
            SqlParameter paraPW = new SqlParameter("PasswordHash", SqlDbType.VarChar, 64);
            SqlParameter paraName = new SqlParameter("Name", SqlDbType.NVarChar, 30);
            SqlParameter paraPosition = new SqlParameter("Position", SqlDbType.NVarChar, 100);
            SqlParameter paraDepartment = new SqlParameter("Department", SqlDbType.NVarChar, 100);
            SqlParameter paraPhoneNumber = new SqlParameter("PhoneNumber", SqlDbType.VarChar, 30);
            SqlParameter paraIsAdmin = new SqlParameter("IsAdmin", SqlDbType.Bit);

            paraId.Value = "test05";
            string pw = "qwer1234!";
            byte[] bytes = null;
            using(SHA256 Hash = SHA256.Create())
            {
                bytes = Hash.ComputeHash(Encoding.UTF8.GetBytes(pw));
            }
            paraPW.Value = Convert.ToBase64String(bytes);
            MessageBox.Show(Convert.ToBase64String(bytes));
            paraName.Value = "이문세";
            paraPosition.Value = "부장";
            paraDepartment.Value = "전략기획부";
            paraPhoneNumber.Value = "01012341234";
            paraIsAdmin.Value = false;

            // cmd.Parameters 컬렉션에 SqlParameter 개체 추가
            cmd2.Parameters.Add(paraId);
            cmd2.Parameters.Add(paraPW);
            cmd2.Parameters.Add(paraName);
            cmd2.Parameters.Add(paraPosition);
            cmd2.Parameters.Add(paraDepartment);
            cmd2.Parameters.Add(paraPhoneNumber);
            cmd2.Parameters.Add(paraIsAdmin);

            string tableName = "UserList";

            cmd2.CommandText = "INSERT INTO [DBO].[" + tableName + "] (UserID,PasswordHash,Name,Position,Department,PhoneNumber,IsAdmin) VALUES (@UserID,@PasswordHash,@Name,@Position,@Department,@PhoneNumber,@IsAdmin);";
            // 정상적으로 쿼리문이 입력되면 반영된 행의 수가 리턴값으로 들어온다.
            int a = cmd2.ExecuteNonQuery();
            if (a > 0)
            {
                MessageBox.Show(String.Format("쿼리문이 정상적으로 입력되었습니다. 반영된 행의 갯수는 {0}개 입니다", a));
            }
            else MessageBox.Show("쿼리문이 반영되지 않았습니다. 다시 한번 확인해주세요.");
        }

        private static void ReadSingleRow(IDataRecord record)
        {
            MessageBox.Show(String.Format("{0}", record[0]));
        }

        private static void ReadMultiRow(IDataRecord record)
        {
            int fieldCount = record.FieldCount;
            List<string> values = new List<string>();

            for (int i = 0; i < fieldCount; i++)
            {
                values.Add(record[i]?.ToString());
            }

            string result = string.Join(", ", values);
            MessageBox.Show(result, "Result");
        }

        public static string BuildConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                ["Server"] = Config.Load("Server"),
                ["Initial Catalog"] = Config.Load("Initial Catalog"),
                ["Persist Security Info"] = bool.Parse(Config.Load("Persist Security Info")),
                ["User Id"] = Config.Load("User ID"),
                ["Password"] = Config.Load("Password"),
                ["MultipleActiveResultSets"] = bool.Parse(Config.Load("MultipleActiveResultSets")),
                ["Encrypt"] = bool.Parse(Config.Load("Encrypt")),
                ["TrustServerCertificate"] = bool.Parse(Config.Load("TrustServerCertificate")),
                ["Connection Timeout"] = int.Parse(Config.Load("Connection Timeout"))
            };

            return builder.ConnectionString;
        }
    }
}
