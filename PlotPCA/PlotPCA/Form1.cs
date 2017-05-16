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


        static void ReadDataFromFile(double[,] data, string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            string text = sr.ReadToEnd();
            sr.Close();

        }

        static double[,] WriteDataToFile(PointCloud pointCloud, string filePath, CsvHelper.CsvWriter csvWriter, int cloudIndex)
        {


            StreamWriter sw = new StreamWriter(filePath);
            int totalRows = pointCloud.Count;
            int totalColumns = 2;
            sw.WriteLine(totalRows + " " + totalColumns);
            var doubles = pointCloud.ReturnDoubleMatrix();
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

        static void PointPairListInitialisation(PointPairList cloudPoints, string fileName)
        {
            double[,] cloudData;
            int pointNumberCloud;

            ReadDataFromFile(out cloudData, out pointNumberCloud, fileName);
            for (int i = 0; i < pointNumberCloud; i++) {
                cloudPoints.Add(cloudData[i, 0], cloudData[i, 1]);
            }
        }

        static void PointPairListInitialisation(PointPairList cloudPoints, PointCloud point)
        {
            double[,] cloudData = point.ReturnRawDoubleMatrix();
            for (int i = 0; i < point.Count; i++) {
                cloudPoints.Add(cloudData[i, 0], cloudData[i, 1]);
            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            myPaneOriginalData.CurveList.Clear();
            myPaneFinalData.CurveList.Clear();

            var initialPointCloud1 = new PointCloud(15, 2, 5, 5);
            var initialPointCloud2 = new PointCloud(25, 4, 10, 10);
            var initialPointCloud3 = new PointCloud(100, 10, 30, 30);

            //var csv = new StreamWriter("cloud.csv");
            //var csvWriter = new CsvHelper.CsvWriter(csv);
            //csvWriter.WriteField("X;Y;cloud index");
            //csvWriter.NextRecord();

            //var data1 = WriteDataToFile(initialPointCloud1, "cloud1.txt", csvWriter, 1);
            //var data2 = WriteDataToFile(initialPointCloud2, "cloud2.txt", csvWriter, 2);
            //var data3 = WriteDataToFile(initialPointCloud3, "cloud3.txt", csvWriter, 3);

            //csv.Flush();
            //csv.Close();



            double[,] data1;
            double[,] data2;
            double[,] data3;

            ReadDataFromFile(out data1, "cloud1.txt");
            ReadDataFromFile(out data2, "cloud2.txt");
            ReadDataFromFile(out data3, "cloud3.txt");
            var list = new List<double[,]>();
            list.Add(data1);
            list.Add(data2);
            list.Add(data3);
            var allData = combineData(list, 140);

            ObjectPCA obj1 = new ObjectPCA(data1);
            obj1.Compute();
            ObjectPCA obj2 = new ObjectPCA(data2);
            obj2.Compute();
            ObjectPCA obj3 = new ObjectPCA(data3);
            obj3.Compute();


            var obj4 = new ObjectPCA(allData);
            obj4.Compute();


            var finalPointCloud1 = new PointCloud(obj4.FinalData.Get(0, 15, 0, 2), 15);
            var finalPointCloud2 = new PointCloud(obj4.FinalData.Get(15, 40, 0, 2), 25);
            var finalPointCloud3 = new PointCloud(obj4.FinalData.Get(40, 140, 0, 2), 100);



            PointPairList initialCloud1 = new PointPairList();
            PointPairList initialCloud2 = new PointPairList();
            PointPairList initialCloud3 = new PointPairList();

            PointPairListInitialisation(initialCloud1, "cloud1.txt");
            PointPairListInitialisation(initialCloud2, "cloud2.txt");
            PointPairListInitialisation(initialCloud3, "cloud3.txt");

            LineItem initialCloud1Points = myPaneOriginalData.AddCurve("Cloud1", initialCloud1, Color.Red, SymbolType.Circle);
            initialCloud1Points.Line.IsVisible = false;
            LineItem initialCloud2Points = myPaneOriginalData.AddCurve("Cloud2", initialCloud2, Color.Blue, SymbolType.Diamond);
            initialCloud2Points.Line.IsVisible = false;
            LineItem initialCloud3Points = myPaneOriginalData.AddCurve("Cloud3", initialCloud3, Color.Green, SymbolType.Star);
            initialCloud3Points.Line.IsVisible = false;


            var finalCloud1 = new PointPairList();
            var finalCloud2 = new PointPairList();
            var finalCloud3 = new PointPairList();

            PointPairListInitialisation(finalCloud1, finalPointCloud1);
            PointPairListInitialisation(finalCloud2, finalPointCloud2);
            PointPairListInitialisation(finalCloud3, finalPointCloud3);

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
