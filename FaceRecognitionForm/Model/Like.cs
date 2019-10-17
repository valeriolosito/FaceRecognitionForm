using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognitionForm.Model
{
    public class Like
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedTime { get; set; }

        public Like(string id, string name, DateTime created_time)
        {
            this.Id = id;
            this.Name = name;
            this.CreatedTime = created_time;
        }   
    }
}
