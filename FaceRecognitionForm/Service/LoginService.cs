using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

namespace FaceRecognitionForm.Service
{
    
    class LoginService
    {
        private List<Person> listPerson = new List<Person>();
        private int lastIndexPersonFound;

        public List<Image> getImage()
        {
            string query = "SELECT Image, p.* FROM Image i join Person p on i.CF_Person = p.CF";
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Connection"]
                .ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(query, conn);

            Image image = null;
            MemoryStream str = new MemoryStream();

            List<Image> listImage = new List<Image>();
            
            int i = 0;

            conn.Open();

            try
            {
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    listPerson = new List<Person>();
                    while (reader.Read())
                    {
                        i++;
                        str = new MemoryStream();
                        str.Write((byte[])reader[0], 0, ((byte[])reader[0]).Length);
                        image = Image.FromStream(str);
                        listImage.Add(image);

                        Person p = new Person()
                        {
                            Name = reader[2].ToString(),
                            Surname = reader[3].ToString(),
                            Cf = reader[4].ToString(),
                            Address = reader[5].ToString(),
                            City = reader[6].ToString(),
                            Telephone = reader[7].ToString(),
                            Profession = reader[8].ToString()
                        };
                        listPerson.Add(p);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            conn.Close();
            return listImage;
        }

        public int Login(Image currentImage)
        {
            List<Image> imgDb;
            int equal = 0, i = 0, result = 0;
            imgDb = getImage();
            foreach (var img in imgDb)
            {
                equal = UtilityRecognition.Compare(img, currentImage);
                result = equal;
                if (equal == 1 || equal == 2)
                {
                    break;
                }
                i++;
            }
            this.lastIndexPersonFound = i;
            return result;
        }

        public Person getLastPersonaFound()
        {
            return listPerson[lastIndexPersonFound];
        }
    }
}
