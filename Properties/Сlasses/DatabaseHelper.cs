using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace TEST.Properties.Сlasses
{
    public static class DatabaseHelper
    {
        // Вывод всей таблицы
        public static DataTable LoadAllContent() 
        {
            string connectionString = ConfigurationManager.ConnectionStrings["UserDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Content";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        // Поиск по всем столбцам
        public static DataTable SearchContent(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return LoadAllContent(); // Если поиск пустой, возвращаем все
            }

            string connectionString = ConfigurationManager.ConnectionStrings["UserDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT * FROM Content
                WHERE 
                    Title LIKE @search OR
                    Author LIKE @search OR
                    Description LIKE @search OR
                    Type LIKE @search OR
                    Platform LIKE @search OR
                    CONVERT(varchar, Publication_date, 105) LIKE @search OR
                    CONVERT(varchar, Views) LIKE @search";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@search", "%" + searchText + "%");

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
        }

    }

    // Добавление записи
    public class ContentAdder
    {
        public void Add(string title, string author, string description,
                       string type, string platform, DateTime date, int views)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["UserDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Content (Title, Author, Description, Type, Platform, Publication_date, Views) " +
                               "VALUES (@Title, @Author, @Description, @Type, @Platform, @Date, @Views)";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Author", author);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@Type", type);
                command.Parameters.AddWithValue("@Platform", platform);
                command.Parameters.AddWithValue("@Date", date);
                command.Parameters.AddWithValue("@Views", views);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
    public class ContentReader
    {
        public DataRow GetContentById(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["UserDB"].ConnectionString;
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Явно указываем все столбцы
                string query = "SELECT Id, Title, Author, Description, Type, Platform, Publication_date, Views FROM Content WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(table);
            }

            return table.Rows.Count > 0 ? table.Rows[0] : null;
        }
    }
}
