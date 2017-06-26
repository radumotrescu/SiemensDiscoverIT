
using System.IO;
using System.Collections.Generic;
using Accord.Controls;
using System;
using Accord.Math;

namespace AccordPCA
{
	class Program
	{

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
			foreach (var pointCloud in pointCloudlist)
			{
				totalRows += pointCloud.Count;
			}
			sw.WriteLine(totalRows + " " + totalColumns);
			foreach (var pointCloud in pointCloudlist)
			{
				var doubles = pointCloud.ReturnDoubleMatrix();
				//ScatterplotBox.Show("doubles", doubles);
				var count = pointCloud.Count;
				for (var i = 0; i < count; i++)
				{
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
			for (var i = 0; i < count; i++)
			{
				for (var j = 0; j < m; j++)
				{
					double x;
					double.TryParse(bits[k++], out x);
					data[i, j] = x;
				}
			}
		}


		static List<int> pick10(List<int> indexes)
		{
			List<int> toReturn = new List<int>();
			Random random = new Random();
			int nr = 0;
			while (nr < 10)
			{
				int x = random.Next(0, indexes.Count);
				while (toReturn.IndexOf(indexes[x]) < 0)
				{
					toReturn.Add(indexes[x]); nr++;
				}

			}
			return toReturn;
		}

		static List<int> removeIndexesFromList(List<int> indexes, List<int> toRemove)
		{
			foreach (int index in toRemove)
			{
				indexes.Remove(index);
			}
			return indexes;
		}

		static double[,] excludePointsByIndex(double[,] initialPointList, List<int> indexes)
		{
			int k = 0;
			double[,] data = new double[0, 2];

			for (int i = 0; i < initialPointList.Rows(); i++)
			{
				bool exists = false;
				for (int j = 0; j < indexes.Count; j++)
				{
					if (indexes[j] == i)
					{
						k++;
						exists = true;
						break;
					}
				}
				if (exists == false)
					data = data.InsertRow(initialPointList.GetRow(i));
			}
			return data;
		}

		static bool inCloud(double[,] cloud, double[] point)
		{
			for (int i = 0; i < cloud.Rows(); i++)
			{
				if (cloud[i, 0] == point[0] && cloud[i, 1] == point[1])
					return true;
			}
			return false;
		}

		static void ReadDataFromFile(out double[,] data, string filePath)
		{
			StreamReader sr = new StreamReader(filePath);
			string text = sr.ReadToEnd();
			sr.Close();

			string[] bits = text.Split(new char[] { ' ', '\n', '\r' });
			List<double[]> doubleBits = new List<double[]>();
			for (int i = 0; i < bits.Length; i++)
				if (bits[i].Equals(""))
					bits = bits.RemoveAt(i--);
			int m;
			int count;
			int.TryParse(bits[0], out count);
			int.TryParse(bits[1], out m);

			data = new double[count, m];

			int k = 2;
			for (var i = 0; i < count; i++)
			{
				for (var j = 0; j < m; j++)
				{
					double x;
					double.TryParse(bits[k++], out x);
					data[i, j] = x;
				}
			}
		}

		static double[,] combineData(List<double[,]> dataList, int count)
		{
			double[,] data = new double[0, 2];
			int k = 0;
			foreach (var dataSet in dataList)
			{
				for (int i = 0; i < dataSet.Rows(); i++)
				{
					data = data.InsertRow(dataSet.GetRow(i), k++);
				}
			}
			return data;
		}

		struct Range
		{
			public double x, y;
			public void newRange(double x, double y)
			{
				this.x = x;
				this.y = y;
			}
		}

		static void getMinMax(double[] data, out double min, out double max)
		{
			min = double.MaxValue;
			max = double.MinValue;
			for (int i = 0; i < data.Length; i++)
			{
				if (data[i] < min)
					min = data[i];
				else if (data[i] > max)
					max = data[i];
			}
		}

		static void Main(string[] args)
		{

			//double[,] data;
			//int pointNumber;

			int cloud1Points = 50;
			int cloud2Points = 50;

			//var pointCloud1 = new PointCloud(20, 2, 3, 3);
			//var pointCloud2 = new PointCloud(50, 4, 10, 10);
			//var pointCloud3 = new PointCloud(100, 10, 25, 25);
			//var cloudList = new List<PointCloud>();
			//cloudList.Add(pointCloud1);

			//cloudList.Add(pointCloud2);
			//cloudList.Add(pointCloud3);

			//WriteDataToFile(cloudList, "data.txt");

			double[,] data1;
			double[,] data2;


			ReadDataFromFile(out data1, "cloud1.txt");
			ReadDataFromFile(out data2, "cloud2.txt");


			var list = new List<double[,]>();
			list.Add(data1);
			list.Add(data2);
			var data = combineData(list, cloud1Points + cloud2Points);

			//ReadDataFromFile(out data, out pointNumber, "data.txt");


			List<int> indexes = new List<int>();
			for (int i = 0; i < 100; i++)
			{
				indexes.Add(i);
			}

			double totalPuncteDiferite = 0;

			for (int l = 0; l < 10; l++)
			{
				//Console.WriteLine(l);
				cloud1Points = 50;
				cloud2Points = 50;

				List<int> firstTen = pick10(indexes);
				indexes = removeIndexesFromList(indexes, firstTen);

				var allData = new double[data.Rows(), data.Columns()];
				Array.Copy(data, allData, allData.Length);

				allData = excludePointsByIndex(allData, firstTen);

				int excludedPointsNumber = 10;

				double[,] cloud1InitialExcluded = new double[0, 2];
				double[,] cloud2InitialExcluded = new double[0, 2];

				Range r1bun = new Range { x = 0, y = 0 }, r2bun = new Range { x = 0, y = 0 };

				int[] indexesInitial = new int[excludedPointsNumber];
				for (int i = 0; i < excludedPointsNumber; i++)
				{
					int j = firstTen[i];
					var point = data.GetRow(j);
					if (inCloud(data1, point))
					{
						cloud1InitialExcluded = cloud1InitialExcluded.InsertRow(point);
						indexesInitial[i] = 1;
						cloud1Points--;
					}
					else if (inCloud(data2, point))
					{
						cloud2InitialExcluded = cloud2InitialExcluded.InsertRow(point);
						indexesInitial[i] = 2;
						cloud2Points--;
					}

				}
				//Console.WriteLine(a.ToString("+0.0;-0.0"));
				//Console.WriteLine(indexesInitial.ToString("+0.0;-0.0"));

				double gamma = 1;
				int minPoints = int.MaxValue;
				double goodGamma = 1;

				double[,] finalData1;
				double[,] finalData2;

				var PCA = new ObjectPCA(allData);
				//PCA.Compute();

				//while (gamma <= 30)
				double separationPoint = 0;

				double[,] cloud1PlottedPoints = new double[0, 2];
				double[,] cloud2PlottedPoints = new double[0, 2];

				double goodSeparationPoint = 0;

				var g1 = new double[0, 0];
				var g2 = new double[0, 0];


				do
				{

					PCA.Gamma = gamma;
					PCA.ComputeKernel();
					//PCA.kernelAccord();
					var cloud1Range = new Range { x = 0, y = cloud1Points };
					var cloud2Range = new Range { x = cloud1Points, y = cloud1Points + cloud2Points };

					finalData1 = PCA.KernelData.Get((int)cloud1Range.x, (int)cloud1Range.y, 0, 2);
					finalData2 = PCA.KernelData.Get((int)cloud2Range.x, (int)cloud2Range.y, 0, 2);

					double min1, max1, min2, max2;

					getMinMax(finalData1.GetColumn(0), out min1, out max1);
					getMinMax(finalData2.GetColumn(0), out min2, out max2);



					Range r1 = new Range(), r2 = new Range();
					//
					{
						double mean1 = min2 - (max1 + min2) / 2;
						if (max1 < min2)
						{
							//Console.WriteLine("okg");
							separationPoint = min2 - mean1;
						}
						else
						{
							separationPoint = min2 + Math.Abs(mean1);
						}

						r1.newRange(min1 - mean1, max1 + mean1);
						r2.newRange(min2 - mean1, max2 + mean1);

						//double mean1 = (min1 + max1) / 2;
						//double mean2 = (min2 + max2) / 2;


						//Console.WriteLine(gamma+ "AM INTRAT ");
						//Console.Write("R1: {0} {1} R2: {2} {3} \n", r1.x, r1.y, r2.x, r2.y);
						//Console.WriteLine(r1.x + " " + r1.y + " " + r2.x + " " + r2.y + " " + r3.x + " " + r3.y + " ");

						//double x, y;
						//x = (double)numericUpDown1.Value;
						//y = (double)numericUpDown2.Value;
						//var p = PCA.plotPointPCA(x, y);

						double[,] plottedPoints = new double[0, 2];

						cloud1PlottedPoints = new double[0, 2];
						cloud2PlottedPoints = new double[0, 2];

						double[,] initialExcludedPoints = new double[0, 2];
						for (int i = 0; i < excludedPointsNumber; i++)
						{
							initialExcludedPoints = initialExcludedPoints.InsertRow(data.GetRow(firstTen[i]));
							plottedPoints = plottedPoints.InsertRow(new double[] { PCA.plotPointKernelPCA(data.GetRow(firstTen[i])[0], data.GetRow(firstTen[i])[1]), 0 });
							if (firstTen[i] < 50)
							{
								cloud1PlottedPoints = cloud1PlottedPoints.InsertRow(new double[] { plottedPoints[i, 0], plottedPoints[i, 1] });
							}
							else
							{
								cloud2PlottedPoints = cloud2PlottedPoints.InsertRow(new double[] { plottedPoints[i, 0], plottedPoints[i, 1] });
							}
						}
						//Console.WriteLine("Cloud1");
						//Console.WriteLine(cloud1PlottedPoints.ToString("+0.0000000;-0.000000"));
						//Console.WriteLine("Cloud2");
						//Console.WriteLine(cloud2PlottedPoints.ToString("+0.0000000;-0.000000"));

						int[] indexesFinal = new int[excludedPointsNumber];
						for (int i = 0; i < excludedPointsNumber; i++)
						{
							if (plottedPoints[i, 0] < separationPoint)
							{

								indexesFinal[i] = 1;
							}
							else if (plottedPoints[i, 0] > separationPoint)
							{

								indexesFinal[i] = 2;
							}

							//Console.WriteLine("{0}, {1}, {2}, {3}", bigAllData.GetRow(indexes[i])[0], plottedPoints[i, 0], indexesInitial[i], indexesFinal[i]);
						}


						//Console.WriteLine();
						//Console.WriteLine(plottedPoints.ToString("+0.0;-0.0"));
						//Console.WriteLine(indexesFinal.ToString("+0.0;-0.0"));


						int differentPoints = 0;
						for (int i = 0; i < excludedPointsNumber; i++)
						{
							if (indexesInitial[i] != indexesFinal[i])
								differentPoints++;
						}
						//Console.WriteLine();
						//Console.WriteLine(differentPoints);
						//Console.WriteLine(differentPoints + " "+gamma);
						if (differentPoints < minPoints)
						{
							minPoints = differentPoints;
							goodSeparationPoint = separationPoint;
							goodGamma = gamma;
							r1bun = r1;
							r2bun = r2;
							g1 = cloud1PlottedPoints;
							g2 = cloud2PlottedPoints;
							if (max1 < min2)
							{
								//Console.WriteLine("ok da");

							}
							else
							{
								//Console.WriteLine("ok nu");
							}
						}

					}

					gamma += 0.1;
				} while (gamma < 20);

				totalPuncteDiferite += minPoints;
				Console.WriteLine("Numarul de puncte diferite: "+minPoints);
				Console.WriteLine("Gamma pentru acest set: "+ goodGamma);
				Console.Write("Indexi puncte excluse: ");
				foreach (int index in firstTen)
				{
					Console.Write(index + " ");
				}
				Console.WriteLine();
				Console.WriteLine();


			}
			Console.WriteLine("Procentaj: "+totalPuncteDiferite+"%");






			//var PCA1 = new ObjectPCA(data);
			//PCA1.Compute();
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