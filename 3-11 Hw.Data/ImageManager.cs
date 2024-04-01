using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace _3_11_Hw.Data
{
    public class Images
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int Views { get; set; }
    }
    public class ImageManager
    {
        private string _connectionString;
        public ImageManager(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int AddImage(Images i)
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT Into Image(Name, Password, Views) VALUES(@name, @password, @views) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@name", i.Name);
            cmd.Parameters.AddWithValue("@password", i.Password);
            cmd.Parameters.AddWithValue("@views", 0);
            connection.Open();
            int id = (int)(decimal)cmd.ExecuteScalar();
            return id;
        }
        public Images GetImageById(int id)
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Image WHERE Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            var image = new Images();
            image.Id = (int)reader["Id"];
            image.Name = (string)reader["Name"];
            image.Password = (string)reader["Password"];
            image.Views = (int)reader["Views"];
            return image;

        }

        public void SetViews(int id)
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Image SET Views= Views + 1 WHERE Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
