using System;
using Accord;
using Accord.Controls;
using Accord.Math;
using Accord.Math.Comparers;
using Accord.Math.Decompositions;
using Accord.Statistics;
using System.IO;
using System.Collections.Generic;

namespace AccordPCA
{
    class Program
    {
        static void Main(string[] args)
        {

            StreamReader sr = File.OpenText("data.txt");
            string text = sr.ReadToEnd();

            string[] bits = text.Split(new char[] { ' ', '\n' });
            List<double[]> doubleBits = new List<double[]>();

            int n, m;
            int.TryParse(bits[0], out n);
            int.TryParse(bits[1], out m);

            double[,] data = new double[n, m];

            int k = 2;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    double x;
                    double.TryParse(bits[k++], out x);
                    data[i, j] = x;
                }
            }


            double[] mean = data.Mean(0);


            double[,] dataAdjusted = data.Subtract(mean, 0);
            double[,] cov = dataAdjusted.Covariance();

            var evd = new EigenvalueDecomposition(cov);

            double[] eigenvalues = evd.RealEigenvalues;
            double[,] eigenvectors = evd.Eigenvectors;

            eigenvectors = Matrix.Sort(eigenvalues, eigenvectors, new GeneralComparer(ComparerDirection.Descending, true));


            double[,] featureVector0 = eigenvectors.GetColumn(0).Transpose();
            double[,] featureVector1 = eigenvectors.GetColumn(1).Transpose();
            double[,] featureVector2 = eigenvectors;



            double[,] finalData = dataAdjusted.Dot(featureVector2);

            double[,] rowZeroMeanData0 = finalData.Dot(featureVector0.Transpose());
            double[,] rowZeroMeanData1 = finalData.Dot(featureVector1.Transpose());
            double[,] rowZeroMeanData2 = finalData.Dot(featureVector2.Transpose());

            double[,] rowOriginalData0 = rowZeroMeanData0.Add(mean, 0);
            double[,] rowOriginalData1 = rowZeroMeanData1.Add(mean, 0);
            double[,] rowOriginalData2 = rowZeroMeanData2.Add(mean, 0);



            List<double[]> topl = new List<double[]>();
            for (int i = 0; i < n; i++)
            {
                topl.Add(rowOriginalData0.GetRow(i));
            }
            for (int i = 0; i < n; i++)
            {
                topl.Add(rowOriginalData1.GetRow(i));
            }
            double[][] toPlot = topl.ToArray();

            Console.WriteLine("Original data");
            Console.WriteLine(data.ToString("+0.0000;-0.0000"));
            Console.WriteLine("Mean");
            Console.WriteLine(mean.ToString("+0.000"));
            Console.WriteLine("Adjusted data");
            Console.WriteLine(dataAdjusted.ToString("+0.0000;-0.0000"));
            Console.WriteLine("Eigenvalues");
            Console.WriteLine(eigenvalues.ToString("+0.0000;-0.0000"));
            Console.WriteLine("Eigenvectors");
            Console.WriteLine(eigenvectors.ToString("+0.0000;-0.0000"));
            Console.WriteLine("Final PCA data");
            Console.WriteLine(finalData.ToString("+0.0000;-0.0000"));



            //Scatterplot scatterplot = new Scatterplot();
            //scatterplot.Compute(finalData);
            //ScatterplotView sv = new ScatterplotView(scatterplot);
            //sv.LinesVisible = true;
            //sv.CreateControl();
            //sv.Show();
            //ScatterplotBox.Show(rowOriginalData0).SetLinesVisible(true).SetSymbolSize((float)(0.10));




            ScatterplotBox.Show("Original data", data);
            ScatterplotBox.Show("PCA final data", finalData);
            ScatterplotBox.Show("Row zero mean data", rowZeroMeanData2);
            ScatterplotBox.Show("Reconstructed data from PCA final data", rowOriginalData2);




        }
    }
}
