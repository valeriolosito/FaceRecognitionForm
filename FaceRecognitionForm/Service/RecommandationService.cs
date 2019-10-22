using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceRecognitionForm.Model;

namespace FaceRecognitionForm.Service
{
    public class RecommandationService
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Connection"]
            .ConnectionString;

        public List<string> GetGenres(List<Movie> titleList)
        {
            List<string> resultList = new List<string>();

            if (titleList.Count != 0)
            {
                string titles = String.Empty;
                for (int i = 0; i < titleList.Count - 1; i++)
                    titles += "'" + titleList[i].Id + "',";

                titles += "'" + titleList.Last().Id + "'";

                SqlConnection conn = new SqlConnection(this.connectionString);

                try
                {
                    conn.Open();
                    string query =
                        "SELECT Genres FROM [FaceRecognition].[dbo].[Film] where Title IN (" + titles + ")";
                    SqlCommand command = new SqlCommand(query, conn);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultList.Add(reader["Genres"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }

            return resultList;
        }

        //restituisce il titolo, il genere e gli attori di un film casuale che appartiene a quel genere
        public List<string> GetMovie(string genre)
        {
            List<string> movies = new List<string>();

            SqlConnection conn = new SqlConnection(this.connectionString);
            try
            {
                conn.Open();
                string query = "SELECT TOP 1 Title, Genres, Actor_1_Name, Actor_2_Name, Actor_3_Name, Movie_imbd_link FROM [FaceRecognition].[dbo].[Film] where Genres like'%" + genre + "%' ORDER BY NEWID()";
                
                SqlCommand command = new SqlCommand(query, conn);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        movies.Add(reader["Title"].ToString());
                        movies.Add(reader["Genres"].ToString());
                       
                        if (reader["Actor_1_Name"] == "")
                            movies.Add("");
                        else if (reader["Actor_2_Name"] == "")
                            movies.Add(reader["Actor_1_Name"].ToString());
                        else if (reader["Actor_3_Name"] == "")
                            movies.Add(reader["Actor_1_Name"] + ", " + reader["Actor_2_Name"]);
                        else
                            movies.Add(reader["Actor_1_Name"] + ", " + reader["Actor_2_Name"] + ", " + reader["Actor_3_Name"]);

                        movies.Add(reader["Movie_imbd_link"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return movies;
        }

        /*
         *   public string GetMovieLink(string title)
          {
              string result = string.Empty;
              SqlConnection conn = new SqlConnection(this.connectionString);
              try
              {
                  conn.Open();
                  string query =
                      "SELECT top(1) Movie_imbd_link FROM [FaceRecognition].[dbo].[Film] where Title = @Title";
                  SqlCommand command = new SqlCommand(query, conn);
                  SqlParameter parName = command.Parameters.AddWithValue("@Title", title);
                  parName.DbType = DbType.String;
                  using (SqlDataReader reader = command.ExecuteReader())
                  {
                      if (reader.Read())
                      {
                          result = reader["Movie_imbd_link"].ToString();
                      }
                  }
              }
              catch (Exception ex)
              {
                  Console.Write(ex.Message);
              }
              finally
              {
                  conn.Close();
              }

              return result;
          }
         */
        public List<string> set(string genre)
        {
            List<string> movies = new List<string>();

            SqlConnection conn = new SqlConnection(this.connectionString);
            try
            {
                conn.Open();
                string query =
                    "SELECT TOP 1 Title, Genres, Actor_1_Name, Actor_2_Name, Actor_3_Name, Movie_imbd_link FROM [FaceRecognition].[dbo].[Film] where Genres like'%" + genre + "%' ORDER BY NEWID()";
                SqlCommand command = new SqlCommand(query, conn);
                //SqlParameter parName = command.Parameters.AddWithValue("@Genre", genre);
                //parName.DbType = DbType.String;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        movies.Add(reader["Title"].ToString());
                        movies.Add(reader["Genres"].ToString());

                        if (reader["Actor_1_Name"] == "")
                            movies.Add("");
                        else if (reader["Actor_2_Name"] == "")
                            movies.Add(reader["Actor_1_Name"].ToString());
                        else if (reader["Actor_3_Name"] == "")
                            movies.Add(reader["Actor_1_Name"] + ", " + reader["Actor_2_Name"]);
                        else
                            movies.Add(reader["Actor_1_Name"] + ", " + reader["Actor_2_Name"] + ", " + reader["Actor_3_Name"]);

                        movies.Add(reader["Movie_imbd_link"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return movies;
        }

    }
}
