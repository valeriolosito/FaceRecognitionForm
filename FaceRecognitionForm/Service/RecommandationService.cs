﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognitionForm.Service
{
    public class RecommandationService
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Connection"]
            .ConnectionString;

        //restituisci il titolo, il genere e gli attori di un film casuale che appartiene a quel genere
        public List<string> GetFilm(string genre)
        {
            List<string> feedbackNumbers = new List<string>();

            SqlConnection conn = new SqlConnection(this.connectionString);
            try
            {
                conn.Open();
                string query =
                    "SELECT TOP 1 Title, Genres, Actor_1_Name, Actor_2_Name, Actor_3_Name FROM [FaceRecognition].[dbo].[Film] where Genres like @Genre ORDER BY NEWID()";
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter parName = command.Parameters.AddWithValue("@Genre", genre);
                parName.DbType = DbType.String;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        feedbackNumbers.Add(reader["Title"].ToString());
                        feedbackNumbers.Add(reader["Genres"].ToString());

                        if (reader["Actor_1_Name"] == "")
                            feedbackNumbers.Add("");
                        else if (reader["Actor_2_Name"] == "")
                            feedbackNumbers.Add(reader["Actor_1_Name"].ToString());
                        else if (reader["Actor_3_Name"] == "")
                            feedbackNumbers.Add(reader["Actor_1_Name"] + ", " + reader["Actor_2_Name"]);
                        else
                            feedbackNumbers.Add(reader["Actor_1_Name"] + ", " + reader["Actor_2_Name"] + ", " + reader["Actor_3_Name"]);
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
            return feedbackNumbers;
        }
    }
}
