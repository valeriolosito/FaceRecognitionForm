using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognitionForm.Service
{
    class FeedbackService
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Connection"]
            .ConnectionString;

        public void AddFeedback(string feedback, string cf)
        {
            SqlConnection conn = new SqlConnection(this.connectionString);
            try
            {
                conn.Open();
                string query =
                    "Insert Into Feedback Values(@Feedback, @Cf)";
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter parName = command.Parameters.AddWithValue("@Feedback", feedback);
                parName.DbType = DbType.String;
                parName = command.Parameters.AddWithValue("@Cf", cf);
                parName.DbType = DbType.String;

                command.ExecuteNonQuery();
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

        public List<int> GetFeedback()
        {
            List<int> feedbackNumbers = new List<int>();

            SqlConnection conn = new SqlConnection(this.connectionString);
            try
            {
                conn.Open();
                string query =
                    "SELECT COUNT(*) FROM Feedback WHERE Feedback = @Feedback";
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter parName = command.Parameters.AddWithValue("@Feedback", "yes");
                parName.DbType = DbType.String;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        feedbackNumbers.Add(Int32.Parse(reader[0].ToString()));
                    }
                }

                query =
                    "SELECT COUNT(*) FROM Feedback WHERE Feedback = @Feedback";
                command = new SqlCommand(query, conn);
                parName = command.Parameters.AddWithValue("@Feedback", "no");
                parName.DbType = DbType.String;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        feedbackNumbers.Add(Int32.Parse(reader[0].ToString()));
                    }
                }
                feedbackNumbers.Add(feedbackNumbers[0] + feedbackNumbers[1]);
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

        public List<int> GetFeedbackSingleUser(string cf)
        {
            List<int> feedbackNumbers = new List<int>();

            SqlConnection conn = new SqlConnection(this.connectionString);
            try
            {
                conn.Open();
                string query =
                    "SELECT COUNT(*) FROM Feedback WHERE Feedback = @Feedback and CF = @Cf";
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter parName = command.Parameters.AddWithValue("@Feedback", "yes");
                parName.DbType = DbType.String;
                parName = command.Parameters.AddWithValue("@Cf", cf);
                parName.DbType = DbType.String;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        feedbackNumbers.Add(Int32.Parse(reader[0].ToString()));
                    }
                }

                query =
                    "SELECT COUNT(*) FROM Feedback WHERE Feedback = @Feedback and CF = @Cf";
                command = new SqlCommand(query, conn);
                parName = command.Parameters.AddWithValue("@Feedback", "no");
                parName.DbType = DbType.String;
                parName = command.Parameters.AddWithValue("@Cf", cf);
                parName.DbType = DbType.String;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        feedbackNumbers.Add(Int32.Parse(reader[0].ToString()));
                    }
                }
                feedbackNumbers.Add(feedbackNumbers[0] + feedbackNumbers[1]);
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
