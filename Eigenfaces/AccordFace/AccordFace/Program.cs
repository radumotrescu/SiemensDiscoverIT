using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Accord.Imaging.Converters;
using Accord.Math;
using AccordPCA;
using System;

namespace AccordFace {
    class Program {
        static void Main(string[] args)
        {
            //Bitmap image1 = new Bitmap(@"D:\eigenfaces edit resize\image1.bmp");
            //Bitmap image1 = new Bitmap(@"D:\eigenfaces edit resize test\image1.bmp");
            //Bitmap image1 = new Bitmap(@"D:\eigenfaces edit black\image1.bmp");


            List<Bitmap> trainingFaces = new List<Bitmap>();
            List<Bitmap> testingFaces = new List<Bitmap>();
            List<Bitmap> settingFaces = new List<Bitmap>();
            int imageWidth = 192;
            int imageHeight = 168;
            int trainingImageNumber = 15;
            int settingImageNumber = 5;
            int testingImageNumber = 45;

            for (int i = 1; i <= trainingImageNumber + testingImageNumber + settingImageNumber; i++)
            {

                string path = string.Format(@"yaleB01\subject1 ({0}).bmp", i);

                Bitmap newBitmap = new Bitmap(path);
                if (i <= trainingImageNumber)
                    trainingFaces.Add(newBitmap);
                else if (i < trainingImageNumber + settingImageNumber)
                    settingFaces.Add(newBitmap);
                else
                    testingFaces.Add(newBitmap);

            }

            //for (int i = 1; i <= trainingImageNumber + testingImageNumber; i++)
            //{

            //    string path = string.Format(@"yaleB03\subject3 ({0}).bmp", i);

            //    Bitmap newBitmap = new Bitmap(path);
            //    if (i <= trainingImageNumber)
            //        trainingFaces.Add(newBitmap);
            //    else
            //        testingFaces.Add(newBitmap);

            //}


            string path1 = string.Format(@"yaleB01\tree.bmp");
            Bitmap newBitmap1 = new Bitmap(path1);
            testingFaces.Add(newBitmap1);

            path1 = string.Format(@"yaleB01\puppy.bmp");
            newBitmap1 = new Bitmap(path1);
            testingFaces.Add(newBitmap1);

            //path1 = string.Format(@"yaleB01\subject3 (3).bmp");
            //newBitmap1 = new Bitmap(path1);
            //testingFaces.Add(newBitmap1);


            ImageToArray converter = new ImageToArray(-1, +1);

            List<double[]> trainingOutputList = new List<double[]>();
            foreach (Bitmap bitmap in trainingFaces)
            {
                double[] newOutput;
                converter.Convert(bitmap, out newOutput);
                trainingOutputList.Add(newOutput);
            }

            List<double[]> testingOutputList = new List<double[]>();
            foreach (Bitmap bitmap in testingFaces)
            {
                double[] newOutput;
                converter.Convert(bitmap, out newOutput);
                testingOutputList.Add(newOutput);
            }

            List<double[]> settingOutputList = new List<double[]>();
            foreach (Bitmap bitmap in settingFaces)
            {
                double[] newOutput;
                converter.Convert(bitmap, out newOutput);
                settingOutputList.Add(newOutput);
            }



            //ArrayToImage ati1 = new ArrayToImage(image1.Height, image1.Width);
            //Bitmap test = new Bitmap(image1.Height, image1.Width);
            //ati1.Convert(output1, out test);
            //test.Save(@"d:\eigenface1.bmp");




            int size = imageHeight * imageWidth;
            double[,] data = new double[trainingFaces.Count, size];


            for (int i = 0; i < trainingOutputList.Count; i++)
            {
                data.SetRow(i, trainingOutputList[i]);
            }


            ObjectPCA obj = new ObjectPCA(data);
            obj.take2();
<<<<<<< HEAD
            var finalData = obj.W;

            //var x = obj.projectImage(testingOutputList[10].Transpose());
            //Console.WriteLine(x);
            double x;
            double max = 0;
            foreach (var row in settingOutputList)
            {
                x = obj.projectImage(row.Transpose());
                if (x > max)
                    max = x;
            }
            Console.WriteLine("max " + max);
            int bad = 0;
            int v = 0;
            foreach (var row in testingOutputList)
            {
                x = obj.projectImage(row.Transpose());
                //Console.WriteLine(x+" "+v++);
                if (x > max)
                {
                    Console.WriteLine(testingOutputList.IndexOf(row) + settingImageNumber + trainingImageNumber + " " + x);
                    bad++;
                }


            }
            Console.WriteLine("eroare " + bad / (testingImageNumber + 0.0) * 100);


            //finalData = obj.W;

            //ArrayToImage ati = new ArrayToImage(imageHeight, imageWidth);
            //ati.Min = finalData.Min();
            //ati.Max = finalData.Max();
            //Bitmap eigenface = new Bitmap(imageHeight, imageWidth);

            //for (int i = 0; i < finalData.Columns(); i++)
            //{
            //    string path = string.Format(@"D:\eigenfaces result\image{0}.bmp", i);
            //    ati.Convert(finalData.GetColumn(i), out eigenface);
            //    eigenface.Save(path);
            //}

=======
            
>>>>>>> a3e1fce4e086278d278a8aff176a6a0e08b0108a
            //obj.Compute();
            //double[,] finalData = obj.KernelData;


            //var image = testingOutputList[14].Transpose().Dot(finalData.Transpose());

            // var image1 = data.Transpose().Dot(finalData.GetColumn(0));
            //double[,] finalData = obj.FinalData;


            //double[,] finalData =obj.plotPointPCA(testingOutputList[0]);
            //foreach (var face in trainingOutputList)
            //    obj.faceRecognition(face);


            //-------------------------------------------------

            //obj.Gamma = Math.Pow(10, -3);
            //obj.ComputeKernel();

            //int[] indexesInitial = new int[testingOutputList.Count];
            //for (int i = 0; i < testingOutputList.Count; i++)
            //    if (i < testingOutputList.Count / 2)
            //        indexesInitial[i] = 1;
            //    else
            //        indexesInitial[i] = 2;

            //double min1 = double.MaxValue, max1 = double.MinValue, min2 = min1, max2 = max1;
            //for (int i = 0; i < trainingOutputList.Count; i++)
            //{
            //    if (i < trainingOutputList.Count / 2)
            //    {
            //        if (obj.KernelVectors[i] < min1)
            //            min1 = obj.KernelVectors[i];
            //        if (obj.KernelVectors[i] > max1)
            //            max1 = obj.KernelVectors[i];
            //    }
            //    else
            //    {
            //        if (obj.KernelVectors[i] < min2)
            //            min2 = obj.KernelVectors[i];
            //        if (obj.KernelVectors[i] > max2)
            //            max2 = obj.KernelVectors[i];
            //    }
            //}

            //int x = 0;

            //double mean1 = min2 - (max1 + min2) / 2;
            //double separationPoint = min2 + Math.Abs(mean1);

            //int[] indexesFinal = new int[testingOutputList.Count];

            //foreach (var face in testingOutputList)
            //{
            //    System.Console.WriteLine(x++);
            //    double aux = obj.plotPointKernelPCA(face);
            //    System.Console.WriteLine(aux);
            //    if (aux < separationPoint)
            //        indexesFinal[testingOutputList.IndexOf(face)] = 1;
            //    else
            //        indexesFinal[testingOutputList.IndexOf(face)] = 2;

            //}

            //Console.WriteLine(obj.KernelVectors.ToString("+0.0000;-0.0000"));
            //Console.WriteLine();
            //Console.WriteLine(obj.KernelValues);
            //Console.WriteLine();
            //Console.WriteLine(indexesInitial.ToString("+0.0000;-0.0000"));
            //Console.WriteLine(indexesFinal.ToString("+0.0000;-0.0000"));
            //Console.WriteLine(separationPoint);

            //------------------------------------------------------------


            //int minimi = 0;
            //foreach (var training in testingOutputList)
            //{

            //    if (obj.faceRecognition(training) < 105)
            //        minimi++;
            //}
            //System.Console.WriteLine(minimi);


            //ArrayToImage ati = new ArrayToImage(imageHeight, imageWidth);
            //ati.Min = finalData.Min();
            //ati.Max = finalData.Max();
            //Bitmap eigenface = new Bitmap(imageHeight, imageWidth);

            //for (int i = 0; i < finalData.Columns(); i++)
            //{
            //    string path = string.Format(@"D:\eigenfaces result\image{0}.bmp", i);
            //    ati.Convert(finalData.GetColumn(i), out eigenface);
            //    eigenface.Save(path);
            //}


            //finalData = image;
            //for (int i = 0; i < finalData.Columns(); i++)
            //{
            //    string path = string.Format(@"D:\eigenfaces result\image{0}.bmp", i);
            //    ati.Convert(finalData.GetColumn(i), out eigenface);
            //    eigenface.Save(path);
            //}


            //string path2 = string.Format(@"D:\eigenfaces result\image{0}.bmp", "asd");
            //ati.Convert(image1, out eigenface);
            //eigenface.Save(path2);
            //eigenface.Dispose();






            foreach (Bitmap bitmap in trainingFaces)
                bitmap.Dispose();
            //Console.WriteLine(output.ToString("+0.00;-0.00"));
            //ArrayToImage ati = new ArrayToImage(image1.Height, image1.Width);
            //Bitmap image2 = new Bitmap(image1.Height, image1.Width);
            //ati.Convert(output, out image2);
            //image2.Save("d:\\image.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            //StreamWriter sw = new StreamWriter("data.txt");
            //foreach (double x in output)
            //    sw.Write(x + " ");


        }
    }
}
