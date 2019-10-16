using Affdex;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FaceRecognitionForm.Model
{
    class Affectiva
    {
        private enum AffectivaProperties
        {
            Engagement,
            Valence,
            Contempt,
            Surprise,
            Anger,
            Sadness,
            Disgust,
            Fear,
            Joy
        }
        public string Engagement { get; set; } 
        public string Valence { get; set; }
        public string Contempt { get; set; }
        public string Surprise { get; set; }
        public string Anger { get; set; }
        public string Sadness { get; set; }
        public string Disgust { get; set; }
        public string Fear { get; set; }
        public string Joy { get; set; }

        private PhotoProcessed feed = null;
        public Affectiva()
        {
            string filename = Application.StartupPath + @"\ImageLogin.jpg";
            string classifierPath = Application.StartupPath + ConfigurationManager.AppSettings["classifierPath"];
            Bitmap bitmap = new Bitmap(filename);
            int numBytes = bitmap.Width * bitmap.Height * 3;
            //Affdex.FrameDetector detector = new FrameDetector(numBytes);
            PhotoDetector detector = new PhotoDetector(1, FaceDetectorMode.LARGE_FACES);
            feed = new PhotoProcessed(detector);
            detector.setClassifierPath(classifierPath);
            detector.setDetectAllEmotions(true);

            Frame frame = LoadFrameFromFile(filename);
            detector.start();
            detector.process(frame);
            detector.stop();
            
            setPropertiesList();

            bitmap.Dispose();
        }
        public void setPropertiesList()
        {
            Dictionary<string, float> properties = feed.emoctionPropertis;
            float result = 0;
            foreach (KeyValuePair<string, float> pair in properties)
            {
                string key = pair.Key;
                result = properties[key];
                key = key.ToLower();
                if (key.Equals(AffectivaProperties.Anger.ToString().ToLower()))
                {
                    this.Anger = result.ToString();
                }
                else if (key.Equals(AffectivaProperties.Contempt.ToString().ToLower()))
                {
                    this.Contempt = result.ToString();
                }
                else if (key.Equals(AffectivaProperties.Disgust.ToString().ToLower()))
                {
                    this.Disgust = result.ToString();
                }
                else if (key.Equals(AffectivaProperties.Engagement.ToString().ToLower()))
                {
                    this.Engagement = result.ToString();
                }
                else if (key.Equals(AffectivaProperties.Fear.ToString().ToLower()))
                {
                    this.Fear = result.ToString();
                }
                else if (key.Equals(AffectivaProperties.Joy.ToString().ToLower()))
                {
                    this.Joy = result.ToString();
                }
                else if (key.Equals(AffectivaProperties.Sadness.ToString().ToLower()))
                {
                    this.Sadness = result.ToString();
                }
                else
                     if (key.Equals(AffectivaProperties.Surprise.ToString().ToLower()))
                {
                    this.Surprise = result.ToString();
                }
                else if (key.Equals(AffectivaProperties.Valence.ToString().ToLower()))
                {
                    this.Valence = result.ToString();
                }
            }
        }

        static Affdex.Frame LoadFrameFromFile(string fileName)
        {
            Bitmap bitmap = new Bitmap(fileName);

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            int numBytes = bitmap.Width * bitmap.Height * 3;
            byte[] rgbValues = new byte[numBytes];

            int data_x = 0;
            int ptr_x = 0;
            int row_bytes = bitmap.Width * 3;

            for (int y = 0; y < bitmap.Height; y++)
            {
                Marshal.Copy(ptr + ptr_x, rgbValues, data_x, row_bytes);//(pixels, data_x, ptr + ptr_x, row_bytes);
                data_x += row_bytes;
                ptr_x += bmpData.Stride;
            }

            Frame f = new Frame(bitmap.Width, bitmap.Height, rgbValues, Affdex.Frame.COLOR_FORMAT.BGR, 0);

            bitmap.Dispose();

            return f;
        }
    }
}
