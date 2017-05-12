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
                var count = pointCloud.Count;
                for (var i = 0; i < count; i++) {
                    sw.WriteLine(doubles[i, 0] + " " + doubles[i, 1]);

                }

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
            for (var i = 0; i < count; i++) {
                for (var j = 0; j < m; j++) {
                    double x;
                    double.TryParse(bits[k++], out x);
                    data[i, j] = x;
                }
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



        }

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            var pointCloud1 = new PointCloud(10, 2, 5, 5);
            var pointCloud2 = new PointCloud(25, 4, 10, 10);
            var pointCloud3 = new PointCloud(50, 10, 25, 25);

            var cloudList = new List<PointCloud>();
            cloudList.Add(pointCloud1);
            WriteDataToFile(cloudList, "cloud1.txt");
            cloudList.Clear();
            cloudList.Add(pointCloud2);
            WriteDataToFile(cloudList, "cloud2.txt");
            cloudList.Clear();
            cloudList.Add(pointCloud3);
            WriteDataToFile(cloudList, "cloud3.txt");




            myPane.CurveList.Clear();

            PointPairList cloud1 = new PointPairList();
            PointPairList cloud2 = new PointPairList();
            PointPairList cloud3 = new PointPairList();


            double[,] cloud1Data;
            int pointNumberCloud1;
            ReadDataFromFile(out cloud1Data, out pointNumberCloud1, "cloud1.txt");
            for (int i = 0; i < pointNumberCloud1; i++) {

                cloud1.Add(cloud1Data[i, 0], cloud1Data[i, 1]);

            }


            double[,] cloud2Data;
            int pointNumberCloud2;
            ReadDataFromFile(out cloud2Data, out pointNumberCloud2, "cloud2.txt");
            for (int i = 0; i < pointNumberCloud2; i++) {

                cloud2.Add(cloud2Data[i, 0], cloud2Data[i, 1]);

            }

            double[,] cloud3Data;
            int pointNumberCloud3;
            ReadDataFromFile(out cloud3Data, out pointNumberCloud3, "cloud3.txt");
            for (int i = 0; i < pointNumberCloud3; i++) {

                cloud3.Add(cloud3Data[i, 0], cloud3Data[i, 1]);

            }

            LineItem cloud1Points = myPane.AddCurve("Cloud1", cloud1, Color.Red, SymbolType.Circle);
            cloud1Points.Line.IsVisible = false;
            LineItem cloud2Points = myPane.AddCurve("Cloud2", cloud2, Color.Blue, SymbolType.Diamond);
            cloud2Points.Line.IsVisible = false;
            LineItem cloud3Points = myPane.AddCurve("Cloud3", cloud3, Color.Green, SymbolType.Star);
            cloud3Points.Line.IsVisible = false;

            zedGraphControl1.IsShowPointValues = true;
            myPane.AxisChange();
            zedGraphControl1.Invalidate();

        }
    }
}
