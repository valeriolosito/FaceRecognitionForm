using Affdex;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FaceRecognitionForm.Model
{
    class PhotoProcessed : ImageListener
    {
        public Dictionary<string, float> emoctionPropertis;

        public PhotoProcessed(Detector detector)
        {
            detector.setImageListener(this);
            emoctionPropertis = new Dictionary<string, float>();
        }
        public void onImageCapture(Frame frame)
        {
            Console.Write(frame.getHeight());
            frame.Dispose();
        }

        public void onImageResults(Dictionary<int, Face> faces, Frame frame)
        {
            
            foreach (KeyValuePair<int, Face> pair in faces)
            {
                Face face = pair.Value;
                if (faces != null)
                {
                    foreach (PropertyInfo prop in typeof(Emotions).GetProperties())
                    {
                        float value = (float)prop.GetValue(face.Emotions, null);
                        this.emoctionPropertis.Add(prop.Name, value);
                        string output = String.Format("{0}:{1}", prop.Name, value);
                        System.Console.WriteLine(output);
                    }
                }
            }
            frame.Dispose();
        }
    }
}
