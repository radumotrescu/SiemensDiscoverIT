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

        GraphPane myPaneOriginalData;
        GraphPane myPaneFinalData;
        GraphPane myPaneOriginalLine;
        GraphPane myPaneFinalLine;

        ObjectPCA PCA { get; set; }

        int cloud1Points { get; set; }
        int cloud2Points { get; set; }
        int cloud3Points { get; set; }

        LineItem InitialCloud1Points { get; set; }
        LineItem InitialCloud2Points { get; set; }
        LineItem InitialCloud3Points { get; set; }

        LineItem FinalCloud1Points { get; set; }
        LineItem FinalCloud2Points { get; set; }
        LineItem FinalCloud3Points { get; set; }

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

            myPaneOriginalLine = zedGraphControl3.GraphPane;
            myPaneOriginalLine.Title.Text = "PCA Original First Component Span";
            myPaneOriginalLine.XAxis.Title.Text = "X Coordinates";
            myPaneOriginalLine.YAxis.Title.Text = "Y Coordinates";
            zedGraphControl3.IsShowPointValues = true;

            myPaneFinalLine = zedGraphControl4.GraphPane;
            myPaneFinalLine.Title.Text = "PCA Final First Component Span";
            myPaneFinalLine.XAxis.Title.Text = "X Coordinates";
            myPaneFinalLine.YAxis.Title.Text = "Y Coordinates";
            zedGraphControl4.IsShowPointValues = true;

        }

        #region utilfunctions

        static double[,] WriteDataToFile(PointCloud pointCloud, string filePath, CsvHelper.CsvWriter csvWriter, int cloudIndex, string type, bool up = true)
        {

            StreamWriter sw = new StreamWriter(filePath);
            int totalRows = pointCloud.Count;
            int totalColumns = 2;
            sw.WriteLine(totalRows + " " + totalColumns);

            double[,] doubles = null;

            if (type.Equals("circle"))
                doubles = pointCloud.ReturnCircleDoubleMatrix();
            else if (type.Equals("cloud"))
                doubles = pointCloud.ReturnCloudDoubleMatrix();
            else if (type.Equals("moon"))
                //doubles = pointCloud.ReturnMoonDoubleMatrix(up);
                doubles = pointCloud.ReturnEllipseMoonDoubleMatrix(up);

            for (int i = 0; i < totalRows; i++)
            {

                sw.WriteLine(doubles[i, 0] + " " + doubles[i, 1]);
                csvWriter.WriteField(doubles[i, 0] + ";" + doubles[i, 1] + ";" + cloudIndex);
                csvWriter.NextRecord();

            }
            //csvWriter.WriteRecord(pointCloud);
            sw.Close();


            return doubles;
        }


        static void WriteDataToFile(double[,] data, string filePath, int cloudIndex)
        {

            StreamWriter sw = new StreamWriter(filePath);
            int totalRows = data.Rows();
            int totalColumns = 2;
            sw.WriteLine(totalRows + " " + totalColumns);


            for (int i = 0; i < totalRows; i++)
            {

                sw.WriteLine(data[i, 0] + " " + data[i, 1]);


            }
            sw.Close();

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

        static void pointPairListInitialisation(PointPairList cloudPoints, double[,] point)
        {
            for (int i = 0; i < point.Rows(); i++)
                cloudPoints.Add(point.GetRow(i)[0], point.GetRow(i)[1]);
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

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }

        private void getMinMax(double[] data, out double min, out double max)
        {
            min = double.MaxValue;
            max = double.MinValue;
            for (int i = 0; i < data.Count(); i++)
            {
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
            //if (range.x < point && range.y > point)
            if (point > range.x)
                if (point < range.y)
                    return true;
            return false;
        }

        private bool inCloud(double[,] cloud, double[] point)
        {
            for (int i = 0; i < cloud.Rows(); i++)
            {
                if (cloud[i, 0] == point[0] && cloud[i, 1] == point[1])
                    return true;
            }
            return false;
        }

        private bool indexExists(int[] index, int x)
        {
            for (int i = 0; i < index.Length; i++)
                if (index[i] == x)
                    return true;
            return false;
        }

        private int[] getRandomIndexes(double[,] pointList, int pointsNumber)
        {
            int[] indexes = new int[pointsNumber];

            Random rand = new Random();

            for (int i = 0; i < pointsNumber; i++)
            {
                int nr;
                do
                {
                    nr = rand.Next(0, pointList.Rows());
                } while (indexExists(indexes, nr));

                indexes[i] = nr;
            }

            return indexes;
        }

        private double[,] excludePointsByIndex(double[,] initialPointList, int[] indexes)
        {
            int k = 0;
            double[,] data = new double[0, 2];

            for (int i = 0; i < initialPointList.Rows(); i++)
            {
                bool exists = false;
                for (int j = 0; j < indexes.Length; j++)
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

        private void initialiseCloudPoints()
        {
            cloud1Points = (int)numericUpDown3.Value;
            cloud2Points = (int)numericUpDown4.Value;
            cloud3Points = (int)numericUpDown5.Value;
        }

        private void clearPlots()
        {
            myPaneOriginalData.CurveList.Clear();
            myPaneFinalData.CurveList.Clear();
            myPaneFinalLine.CurveList.Clear();
            myPaneOriginalLine.CurveList.Clear();
        }

        private void replot()
        {
            myPaneOriginalData.AxisChange();
            zedGraphControl1.Invalidate();

            myPaneFinalData.AxisChange();
            zedGraphControl2.Invalidate();

            myPaneOriginalLine.AxisChange();
            zedGraphControl3.Invalidate();

            myPaneFinalLine.AxisChange();
            zedGraphControl4.Invalidate();
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {

            Console.Clear();

            initialiseCloudPoints();

            clearPlots();

            var initialPointCloud1 = new PointCloud(cloud1Points, 5, 8, 8);
            var initialPointCloud2 = new PointCloud(cloud2Points, 5, 15, 15);
            var initialPointCloud3 = new PointCloud(cloud3Points, 10, 20, 20);

            var csv = new StreamWriter("cloud.csv");
            var csvWriter = new CsvHelper.CsvWriter(csv);
            csvWriter.WriteField("X;Y;cloud index");
            csvWriter.NextRecord();

            var data1 = WriteDataToFile(initialPointCloud1, "cloud1.txt", csvWriter, 1, "cloud");
            var data2 = WriteDataToFile(initialPointCloud2, "cloud2.txt", csvWriter, 2, "cloud");
            var data3 = WriteDataToFile(initialPointCloud3, "cloud3.txt", csvWriter, 3, "cloud");

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
            list.Add(data3);
            var allData = combineData(list, cloud1Points + cloud2Points + cloud3Points);

            var bigAllData = new double[allData.Rows(), allData.Columns()];
            Array.Copy(allData, bigAllData, allData.Length);

            //bigAllData = bigAllData.Transpose();

            int excludedPointsNumber = 30;

            var indexes = getRandomIndexes(allData, excludedPointsNumber);
            allData = excludePointsByIndex(allData, indexes);


            int[] indexesInitial = new int[excludedPointsNumber];
            for (int i = 0; i < excludedPointsNumber; i++)
            {
                int j = indexes[i];
                var point = bigAllData.GetRow(j);
                if (inCloud(data1, point))
                {
                    indexesInitial[i] = 1;
                    cloud1Points--;
                }
                else if (inCloud(data2, point))
                {
                    indexesInitial[i] = 2;
                    cloud2Points--;
                }
                else if (inCloud(data3, point))
                {
                    indexesInitial[i] = 3;
                    cloud3Points--;
                }

            }
            //Console.WriteLine(a.ToString("+0.0;-0.0"));
            Console.WriteLine(indexesInitial.ToString("+0.0;-0.0"));

            PCA = new ObjectPCA(allData);
            PCA.Compute();

            var cloud1Range = new Range { x = 0, y = cloud1Points };
            var cloud2Range = new Range { x = cloud1Points, y = cloud1Points + cloud2Points };
            var cloud3Range = new Range { x = cloud2Range.y, y = cloud2Range.y + cloud3Points };

            var finalData1 = PCA.FinalData.Get((int)cloud1Range.x, (int)cloud1Range.y, 0, 2);
            var finalData2 = PCA.FinalData.Get((int)cloud2Range.x, (int)cloud2Range.y, 0, 2);
            var finalData3 = PCA.FinalData.Get((int)cloud3Range.x, (int)cloud3Range.y, 0, 2);

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

            //Console.WriteLine(r1.x + " " + r1.y + " " + r2.x + " " + r2.y + " " + r3.x + " " + r3.y + " ");

            double x, y;
            x = (double)numericUpDown1.Value;
            y = (double)numericUpDown2.Value;
            var p = PCA.plotPointPCA(new double[] { x,y});

            double[,] plottedPoints = new double[0, 2];
            for (int i = 0; i < excludedPointsNumber; i++)
            {
                plottedPoints = plottedPoints.InsertRow(PCA.plotPointPCA(bigAllData.GetRow(indexes[i])));
            }
            int[] indexesFinal = new int[excludedPointsNumber];
            for (int i = 0; i < excludedPointsNumber; i++)
            {
                if (checkPoint(r1, plottedPoints[i, 0]))
                {
                    indexesFinal[i] = 1;
                }
                else if (checkPoint(r2, plottedPoints[i, 0]))
                {
                    indexesFinal[i] = 2;
                }
                else if (checkPoint(r3, plottedPoints[i, 0]))
                {
                    indexesFinal[i] = 3;
                }
            }
            Console.WriteLine();
            Console.WriteLine(plottedPoints.ToString("+0.0;-0.0"));
            Console.WriteLine(indexesFinal.ToString("+0.0;-0.0"));


            int differentPoints = 0;
            for (int i = 0; i < excludedPointsNumber; i++)
            {
                if (indexesInitial[i] != indexesFinal[i])
                    differentPoints++;
            }
            Console.WriteLine();
            Console.WriteLine(differentPoints);
            //if (checkPoint(r1, point[0, 0])) {
            //    label1.Text = "Cloud1";
            //}
            //else if (checkPoint(r2, point[0, 0])) {
            //    label1.Text = "Cloud2";
            //}
            //else if (checkPoint(r3, point[0, 0])) {
            //    label1.Text = "Cloud3";
            //}
            //else
            //    label1.Text = "None";

            #region lines

            double[,] initialLineData1 = new double[data1.Rows(), 2];
            for (int i = 0; i < data1.Rows(); i++)
            {
                initialLineData1[i, 0] = data1[i, 0];
                initialLineData1[i, 1] = 1;
            }
            double[,] initialLineData2 = new double[data2.Rows(), 2];
            for (int i = 0; i < data2.Rows(); i++)
            {
                initialLineData2[i, 0] = data2[i, 0];
                initialLineData2[i, 1] = 1;
            }
            double[,] initialLineData3 = new double[data3.Rows(), 2];
            for (int i = 0; i < data3.Rows(); i++)
            {
                initialLineData3[i, 0] = data3[i, 0];
                initialLineData3[i, 1] = 1;
            }



            var initialLine1 = new PointPairList();
            pointPairListInitialisation(initialLine1, initialLineData1);
            var initialLine2 = new PointPairList();
            pointPairListInitialisation(initialLine2, initialLineData2);
            var initialLine3 = new PointPairList();
            pointPairListInitialisation(initialLine3, initialLineData3);

            LineItem initialLine1Points = myPaneOriginalLine.AddCurve("Cloud1", initialLine1, Color.Red, SymbolType.Circle);
            initialLine1Points.Line.IsVisible = false;
            LineItem initialLine2Points = myPaneOriginalLine.AddCurve("Cloud2", initialLine2, Color.Blue, SymbolType.Diamond);
            initialLine2Points.Line.IsVisible = false;
            LineItem initialLine3Points = myPaneOriginalLine.AddCurve("Cloud3", initialLine3, Color.Green, SymbolType.Star);
            initialLine3Points.Line.IsVisible = false;


            double[,] finalLineData1 = new double[finalData1.Rows(), 2];
            for (int i = 0; i < finalData1.Rows(); i++)
            {
                finalLineData1[i, 0] = finalData1[i, 0];
                finalLineData1[i, 1] = 1;
            }
            double[,] finalLineData2 = new double[finalData2.Rows(), 2];
            for (int i = 0; i < finalData2.Rows(); i++)
            {
                finalLineData2[i, 0] = finalData2[i, 0];
                finalLineData2[i, 1] = 1;
            }
            double[,] finalLineData3 = new double[finalData3.Rows(), 2];
            for (int i = 0; i < finalData3.Rows(); i++)
            {
                finalLineData3[i, 0] = finalData3[i, 0];
                finalLineData3[i, 1] = 1;
            }


            var finalLine1 = new PointPairList();
            pointPairListInitialisation(finalLine1, finalLineData1);
            var finalLine2 = new PointPairList();
            pointPairListInitialisation(finalLine2, finalLineData2);
            var finalLine3 = new PointPairList();
            pointPairListInitialisation(finalLine3, finalLineData3);

            LineItem finalLine1Points = myPaneFinalLine.AddCurve("Cloud1", finalLine1, Color.Red, SymbolType.Circle);
            finalLine1Points.Line.IsVisible = false;
            LineItem finalLine2Points = myPaneFinalLine.AddCurve("Cloud2", finalLine2, Color.Blue, SymbolType.Diamond);
            finalLine2Points.Line.IsVisible = false;
            LineItem finalLine3Points = myPaneFinalLine.AddCurve("Cloud3", finalLine3, Color.Green, SymbolType.Star);
            finalLine3Points.Line.IsVisible = false;


            #endregion

            var initialCloud1 = new PointPairList();
            var initialCloud2 = new PointPairList();
            var initialCloud3 = new PointPairList();


            pointPairListInitialisation(initialCloud1, data1);
            pointPairListInitialisation(initialCloud2, data2);
            pointPairListInitialisation(initialCloud3, data3);

            var finalCloud1 = new PointPairList();
            var finalCloud2 = new PointPairList();
            var finalCloud3 = new PointPairList();


            pointPairListInitialisation(finalCloud1, finalData1);
            pointPairListInitialisation(finalCloud2, finalData2);
            pointPairListInitialisation(finalCloud3, finalData3);


            InitialCloud1Points = myPaneOriginalData.AddCurve("Cloud1", initialCloud1, Color.Red, SymbolType.Circle);
            InitialCloud1Points.Line.IsVisible = false;
            InitialCloud2Points = myPaneOriginalData.AddCurve("Cloud2", initialCloud2, Color.Blue, SymbolType.Diamond);
            InitialCloud2Points.Line.IsVisible = false;
            InitialCloud3Points = myPaneOriginalData.AddCurve("Cloud3", initialCloud3, Color.Green, SymbolType.Star);
            InitialCloud3Points.Line.IsVisible = false;

            FinalCloud1Points = myPaneFinalData.AddCurve("Cloud1", finalCloud1, Color.Red, SymbolType.Circle);
            FinalCloud1Points.Line.IsVisible = false;
            FinalCloud2Points = myPaneFinalData.AddCurve("Cloud2", finalCloud2, Color.Blue, SymbolType.Diamond);
            FinalCloud2Points.Line.IsVisible = false;
            FinalCloud3Points = myPaneFinalData.AddCurve("Cloud3", finalCloud3, Color.Green, SymbolType.Star);
            FinalCloud3Points.Line.IsVisible = false;


            replot();

            //LineItem initialPoint = myPaneOriginalData.AddCurve("Point", new double[] { x }, new double[] { y }, Color.Brown, SymbolType.Triangle);
            //initialPoint.Line.IsVisible = false;
            //finalCloud1.Add(new PointPair(point[0, 0], point[1, 0]));

            //LineItem finalPoint = myPaneFinalData.AddCurve("Point", point.GetRow(0), point.GetRow(1), Color.Brown, SymbolType.Triangle);
            //finalPoint.Line.IsVisible = false;



        }

        private int testNumber = 0;
        private double totalNumber = 0;


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

		private void button2_Click(object sender, EventArgs e)
        {
            clearPlots();

            initialiseCloudPoints();



            var initialPointCloud1 = new PointCloud(cloud1Points, 4, 5, 5);
            var initialPointCloud2 = new PointCloud(cloud2Points, 4, 6, 1);


            //var csv = new StreamWriter("cloud.csv");
            //var csvWriter = new CsvHelper.CsvWriter(csv);
            //csvWriter.WriteField("X;Y;cloud index");
            //csvWriter.NextRecord();

            //var data1 = WriteDataToFile(initialPointCloud1, "cloud1.txt", csvWriter, 1, "moon", false);
            //var data2 = WriteDataToFile(initialPointCloud2, "cloud2.txt", csvWriter, 2, "moon", true);

            //csv.Flush();
            //csv.Close();



            double[,] data1;
            double[,] data2;


            ReadDataFromFile(out data1, "cloud1.txt");
            ReadDataFromFile(out data2, "cloud2.txt");


            var list = new List<double[,]>();
            list.Add(data1);
            list.Add(data2);
            var allData = combineData(list, cloud1Points + cloud2Points);


            //PCA = new ObjectPCA(allData);
            //PCA.Gamma = (double)numericUpDown5.Value;
            //PCA.Compute();
            //PCA.ComputeKernel();




            var bigAllData = new double[allData.Rows(), allData.Columns()];
            Array.Copy(allData, bigAllData, allData.Length);

            //bigAllData = bigAllData.Transpose();

            int excludedPointsNumber = 10;

            var indexes = getRandomIndexes(allData, excludedPointsNumber);
            allData = excludePointsByIndex(allData, indexes);


            double[,] cloud1InitialExcluded = new double[0, 2];
            double[,] cloud2InitialExcluded = new double[0, 2];

            int[] indexesInitial = new int[excludedPointsNumber];
            for (int i = 0; i < excludedPointsNumber; i++)
            {
                int j = indexes[i];
                var point = bigAllData.GetRow(j);
                if (inCloud(data1, point))
                {
                    cloud1InitialExcluded = cloud1InitialExcluded.InsertRow(point);
                    indexesInitial[i] = 1;
                    //cloud1Points--;
                }
                else if (inCloud(data2, point))
                {
                    cloud2InitialExcluded = cloud2InitialExcluded.InsertRow(point);
                    indexesInitial[i] = 2;
                    //cloud2Points--;
                }

            }
            //Console.WriteLine(a.ToString("+0.0;-0.0"));
            //Console.WriteLine(indexesInitial.ToString("+0.0;-0.0"));

            double gamma = 1;
            int minPoints = int.MaxValue;
            double goodGamma = 1;

            double[,] finalData1;
            double[,] finalData2;

            Range cloud1Range;
            Range cloud2Range;

            Range r1bun = new Range { x = 0, y = 0 }, r2bun = new Range { x = 0, y = 0 };

            PCA = new ObjectPCA(allData);
            //PCA.Compute();

            bool tested = false;
            //while (gamma <= 30)
            double separationPoint = 0;

            double[,] cloud1PlottedPoints = new double[0, 2];
            double[,] cloud2PlottedPoints = new double[0, 2];

            double goodSeparationPoint = 0;

            #region dowhile

            //var g1 = new double[0, 0];
            //var g2 = new double[0, 0];
            //do
            //{

            //    PCA.Gamma = gamma;
            //    PCA.ComputeKernel();
            //    //PCA.kernelAccord();
            //    cloud1Range = new Range { x = 0, y = cloud1Points };
            //    cloud2Range = new Range { x = cloud1Points, y = cloud1Points + cloud2Points };

            //    finalData1 = PCA.KernelData.Get((int)cloud1Range.x, (int)cloud1Range.y, 0, 2);
            //    finalData2 = PCA.KernelData.Get((int)cloud2Range.x, (int)cloud2Range.y, 0, 2);

            //    double min1, max1, min2, max2;

            //    getMinMax(finalData1.GetColumn(0), out min1, out max1);
            //    getMinMax(finalData2.GetColumn(0), out min2, out max2);



            //    Range r1 = new Range(), r2 = new Range();
            //    //
            //    {
            //        tested = true;
            //        double mean1 = min2 - (max1 + min2) / 2;
            //        if (max1 < min2)
            //        {
            //            Console.WriteLine("okg");
            //            separationPoint = min2 - mean1;
            //        }
            //        else
            //        {
            //            separationPoint = min2 + Math.Abs(mean1);
            //        }

            //        r1.newRange(min1 - mean1, max1 + mean1);
            //        r2.newRange(min2 - mean1, max2 + mean1);

            //        //double mean1 = (min1 + max1) / 2;
            //        //double mean2 = (min2 + max2) / 2;


            //        //Console.WriteLine(gamma+ "AM INTRAT ");
            //        //Console.Write("R1: {0} {1} R2: {2} {3} \n", r1.x, r1.y, r2.x, r2.y);
            //        //Console.WriteLine(r1.x + " " + r1.y + " " + r2.x + " " + r2.y + " " + r3.x + " " + r3.y + " ");

            //        //double x, y;
            //        //x = (double)numericUpDown1.Value;
            //        //y = (double)numericUpDown2.Value;
            //        //var p = PCA.plotPointPCA(x, y);

            //        double[,] plottedPoints = new double[0, 2];

            //        cloud1PlottedPoints = new double[0, 2];
            //        cloud2PlottedPoints = new double[0, 2];

            //        double[,] initialExcludedPoints = new double[0, 2];
            //        for (int i = 0; i < excludedPointsNumber; i++)
            //        {
            //            initialExcludedPoints = initialExcludedPoints.InsertRow(bigAllData.GetRow(indexes[i]));
            //            plottedPoints = plottedPoints.InsertRow(new double[] { PCA.plotPointKernelPCA(bigAllData.GetRow(indexes[i])), 0 });
            //            if (indexes[i] < 50)
            //            {
            //                cloud1PlottedPoints = cloud1PlottedPoints.InsertRow(new double[] { plottedPoints[i, 0], plottedPoints[i, 1] });
            //            }
            //            else
            //            {
            //                cloud2PlottedPoints = cloud2PlottedPoints.InsertRow(new double[] { plottedPoints[i, 0], plottedPoints[i, 1] });
            //            }
            //        }
            //        //Console.WriteLine("Cloud1");
            //        //Console.WriteLine(cloud1PlottedPoints.ToString("+0.0000000;-0.000000"));
            //        //Console.WriteLine("Cloud2");
            //        //Console.WriteLine(cloud2PlottedPoints.ToString("+0.0000000;-0.000000"));

            //        int[] indexesFinal = new int[excludedPointsNumber];
            //        for (int i = 0; i < excludedPointsNumber; i++)
            //        {
            //            if (plottedPoints[i, 0] < separationPoint)
            //            {

            //                indexesFinal[i] = 1;
            //            }
            //            else if (plottedPoints[i, 0] > separationPoint)
            //            {

            //                indexesFinal[i] = 2;
            //            }

            //            //Console.WriteLine("{0}, {1}, {2}, {3}", bigAllData.GetRow(indexes[i])[0], plottedPoints[i, 0], indexesInitial[i], indexesFinal[i]);
            //        }


            //        //Console.WriteLine();
            //        //Console.WriteLine(plottedPoints.ToString("+0.0;-0.0"));
            //        //Console.WriteLine(indexesFinal.ToString("+0.0;-0.0"));


            //        int differentPoints = 0;
            //        for (int i = 0; i < excludedPointsNumber; i++)
            //        {
            //            if (indexesInitial[i] != indexesFinal[i])
            //                differentPoints++;
            //        }
            //        //Console.WriteLine();
            //        //Console.WriteLine(differentPoints);
            //        //Console.WriteLine(differentPoints + " "+gamma);
            //        if (differentPoints < minPoints)
            //        {
            //            minPoints = differentPoints;
            //            goodSeparationPoint = separationPoint;
            //            goodGamma = gamma;
            //            r1bun = r1;
            //            r2bun = r2;
            //            g1 = cloud1PlottedPoints;
            //            g2 = cloud2PlottedPoints;
            //            if (max1 < min2)
            //            {
            //                Console.WriteLine("ok da");

            //            }
            //            else
            //            {
            //                Console.WriteLine("ok nu");
            //            }
            //        }

            //    }

            //    gamma += 0.1;
            //} while (gamma < 20);


            #endregion

            tested = true;
            if (tested == true)
            {
                totalNumber += minPoints;
                testNumber++;

                Console.WriteLine("Final gamma: " + goodGamma);
                Console.WriteLine("Minimum points: " + minPoints);

                //Console.WriteLine("R1: {0} {1} R2: {2} {3}", r1bun.x, r1bun.y, r2bun.x, r2bun.y);
                Console.WriteLine("Separation Point: " + goodSeparationPoint);
                Console.WriteLine();
                goodGamma = 15;
                PCA = new ObjectPCA(bigAllData);
                //PCA = new ObjectPCA(allData);
                PCA.Gamma = goodGamma;
                PCA.Compute();
                PCA.ComputeKernel();

                //Console.WriteLine("new");
                //var x = PCA.KernelData;

                //PCA = new ObjectPCA(x);
                //PCA.Compute();

                //var x = PCA.allKernelData;

                //var a = PCA.allKernelVectors.Dot(PCA.allKernelValues);
                //var b = PCA.allKernelValues.Dot(PCA.allKernelVectors);

                //var c = a.Subtract(b);




                //double plot = PCA.plotPointKernelPCA(bigAllData.GetRow(0)[0], bigAllData.GetRow(0)[1]);
                //double expected = PCA.KernelData.GetRow(0)[0];
                //Console.WriteLine("Plot " + plot);
                //Console.WriteLine("Expected " + expected);

                //PCA.kernelAccord();
                cloud1Range = new Range { x = 0, y = cloud1Points };
                cloud2Range = new Range { x = cloud1Points, y = cloud1Points + cloud2Points };

                finalData1 = PCA.KernelData.Get((int)cloud1Range.x, (int)cloud1Range.y, 0, 2);
                finalData2 = PCA.KernelData.Get((int)cloud2Range.x, (int)cloud2Range.y, 0, 2);

                //Console.WriteLine(PCA.KernelData.ToString("+0.000;-0.000"));


                double[,] plottedPoints = new double[0, 2];
                plottedPoints = plottedPoints.InsertRow(new double[] { goodSeparationPoint, 0 });





                //var excludedCloudFinal = new PointPairList();
                //pointPairListInitialisation(excludedCloudFinal, plottedPoints);
                //var excludedCloudPointsInitial = myPaneFinalData.AddCurve("Excluded points", excludedCloudFinal, Color.Green, SymbolType.Square);
                //excludedCloudPointsInitial.Line.IsVisible = false;


                //var excludedCloud1 = new PointPairList();
                //pointPairListInitialisation(excludedCloud1,g1);
                //var excludedCloud1PointsFinal = myPaneFinalData.AddCurve("Excluded points", excludedCloud1, Color.Black, SymbolType.Square);
                //excludedCloud1PointsFinal.Line.IsVisible = false;

                //var excludedCloud2 = new PointPairList();
                //pointPairListInitialisation(excludedCloud2, g2);
                //var excludedCloud2PointsFinal = myPaneFinalData.AddCurve("Excluded points", excludedCloud2, Color.Brown, SymbolType.Square);
                //excludedCloud2PointsFinal.Line.IsVisible = false;


                //var excludedCloudInitial = new PointPairList();
                //pointPairListInitialisation(excludedCloudInitial, initialExcludedPoints);
                //var excludedCloudPointsFinal = myPaneOriginalData.AddCurve("Excluded points", excludedCloudInitial, Color.Green, SymbolType.Square);
                //excludedCloudPointsFinal.Line.IsVisible = false;

                //var excludedCloud1Initial = new PointPairList();
                //pointPairListInitialisation(excludedCloud1Initial, cloud1InitialExcluded);
                //var excludedCloud1PointsInitial = myPaneOriginalData.AddCurve("Excluded points", excludedCloud1Initial, Color.Black, SymbolType.Square);
                //excludedCloud1PointsInitial.Line.IsVisible = false;

                //var excludedCloud2Initial = new PointPairList();
                //pointPairListInitialisation(excludedCloud2Initial, cloud2InitialExcluded);
                //var excludedCloud2PointsInitial = myPaneOriginalData.AddCurve("Excluded points", excludedCloud2Initial, Color.Brown, SymbolType.Square);
                //excludedCloud2PointsInitial.Line.IsVisible = false;


                #region ploting

                var initialCloud1 = new PointPairList();
                var initialCloud2 = new PointPairList();


                var initialData1 = PCA.InitialData.Get((int)cloud1Range.x, (int)cloud1Range.y, 0, 2);
                var initialData2 = PCA.InitialData.Get((int)cloud2Range.x, (int)cloud2Range.y, 0, 2);

                pointPairListInitialisation(initialCloud1, initialData1);
                pointPairListInitialisation(initialCloud2, initialData2);

                InitialCloud1Points = myPaneOriginalData.AddCurve("Cloud1", initialCloud1, Color.Red, SymbolType.Circle);
                InitialCloud1Points.Line.IsVisible = false;
                InitialCloud2Points = myPaneOriginalData.AddCurve("Cloud2", initialCloud2, Color.Blue, SymbolType.Diamond);
                InitialCloud2Points.Line.IsVisible = false;

                var finalCloud1 = new PointPairList();
                var finalCloud2 = new PointPairList();

                pointPairListInitialisation(finalCloud1, finalData1);
                pointPairListInitialisation(finalCloud2, finalData2);

                FinalCloud1Points = myPaneFinalData.AddCurve("Cloud1", finalCloud1, Color.Red, SymbolType.Circle);
                FinalCloud1Points.Line.IsVisible = false;
                FinalCloud2Points = myPaneFinalData.AddCurve("Cloud2", finalCloud2, Color.Blue, SymbolType.Diamond);
                FinalCloud2Points.Line.IsVisible = false;


                // ------------- 
                double[,] initialLineData1 = new double[data1.Rows(), 2];
                for (int i = 0; i < data1.Rows(); i++)
                {
                    initialLineData1[i, 0] = data1[i, 0];
                    initialLineData1[i, 1] = 1;
                }
                double[,] initialLineData2 = new double[data2.Rows(), 2];
                for (int i = 0; i < data2.Rows(); i++)
                {
                    initialLineData2[i, 0] = data2[i, 0];
                    initialLineData2[i, 1] = 1;
                }




                var initialLine1 = new PointPairList();
                pointPairListInitialisation(initialLine1, initialLineData1);
                var initialLine2 = new PointPairList();
                pointPairListInitialisation(initialLine2, initialLineData2);


                LineItem initialLine1Points = myPaneOriginalLine.AddCurve("Cloud1", initialLine1, Color.Red, SymbolType.Circle);
                initialLine1Points.Line.IsVisible = false;
                LineItem initialLine2Points = myPaneOriginalLine.AddCurve("Cloud2", initialLine2, Color.Blue, SymbolType.Diamond);
                initialLine2Points.Line.IsVisible = false;



                double[,] finalLineData1 = new double[finalData1.Rows(), 2];
                for (int i = 0; i < finalData1.Rows(); i++)
                {
                    finalLineData1[i, 0] = finalData1[i, 0];
                    finalLineData1[i, 1] = 1;
                }
                double[,] finalLineData2 = new double[finalData2.Rows(), 2];
                for (int i = 0; i < finalData2.Rows(); i++)
                {
                    finalLineData2[i, 0] = finalData2[i, 0];
                    finalLineData2[i, 1] = 1;
                }



                var finalLine1 = new PointPairList();
                pointPairListInitialisation(finalLine1, finalLineData1);
                var finalLine2 = new PointPairList();
                pointPairListInitialisation(finalLine2, finalLineData2);


                LineItem finalLine1Points = myPaneFinalLine.AddCurve("Cloud1", finalLine1, Color.Red, SymbolType.Circle);
                finalLine1Points.Line.IsVisible = false;
                LineItem finalLine2Points = myPaneFinalLine.AddCurve("Cloud2", finalLine2, Color.Blue, SymbolType.Diamond);
                finalLine2Points.Line.IsVisible = false;



                //-------------

                replot();

                #endregion



                WriteDataToFile(initialData1, "cloud1initial.txt", 1);
                WriteDataToFile(initialData2, "cloud2initial.txt", 1);


                label1.Text = ((totalNumber / testNumber) * 10).ToString();
            }
            else
            {
                Console.WriteLine("No gamma found");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            clearPlots();

            initialiseCloudPoints();


            var initialPointCloud1 = new PointCloud(cloud1Points, 2, 15, 15);
            var initialPointCloud2 = new PointCloud(cloud2Points, 6, 15, 15);

            var csv = new StreamWriter("cloud.csv");
            var csvWriter = new CsvHelper.CsvWriter(csv);
            csvWriter.WriteField("X;Y;cloud index");
            csvWriter.NextRecord();

            var data1 = WriteDataToFile(initialPointCloud1, "cloud1.txt", csvWriter, 1, "cloud");
            var data2 = WriteDataToFile(initialPointCloud2, "cloud2.txt", csvWriter, 2, "circle");


            csv.Flush();
            csv.Close();



            //double[,] data1;
            //double[,] data2;

            //ReadDataFromFile(out data1, "cloud1.txt");
            //ReadDataFromFile(out data2, "cloud2.txt");



            var list = new List<double[,]>();
            list.Add(data1);
            list.Add(data2);
            var allData = combineData(list, cloud1Points + cloud2Points);


            PCA = new ObjectPCA(allData);
            PCA.Gamma = (double)numericUpDown5.Value;
            PCA.Compute();
            PCA.ComputeKernel();




            var finalData1 = PCA.KernelData.Get(0, cloud1Points, 0, 2);
            var finalData2 = PCA.KernelData.Get(cloud1Points, cloud1Points + cloud2Points, 0, 2);

            var initialCloud1 = new PointPairList();
            var initialCloud2 = new PointPairList();

            var finalCloud1 = new PointPairList();
            var finalCloud2 = new PointPairList();

            pointPairListInitialisation(initialCloud1, data1);
            pointPairListInitialisation(initialCloud2, data2);

            pointPairListInitialisation(finalCloud1, finalData1);
            pointPairListInitialisation(finalCloud2, finalData2);

            InitialCloud1Points = myPaneOriginalData.AddCurve("Cloud1", initialCloud1, Color.Red, SymbolType.Circle);
            InitialCloud1Points.Line.IsVisible = false;
            InitialCloud2Points = myPaneOriginalData.AddCurve("Cloud2", initialCloud2, Color.Blue, SymbolType.Diamond);
            InitialCloud2Points.Line.IsVisible = false;

            FinalCloud1Points = myPaneFinalData.AddCurve("Cloud1", finalCloud1, Color.Red, SymbolType.Circle);
            FinalCloud1Points.Line.IsVisible = false;
            FinalCloud2Points = myPaneFinalData.AddCurve("Cloud2", finalCloud2, Color.Blue, SymbolType.Diamond);
            FinalCloud2Points.Line.IsVisible = false;

            replot();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            clearPlots();

            int cloud1points = 50;
            int cloud2points = 50;
            int cloud3points = 50;


            double[,] data1;
            double[,] data2;
            double[,] data3;

            ReadDataFromFile(out data1, "setosa.txt");
            ReadDataFromFile(out data2, "versicolor.txt");
            ReadDataFromFile(out data3, "virginica.txt");
            var list = new List<double[,]>();
            list.Add(data1);
            list.Add(data2);
            list.Add(data3);
            var allData = combineData(list, cloud1points + cloud2points + cloud3points);


            PCA = new ObjectPCA(allData);
            PCA.Compute();


            var finalData1 = PCA.Data2D.Get(0, cloud1points, 0, 2);
            var finalData2 = PCA.Data2D.Get(cloud1points, cloud1points + cloud2points, 0, 2);
            var finalData3 = PCA.Data2D.Get(cloud1points + cloud2points, cloud1points + cloud2points + cloud3points, 0, 2);


            //double min1, max1, min2, max2, min3, max3;

            //getMinMax(finalData1.GetColumn(0), out min1, out max1);
            //getMinMax(finalData2.GetColumn(0), out min2, out max2);
            //getMinMax(finalData3.GetColumn(0), out min3, out max3);

            //Range r1 = new Range(), r2 = new Range(), r3 = new Range();
            //double mean1 = min2 - (max1 + min2) / 2;
            //double mean2 = min3 - (max2 + min3) / 2;
            //r1.newRange(min1 - mean1, max1 + mean1);
            //r2.newRange(min2 - mean1, max2 + mean2);
            //r3.newRange(min3 - mean2, max3 + mean2);



            //double x, y;
            //x = (double)numericUpDown1.Value;
            //y = (double)numericUpDown2.Value;
            //var point = PCA.plotPointPCA(x, y);

            //if (checkPoint(r1, point[0, 0]))
            //{
            //    label1.Text = "Cloud1";
            //}
            //else if (checkPoint(r2, point[0, 0]))
            //{
            //    label1.Text = "Cloud2";
            //}
            //else if (checkPoint(r3, point[0, 0]))
            //{
            //    label1.Text = "Cloud3";
            //}
            //else
            //    label1.Text = "None";




            //double[,] initialLineData1 = new double[data1.Rows(), 2];
            //for (int i = 0; i < data1.Rows(); i++)
            //{
            //    initialLineData1[i, 0] = data1[i, 0];
            //    initialLineData1[i, 1] = 1;
            //}
            //double[,] initialLineData2 = new double[data2.Rows(), 2];
            //for (int i = 0; i < data2.Rows(); i++)
            //{
            //    initialLineData2[i, 0] = data2[i, 0];
            //    initialLineData2[i, 1] = 1;
            //}
            //double[,] initialLineData3 = new double[data3.Rows(), 2];
            //for (int i = 0; i < data3.Rows(); i++)
            //{
            //    initialLineData3[i, 0] = data3[i, 0];
            //    initialLineData3[i, 1] = 1;
            //}


            //var initialLine1 = new PointPairList();
            //pointPairListInitialisation(initialLine1, initialLineData1);
            //var initialLine2 = new PointPairList();
            //pointPairListInitialisation(initialLine2, initialLineData2);
            //var initialLine3 = new PointPairList();
            //pointPairListInitialisation(initialLine3, initialLineData3);

            //LineItem initialLine1Points = myPaneOriginalLine.AddCurve("Setosa", initialLine1, Color.Red, SymbolType.Circle);
            //initialLine1Points.Line.IsVisible = false;
            //LineItem initialLine2Points = myPaneOriginalLine.AddCurve("Versicolor", initialLine2, Color.Green, SymbolType.Diamond);
            //initialLine2Points.Line.IsVisible = false;
            //LineItem initialLine3Points = myPaneOriginalLine.AddCurve("Virginica", initialLine3, Color.Blue, SymbolType.Star);
            //initialLine3Points.Line.IsVisible = false;


            double[,] finalLineData1 = new double[finalData1.Rows(), 2];
            for (int i = 0; i < finalData1.Rows(); i++)
            {
                finalLineData1[i, 0] = finalData1[i, 1];
                finalLineData1[i, 1] = 1;
            }
            double[,] finalLineData2 = new double[finalData2.Rows(), 2];
            for (int i = 0; i < finalData2.Rows(); i++)
            {
                finalLineData2[i, 0] = finalData2[i, 1];
                finalLineData2[i, 1] = 1;
            }
            double[,] finalLineData3 = new double[finalData3.Rows(), 2];
            for (int i = 0; i < finalData3.Rows(); i++)
            {
                finalLineData3[i, 0] = finalData3[i, 1];
                finalLineData3[i, 1] = 1;
            }


            var finalLine1 = new PointPairList();
            pointPairListInitialisation(finalLine1, finalLineData1);
            var finalLine2 = new PointPairList();
            pointPairListInitialisation(finalLine2, finalLineData2);
            var finalLine3 = new PointPairList();
            pointPairListInitialisation(finalLine3, finalLineData3);

            LineItem finalLine1Points = myPaneFinalLine.AddCurve("Setosa", finalLine1, Color.Red, SymbolType.Circle);
            finalLine1Points.Line.IsVisible = false;
            LineItem finalLine2Points = myPaneFinalLine.AddCurve("Versicolor", finalLine2, Color.Green, SymbolType.Diamond);
            finalLine2Points.Line.IsVisible = false;
            LineItem finalLine3Points = myPaneFinalLine.AddCurve("Virginica", finalLine3, Color.Blue, SymbolType.Star);
            finalLine3Points.Line.IsVisible = false;


            //var initialCloud1 = new PointPairList();
            //var initialCloud2 = new PointPairList();
            //var initialCloud3 = new PointPairList();


            //pointPairListInitialisation(initialCloud1, data1);
            //pointPairListInitialisation(initialCloud2, data2);
            //pointPairListInitialisation(initialCloud3, data3);

            //LineItem initialCloud1Points = myPaneOriginalData.AddCurve("Setosa", initialCloud1, Color.Red, SymbolType.Circle);
            //initialCloud1Points.Line.IsVisible = false;
            //LineItem initialCloud2Points = myPaneOriginalData.AddCurve("Versicolor", initialCloud2, Color.Blue, SymbolType.Diamond);
            //initialCloud2Points.Line.IsVisible = false;
            //LineItem initialCloud3Points = myPaneOriginalData.AddCurve("Virginica", initialCloud3, Color.Green, SymbolType.Star);
            //initialCloud3Points.Line.IsVisible = false;

            //LineItem initialPoint = myPaneOriginalData.AddCurve("Point", new double[] { x }, new double[] { y }, Color.Brown, SymbolType.Triangle);
            //initialPoint.Line.IsVisible = false;


            var finalCloud1 = new PointPairList();
            var finalCloud2 = new PointPairList();
            var finalCloud3 = new PointPairList();


            pointPairListInitialisation(finalCloud1, finalData1);
            pointPairListInitialisation(finalCloud2, finalData2);
            pointPairListInitialisation(finalCloud3, finalData3);

            //finalCloud1.Add(new PointPair(point[0, 0], point[1, 0]));

            FinalCloud1Points = myPaneFinalData.AddCurve("Setosa", finalCloud1, Color.Red, SymbolType.Circle);
            FinalCloud1Points.Line.IsVisible = false;
            FinalCloud2Points = myPaneFinalData.AddCurve("Versicolor", finalCloud2, Color.Blue, SymbolType.Diamond);
            FinalCloud2Points.Line.IsVisible = false;
            FinalCloud3Points = myPaneFinalData.AddCurve("Virginica", finalCloud3, Color.Green, SymbolType.Star);
            FinalCloud3Points.Line.IsVisible = false;

            //LineItem finalPoint = myPaneFinalData.AddCurve("Point", point.GetRow(0), point.GetRow(1), Color.Brown, SymbolType.Triangle);
            //finalPoint.Line.IsVisible = false;

            replot();
        }
    }
}
