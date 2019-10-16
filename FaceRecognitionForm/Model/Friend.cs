namespace FaceRecognitionForm.Model
{
    public class Friend
    {
        public Friend(dynamic name1, dynamic id1)
        {
            this.Name = name1;
            this.Id = id1;
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}
