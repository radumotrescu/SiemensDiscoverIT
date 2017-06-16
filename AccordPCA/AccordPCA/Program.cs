﻿
using System.IO;
using System.Collections.Generic;
using Accord.Controls;

namespace AccordPCA {
	class Program {

		/// <summary>
		/// Method the write the given point cloud to file
		/// </summary>
		/// <param name="pointCloudlist">The list of point cloud to write</param>
		/// <param name="filePath">The path of the file to write to</param>
		static void WriteDataToFile(List<PointCloud> pointCloudlist, string filePath)
		{
			StreamWriter sw = new StreamWriter(filePath);
			int totalRows = 0;
			const int totalColumns = 2;
			foreach (var pointCloud in pointCloudlist) {
				totalRows += pointCloud.Count;
			}
			sw.WriteLine(totalRows + " " + totalColumns);
			foreach (var pointCloud in pointCloudlist) {
				var doubles = pointCloud.ReturnDoubleMatrix();
                //ScatterplotBox.Show("doubles", doubles);
				var count = pointCloud.Count;
				for (var i = 0; i < count; i++) {
					sw.WriteLine(doubles[i, 0] + " " + doubles[i, 1]);

				}

			}
			sw.Close();

		}


		/// <summary>
		/// Method to read the data from file
		/// </summary>
		/// <param name="data">Matrix to write the data to</param>
		/// <param name="count">The number of rows in the data file</param>
		/// <param name="filePath">The path of the file to write from</param>
		static void ReadDataFromFile(out double[,] data, out int count, string filePath)
		{
			StreamReader sr = new StreamReader(filePath);
			string text = sr.ReadToEnd();
			sr.Close();

			string[] bits = text.Split(new char[] { ' ', '\n' });
			List<double[]> doubleBits = new List<double[]>();

			int m;
			int.TryParse(bits[0], out count);
			int.TryParse(bits[1], out m);

			data = new double[count, m];

			int k = 2;
			for (var i = 0; i < count; i++) {
				for (var j = 0; j < m; j++) {
					double x;
					double.TryParse(bits[k++], out x);
					data[i, j] = x;
				}
			}
		}


		static void Main(string[] args)
		{

			double[,] data;
			int pointNumber;

			var pointCloud1 = new PointCloud(20, 2, 3, 3);
			var pointCloud2 = new PointCloud(50, 4, 10, 10);
			var pointCloud3 = new PointCloud(100, 10, 25, 25);
			var cloudList = new List<PointCloud>();
			cloudList.Add(pointCloud1);

			cloudList.Add(pointCloud2);
			cloudList.Add(pointCloud3);

			//WriteDataToFile(cloudList, "data.txt");
			ReadDataFromFile(out data, out pointNumber, "kerneldata.txt");

			var PCA1 = new ObjectPCA(data);
			PCA1.Compute();
			//PCA1.ShowScatterplots();

            //PCA1.ComputeKernel();

			//PCA1.pointRecognition(2, 5);

			//double[,] curve = new double[60, 2];
			//double x = 1;
			//double finish = 0;
			//double y = 9;
			//int i = 0;
			//while (y > finish) {
			//	y = -System.Math.Pow(x, 2);
			//	curve[i, 0] = x;
			//	curve[i, 1] = y;
			//	i++;
			//}


			//ScatterplotBox.Show(curve);


		}
	}
}
