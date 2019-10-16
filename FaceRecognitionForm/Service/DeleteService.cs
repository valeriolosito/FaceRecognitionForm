using System;
using System.Data;
using System.Data.SqlClient;

namespace FaceRecognitionForm.Service
{
    class DeleteService
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Connection"]
            .ConnectionString;
        
        public int RemoveUser(string taxCode)
        {
            int rowsAffectedPerson = 0;
            int rowsAffectedImage = 0;
            SqlConnection conn = new SqlConnection(this.connectionString);
            try
            {
                conn.Open();

                string query = "Delete FROM Person WHERE CF = @TaxCode";
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter parTaxCode = command.Parameters.AddWithValue("@TaxCode", taxCode);
                parTaxCode.DbType = DbType.String;

                rowsAffectedPerson = command.ExecuteNonQuery();

                query = "Delete FROM Image WHERE CF_Person = @TaxCode";
                command = new SqlCommand(query, conn);
                parTaxCode = command.Parameters.AddWithValue("@TaxCode", taxCode);
                parTaxCode.DbType = DbType.String;

                rowsAffectedImage = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return (rowsAffectedPerson + rowsAffectedImage);
        }
    }
}
