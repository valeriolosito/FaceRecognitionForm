using System;
using System.Data;
using System.Data.SqlClient;

namespace FaceRecognitionForm.Service
{
    class RegisterService
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Connection"]
                    .ConnectionString;

        public void addUser(Person person, byte[] imgData)
        {
            addUser(person);
            //int idPerson = getIdUser(person.Cf);
            addImage(person.Cf, imgData);

        }

        private void addUser(Person person)
        {
            SqlConnection conn = new SqlConnection(this.connectionString);
            try
            {

                conn.Open();
                string query =
                    "Insert Into Person (Name, Surname, CF, Address, City, Telephone, Profession, Email) Values(@Name, @Surname, @CF, @Address, @City, @Telephone, @Profession, @Email)";
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter parName = command.Parameters.AddWithValue("@Name", person.Name);
                parName.DbType = DbType.String;
                SqlParameter parSurname = command.Parameters.AddWithValue("@Surname", person.Surname);
                parSurname.DbType = DbType.String;
                SqlParameter parCF = command.Parameters.AddWithValue("@CF", person.Cf);
                parCF.DbType = DbType.String;
                SqlParameter parAddress = command.Parameters.AddWithValue("@Address", person.Address);
                parAddress.DbType = DbType.String;
                SqlParameter parCity = command.Parameters.AddWithValue("@City", person.City);
                parCity.DbType = DbType.String;
                SqlParameter parTelephone =
                    command.Parameters.AddWithValue("@Telephone", person.Telephone);
                parTelephone.DbType = DbType.String;
                SqlParameter parProfession =
                    command.Parameters.AddWithValue("@Profession", person.Profession);
                parProfession.DbType = DbType.String;
                SqlParameter parEmail =
                    command.Parameters.AddWithValue("@Email", person.Email);
                parEmail.DbType = DbType.String;
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

        public string getTaxCode(string taxCode)
        {
            string result = null;
            SqlConnection conn = new SqlConnection(this.connectionString);
            try
            {
                conn.Open();
                string query = "SELECT CF FROM Person WHERE CF = @CF";
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter parName = command.Parameters.AddWithValue("@CF", taxCode);
                parName.DbType = DbType.String;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = reader["CF"].ToString();
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        private void addImage(string CF_person, byte[] imgData)
        {
            SqlConnection conn = new SqlConnection(this.connectionString);
            try
            {
                conn.Open();
                string query = "Insert Into Image Values(@CF_Person, @Image)";
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter parCF = command.Parameters.AddWithValue("@CF_Person", CF_person);
                parCF.DbType = DbType.String;
                SqlParameter parImage = command.Parameters.AddWithValue("@Image", imgData);
                parImage.DbType = DbType.Binary;
                command.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public Person GetInfoUserReg(string email)
        {
            Person p = null;
            SqlConnection conn = new SqlConnection(this.connectionString);
            try
            {
                conn.Open();
                string query = "SELECT Name, Surname, CF, Address, City, Telephone, Profession FROM Person WHERE Email = @email";
                SqlCommand command = new SqlCommand(query, conn);
                SqlParameter parName = command.Parameters.AddWithValue("@email", email);
                parName.DbType = DbType.String;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        p = new Person(
                                reader["Name"].ToString(),
                                reader["Surname"].ToString(),
                                    reader["CF"].ToString(),
                                    reader["Address"].ToString(),
                                    reader["City"].ToString(),
                                     reader["Telephone"].ToString(),
                                    reader["Profession"].ToString(),
                                email
                                );

                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            finally
            {
                conn.Close();
            }
            return p;
        }
    }

}
