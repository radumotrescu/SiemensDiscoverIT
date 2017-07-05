using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Accord.Imaging.Converters;
using Accord.Math;
using AccordPCA;

namespace AccordFace {
    class Program {
        static void Main(string[] args)
        {
            //Bitmap image1 = new Bitmap(@"D:\eigenfaces edit resize\image1.bmp");
            //Bitmap image1 = new Bitmap(@"D:\eigenfaces edit resize test\image1.bmp");
            //Bitmap image1 = new Bitmap(@"D:\eigenfaces edit black\image1.bmp");


            List<Bitmap> trainingFaces = new List<Bitmap>();
            List<Bitmap> testingFaces = new List<Bitmap>();
            int imageWidth = 192;
            int imageHeight = 168;
            int trainingImageNumber = 10;
            int testingImageNumber = 3;

            for (int i = 1; i <= trainingImageNumber + testingImageNumber; i++)
            {

                string path = string.Format(@"yaleB01\subject1 ({0}).bmp", i);

                Bitmap newBitmap = new Bitmap(path);
                if (i <= trainingImageNumber)
                    trainingFaces.Add(newBitmap);
                else
                    testingFaces.Add(newBitmap);

            }

			for (int i = 1; i <= trainingImageNumber + testingImageNumber; i++)
			{

				string path = string.Format(@"yaleB03\subject3 ({0}).bmp", i);

				Bitmap newBitmap = new Bitmap(path);
				if (i <= trainingImageNumber)
					trainingFaces.Add(newBitmap);
				else
					testingFaces.Add(newBitmap);

			}


			//string path1 = string.Format(@"yaleB01\tree.bmp");
			//Bitmap newBitmap1 = new Bitmap(path1);
			//testingFaces.Add(newBitmap1);

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
            obj.Gamma = 22560;
            obj.ComputeKernel();
			//obj.Compute();
			//double[,] finalData = obj.KernelData;


			//var image = testingOutputList[14].Transpose().Dot(finalData.Transpose());

			// var image1 = data.Transpose().Dot(finalData.GetColumn(0));
			//double[,] finalData = obj.FinalData;


			//double[,] finalData =obj.plotPointPCA(testingOutputList[0]);
			//foreach (var face in trainingOutputList)
			//    obj.faceRecognition(face);


			int[] indexesInitial = new int[testingOutputList.Count];
			for (int i = 0; i < testingOutputList.Count; i++)
				if (i < testingOutputList.Count / 2)
					indexesInitial[i] = 1;
				else
					indexesInitial[i] = 2;

			int x = 0;
			foreach(var face in testingOutputList)
			{
				System.Console.WriteLine(x++);
				System.Console.WriteLine(obj.plotPointKernelPCA(face));
			}
				


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
