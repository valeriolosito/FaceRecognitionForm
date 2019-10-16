namespace FaceRecognitionForm
{
    class Person
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Cf { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Telephone { get; set; }
        public string Profession { get; set; }
        public string Email { get; set; }

        public Person(string name, string surname, string cf, string address, string city, string telephone, string profession, string email)
        {
            
            this.Name = name;
            this.Surname = surname;
            this.Cf = cf;
            this.Address = address;
            this.City = city;
            this.Telephone = telephone;
            this.Profession = profession;
            this.Email = email;
        }

        public Person() { }
    }
}
