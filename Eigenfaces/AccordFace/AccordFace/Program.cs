using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Accord.Imaging.Converters;
using Accord.Math;
using AccordPCA;

namespace AccordFace
{
	class Program
	{
		static void Main(string[] args)
		{
			//Bitmap image1 = new Bitmap(@"D:\eigenfaces edit resize\image1.bmp");
			//Bitmap image1 = new Bitmap(@"D:\eigenfaces edit resize test\image1.bmp");
			//Bitmap image1 = new Bitmap(@"D:\eigenfaces edit black\image1.bmp");


			List<Bitmap> bitmapList = new List<Bitmap>();
			int imageWidth = 192;
			int imageHeight = 168;
			int imageNumber = 51;

			for (int i = 1; i < imageNumber; i++)
			{
				string path = string.Format(@"yaleB01\subject1 ({0}).bmp", i);
				
				Bitmap newBitmap = new Bitmap(path);
				bitmapList.Add(newBitmap);
			}

			ImageToArray converter = new ImageToArray(-1, +1);

			List<double[]> outputList = new List<double[]>();
			foreach (Bitmap bitmap in bitmapList)
			{
				double[] newOutput;
				converter.Convert(bitmap, out newOutput);
				outputList.Add(newOutput);
			}



			//ArrayToImage ati1 = new ArrayToImage(image1.Height, image1.Width);
			//Bitmap test = new Bitmap(image1.Height, image1.Width);
			//ati1.Convert(output1, out test);
			//test.Save(@"d:\eigenface1.bmp");




			int size = imageHeight * imageWidth;
			double[,] data = new double[size, imageNumber];


			for (int i = 0; i < outputList.Count; i++)
			{
				data.SetColumn(i, outputList[i]);
			}


			ObjectPCA obj = new ObjectPCA(data);

			obj.Compute();
			double[,] finalData = obj.FinalData;


			ArrayToImage ati = new ArrayToImage(imageHeight, imageWidth);
			ati.Min = finalData.Min();
			ati.Max = finalData.Max();
			Bitmap eigenface = new Bitmap(imageHeight, imageWidth);

			for (int i = 0; i < finalData.Columns(); i++)
			{
				string path = string.Format(@"D:\eigenfaces result\image{0}.bmp", i + 1);
				ati.Convert(finalData.GetColumn(i), out eigenface);
				eigenface.Save(path);
			}



			eigenface.Dispose();
			foreach (Bitmap bitmap in bitmapList)
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
