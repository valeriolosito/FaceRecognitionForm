using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DlibDotNet;
using Image = System.Drawing.Image;

namespace FaceRecognitionForm
{
    public class UtilityRecognition
    {
        //return 0: confronto KO
        //return 1: confronto OK
        //return 2: nella foto scattata non sono presenti facce

        public static int Compare(Image imageDb, Image imageCamera)
        {
            // valore se resta 1 è andato tutto bene
            int result = 1;
            try
            {
               //path necessari per l'operazione
                string pathImageUnited = Application.StartupPath + ConfigurationManager.AppSettings["pathImageUnited"];
                string pathShapePredictor = Application.StartupPath + ConfigurationManager.AppSettings["pathShapePredictor"];
                string pathDlibFaceRecognition = Application.StartupPath + ConfigurationManager.AppSettings["pathDlibFaceRecognition"];
                
                //unisce le due foto
                int width = imageDb.Width + imageCamera.Width;
                int height = Math.Max(imageDb.Height, imageCamera.Height);
                Bitmap img3 = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(img3);
                g.Clear(Color.Black);
                g.DrawImage(imageDb, new System.Drawing.Point(0, 0));
                g.DrawImage(imageCamera, new System.Drawing.Point(imageDb.Width, 0));
                //salva la nuova immagine contente le due immagini precedenti 
                img3.Save(pathImageUnited, System.Drawing.Imaging.ImageFormat.Jpeg);
             
                int numPersone = 1;
               
                // The first thing we are going to do is load all our models.  First, since we need to
                // find faces in the image we will need a face detector:
                using (var detector = Dlib.GetFrontalFaceDetector())
                // We will also use a face landmarking model to align faces to a standard pose:  (see face_landmark_detection_ex.cpp for an introduction)
                using (var sp = ShapePredictor.Deserialize(pathShapePredictor))
                // And finally we load the DNN responsible for face recognition.
                using (var net = DlibDotNet.Dnn.LossMetric.Deserialize(pathDlibFaceRecognition))

                using (var img = Dlib.LoadImageAsMatrix<RgbPixel>(pathImageUnited))
                {
                    // Run the face detector on the image of our action heroes, and for each face extract a
                    // copy that has been normalized to 150x150 pixels in size and appropriately rotated
                    // and centered.
                    var faces = new List<Matrix<RgbPixel>>();
                    foreach (var face in detector.Operator(img))
                    {
                        var shape = sp.Detect(img, face);
                        var faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                        var faceChip = Dlib.ExtractImageChip<RgbPixel>(img, faceChipDetail);
                       
                        faces.Add(faceChip);
                    }

                    if (faces.Count < 2)
                    {
                        return result = 2;
                    }

                    // This call asks the DNN to convert each face image in faces into a 128D vector.
                    // In this 128D vector space, images from the same person will be close to each other
                    // but vectors from different people will be far apart.  So we can use these vectors to
                    // identify if a pair of images are from the same person or from different people.  
                    var faceDescriptors = net.Operator(faces);

                    // In particular, one simple thing we can do is face clustering.  This next bit of code
                    // creates a graph of connected faces and then uses the Chinese whispers graph clustering
                    // algorithm to identify how many people there are and which faces belong to whom.
                    var edges = new List<SamplePair>();
                    for (uint i = 0; i < faceDescriptors.Count; ++i)
                    {
                        for (var j = i; j < faceDescriptors.Count; ++j)
                        {
                            // Faces are connected in the graph if they are close enough.  Here we check if
                            // the distance between two face descriptors is less than 0.6, which is the
                            // decision threshold the network was trained to use.  Although you can
                            // certainly use any other threshold you find useful.
                            var diff = faceDescriptors[i] - faceDescriptors[j];
                            if (Dlib.Length(diff) < 0.6)
                                edges.Add(new SamplePair(i, j));
                        }
                    }

                    Dlib.ChineseWhispers(edges, 100, out var numClusters, out var labels);
                    // This will correctly indicate that there are 4 people in the image.
                    Console.WriteLine($"number of people found in the image: {numClusters}");
                    numPersone = Int32.Parse(numClusters.ToString());
                }

                if (numPersone > 1)
                    return result = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
            return result;
        }

        public static int getNumberFaces(string imagePath)
        {
            int result = 0;
            string pathShapePredictor = Application.StartupPath + ConfigurationManager.AppSettings["pathShapePredictor"];
            using (var detector = Dlib.GetFrontalFaceDetector())
            using (var sp = ShapePredictor.Deserialize(pathShapePredictor))
            using (var img = Dlib.LoadImageAsMatrix<RgbPixel>(imagePath))
            {
                var faces = new List<Matrix<RgbPixel>>();
                foreach (var face in detector.Operator(img))
                {
                    var shape = sp.Detect(img, face);
                    var faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                    var faceChip = Dlib.ExtractImageChip<RgbPixel>(img, faceChipDetail);

                    faces.Add(faceChip);
                }
                result = faces.Count;
            }
            return result;
        }
    }
}



