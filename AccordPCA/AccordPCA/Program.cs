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

        static void ReadData(out double[,] data, out int n)
        {
            //NumberGenerator ng = new NumberGenerator();
            //ng.WriteToFile("data.txt", 25, 2, 4.0, 4.0, 2.0, 10.0, 10.0, 2.0);

            StreamReader sr = new StreamReader("data.txt");
            string text = sr.ReadToEnd();
            sr.Close();

            string[] bits = text.Split(new char[] { ' ', '\n' });
            List<double[]> doubleBits = new List<double[]>();

            int  m;
            int.TryParse(bits[0], out n);
            int.TryParse(bits[1], out m);

            data = new double[n, m];

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
        }


        static void Main(string[] args)
        {

            double[,] data;
            int n;
            ReadData(out data, out n);

            ObjectPCA obj1 = new ObjectPCA(data, n);
            obj1.Compute();

        }
    }
}
