using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using System.IO;
using AccordPCA;
using Accord.Math;

namespace PlotPCA {
	public partial class Form1 : Form {


		static double[,] WriteDataToFile(PointCloud pointCloud, string filePath, CsvHelper.CsvWriter csvWriter, int cloudIndex, bool up)
		{


			StreamWriter sw = new StreamWriter(filePath);
			int totalRows = pointCloud.Count;
			int totalColumns = 2;
			sw.WriteLine(totalRows + " " + totalColumns);
			var doubles = pointCloud.ReturnCircleDoubleMatrix();
			for (int i = 0; i < totalRows; i++) {

				sw.WriteLine(doubles[i, 0] + " " + doubles[i, 1]);
				csvWriter.WriteField(doubles[i, 0] + ";" + doubles[i, 1] + ";" + cloudIndex);
				csvWriter.NextRecord();

			}
			//csvWriter.WriteRecord(pointCloud);
			sw.Close();


			return doubles;
		}

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

		static void ReadDataFromFile(out double[,] data, string filePath)
		{
			StreamReader sr = new StreamReader(filePath);
			string text = sr.ReadToEnd();
			sr.Close();

			string[] bits = text.Split(new char[] { ' ', '\n' });
			List<double[]> doubleBits = new List<double[]>();

			int m;
			int count;
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


		static void pointPairListInitialisation(PointPairList cloudPoints, double[,] point)
		{
			for (int i = 0; i < point.Rows(); i++)
				cloudPoints.Add(point.GetRow(i)[0], point.GetRow(i)[1]);
		}

		GraphPane myPaneOriginalData;
		GraphPane myPaneFinalData;
		public Form1()
		{
			InitializeComponent();

			myPaneOriginalData = zedGraphControl1.GraphPane;
			myPaneOriginalData.Title.Text = "PCA Initial Data";
			myPaneOriginalData.XAxis.Title.Text = "X Coordinates";
			myPaneOriginalData.YAxis.Title.Text = "Y Coordinates";
			zedGraphControl1.IsShowPointValues = true;

			myPaneFinalData = zedGraphControl2.GraphPane;
			myPaneFinalData.Title.Text = "PCA Final Data";
			myPaneFinalData.XAxis.Title.Text = "X Coordinates";
			myPaneFinalData.YAxis.Title.Text = "Y Coordinates";
			zedGraphControl2.IsShowPointValues = true;

		}

		static double[,] combineData(List<double[,]> dataList, int count)
		{
			double[,] data = new double[count, 2];
			int k = 0;
			foreach (var dataSet in dataList) {
				for (int i = 0; i < dataSet.Rows(); i++) {
					data = data.InsertRow(dataSet.GetRow(i), k++);
				}
			}
			return data;
		}

		private void zedGraphControl1_Load(object sender, EventArgs e)
		{

		}

		private void getMinMax(double[] data, out double min, out double max)
		{
			min = double.MaxValue;
			max = double.MinValue;
			for (int i = 0; i < data.Count(); i++) {
				if (data[i] < min)
					min = data[i];
				else if (data[i] > max)
					max = data[i];
			}
		}

		struct Range {
			public double x, y;
			public void newRange(double x, double y)
			{
				this.x = x;
				this.y = y;
			}
		}

		private bool checkPoint(Range range, double point)
		{
			if (range.x < point && range.y > point)
				return true;
			return false;
		}

		private void button1_Click(object sender, EventArgs e)
		{

			int cloud1points = 500;
			int cloud2points = 500;
			int cloud3points = 100;

			myPaneOriginalData.CurveList.Clear();
			myPaneFinalData.CurveList.Clear();

			var initialPointCloud1 = new PointCloud(cloud1points, 2, 15.7, 14.7);
			var initialPointCloud2 = new PointCloud(cloud2points, 4, 15, 15);
			//var initialPointCloud3 = new PointCloud(cloud3points, 10, 30, 30);

			var csv = new StreamWriter("cloud.csv");
			var csvWriter = new CsvHelper.CsvWriter(csv);
			csvWriter.WriteField("X;Y;cloud index");
			csvWriter.NextRecord();

			var data1 = WriteDataToFile(initialPointCloud1, "cloud1.txt", csvWriter, 1,true);
			var data2 = WriteDataToFile(initialPointCloud2, "cloud2.txt", csvWriter, 2,false);
			//var data3 = WriteDataToFile(initialPointCloud3, "cloud3.txt", csvWriter, 3);

			csv.Flush();
			csv.Close();



			//double[,] data1;
			//double[,] data2;
			//double[,] data3;

			//ReadDataFromFile(out data1, "cloud1.txt");
			//ReadDataFromFile(out data2, "cloud2.txt");
			//ReadDataFromFile(out data3, "cloud3.txt");
			var list = new List<double[,]>();
			list.Add(data1);
			list.Add(data2);
			//list.Add(data3);
			var allData = combineData(list, 140);


			var obj4 = new ObjectPCA(allData);
			obj4.Compute();


			var finalData1 = obj4.FinalData.Get(0, cloud1points, 0, 2);
			var finalData2 = obj4.FinalData.Get(cloud1points, cloud1points + cloud2points, 0, 2);
			var finalData3 = obj4.FinalData.Get(cloud1points + cloud2points, cloud1points + cloud2points + cloud3points, 0, 2);


			double min1, max1, min2, max2, min3, max3;

			getMinMax(finalData1.GetColumn(0), out min1, out max1);
			getMinMax(finalData2.GetColumn(0), out min2, out max2);
			getMinMax(finalData3.GetColumn(0), out min3, out max3);

			Range r1 = new Range(), r2 = new Range(), r3 = new Range();
			double mean1 = min2 - (max1 + min2) / 2;
			double mean2 = min3 - (max2 + min3) / 2;
			r1.newRange(min1 - mean1, max1 + mean1);
			r2.newRange(min2 - mean1, max2 + mean2);
			r3.newRange(min3 - mean2, max3 + mean2);



			double x, y;
			x = 25;
			y = 25;
			var point = obj4.plotPointPCA(x, y);

			var boo = checkPoint(r2, point[0, 0]);


			var initialCloud1 = new PointPairList();
			var initialCloud2 = new PointPairList();
			var initialCloud3 = new PointPairList();

			pointPairListInitialisation(initialCloud1, data1);
			pointPairListInitialisation(initialCloud2, data2);
			//pointPairListInitialisation(initialCloud3, data3);

			LineItem initialCloud1Points = myPaneOriginalData.AddCurve("Cloud1", initialCloud1, Color.Red, SymbolType.Circle);
			initialCloud1Points.Line.IsVisible = false;
			LineItem initialCloud2Points = myPaneOriginalData.AddCurve("Cloud2", initialCloud2, Color.Blue, SymbolType.Diamond);
			initialCloud2Points.Line.IsVisible = false;
			LineItem initialCloud3Points = myPaneOriginalData.AddCurve("Cloud3", initialCloud3, Color.Green, SymbolType.Star);
			initialCloud3Points.Line.IsVisible = false;


			var finalCloud1 = new PointPairList();
			var finalCloud2 = new PointPairList();
			var finalCloud3 = new PointPairList();


			pointPairListInitialisation(finalCloud1, finalData1);
			pointPairListInitialisation(finalCloud2, finalData2);
			//pointPairListInitialisation(finalCloud3, finalData3);

			//finalCloud1.Add(new PointPair(point[0, 0], point[1, 0]));

			LineItem finalCloud1Points = myPaneFinalData.AddCurve("Cloud1", finalCloud1, Color.Red, SymbolType.Circle);
			finalCloud1Points.Line.IsVisible = false;
			LineItem finalCloud2Points = myPaneFinalData.AddCurve("Cloud2", finalCloud2, Color.Blue, SymbolType.Diamond);
			finalCloud2Points.Line.IsVisible = false;
			LineItem finalCloud3Points = myPaneFinalData.AddCurve("Cloud3", finalCloud3, Color.Green, SymbolType.Star);
			finalCloud3Points.Line.IsVisible = false;

			myPaneOriginalData.AxisChange();
			zedGraphControl1.Invalidate();

			myPaneFinalData.AxisChange();
			zedGraphControl2.Invalidate();


		}
	}
}
