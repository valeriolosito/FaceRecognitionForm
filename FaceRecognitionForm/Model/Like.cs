using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognitionForm.Model
{
    public class Like
    {
        public string id { get; set; }
        public string name { get; set; }
        public DateTime created_time { get; set; }

        public Like(string id, string name, DateTime created_time)
        {
            this.id = id;
            this.name = name;
            this.created_time = created_time;
        }   
    }
}
