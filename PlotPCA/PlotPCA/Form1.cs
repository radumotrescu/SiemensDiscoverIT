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

namespace PlotPCA {
    public partial class Form1 : Form {



        static double[,] WriteDataToFile(PointCloud pointCloud, string filePath)
        {
            StreamWriter sw = new StreamWriter(filePath);
            int totalRows = pointCloud.Count;
            int totalColumns = 2;
            sw.WriteLine(totalRows + " " + totalColumns);
            var doubles = pointCloud.ReturnDoubleMatrix();
            for (int i = 0; i < totalRows; i++) {
                sw.WriteLine(doubles[i, 0] + " " + doubles[i, 1]);
            }
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

        static void PointPairListInitialisation(PointPairList cloudPoints, string fileName)
        {
            double[,] cloudData;
            int pointNumberCloud;

            ReadDataFromFile(out cloudData, out pointNumberCloud, fileName);
            for (int i = 0; i < pointNumberCloud; i++) {
                cloudPoints.Add(cloudData[i, 0], cloudData[i, 1]);
            }
        }

        GraphPane myPane;
        public Form1()
        {
            InitializeComponent();

            myPane = zedGraphControl1.GraphPane;
            myPane.Title.Text = "PCA Plot Data";
            myPane.XAxis.Title.Text = "X Coordinates";
            myPane.YAxis.Title.Text = "Y Coordinates";
            zedGraphControl1.IsShowPointValues = true;


        }

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            myPane.CurveList.Clear();

            var pointCloud1 = new PointCloud(15, 2, 5, 5);
            var pointCloud2 = new PointCloud(25, 4, 10, 10);
            var pointCloud3 = new PointCloud(100, 10, 30, 30);

            var data1=WriteDataToFile(pointCloud1, "cloud1.txt");
            var data2=WriteDataToFile(pointCloud2, "cloud2.txt");
            var data3=WriteDataToFile(pointCloud3, "cloud3.txt");












            PointPairList cloud1 = new PointPairList();
            PointPairList cloud2 = new PointPairList();
            PointPairList cloud3 = new PointPairList();

            PointPairListInitialisation(cloud1, "cloud1.txt");
            PointPairListInitialisation(cloud2, "cloud2.txt");
            PointPairListInitialisation(cloud3, "cloud3.txt");

            LineItem cloud1Points = myPane.AddCurve("Cloud1", cloud1, Color.Red, SymbolType.Circle);
            cloud1Points.Line.IsVisible = false;
            LineItem cloud2Points = myPane.AddCurve("Cloud2", cloud2, Color.Blue, SymbolType.Diamond);
            cloud2Points.Line.IsVisible = false;
            LineItem cloud3Points = myPane.AddCurve("Cloud3", cloud3, Color.Green, SymbolType.Star);
            cloud3Points.Line.IsVisible = false;

            myPane.AxisChange();
            zedGraphControl1.Invalidate();
        }
    }
}
