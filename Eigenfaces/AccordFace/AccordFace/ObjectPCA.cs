using System;
using Accord.Math;
using Accord.Math.Comparers;
using Accord.Math.Decompositions;
using Accord.Statistics;
using System.Collections.Generic;
using Accord.MachineLearning;
using System.IO;

namespace AccordPCA {


    class ObjectPCA {
        /// <summary>
        /// ObjectPCA constructor
        /// </summary>
        /// <param name="data">Matrix to compute the PCA algorithm on</param>
        public ObjectPCA(double[,] data)
        {
            this.initialData = data;
            Gamma = 0;

        }

        private double[,] initialData;
        private double mean;
        private double[,] dataAdjusted;
        private double[,] covarianceMatrix;
        private EigenvalueDecomposition evd;
        private double[] eigenvalues;
        private double[,] eigenvectors;
        private double[,] finalData;
        private double[,] data2D;

        /// <summary>
        /// The data to compute the PCA on
        /// </summary>
        public double[,] InitialData
        {
            get { return initialData; }
            set { initialData = value; }
        }

        /// <summary>
        /// The means of the initial data matrix
        /// </summary>
        public double Mean
        {
            get { return mean; }
        }

        /// <summary>
        /// The adjusted data of the initial data matrix : DataAdjusted = InitialData - Mean
        /// </summary>
        public double[,] DataAdjusted
        {
            get { return dataAdjusted; }
        }

        /// <summary>
        /// The covariance matrix of the adjusted data matrix : 
        /// cov(x, y) = (sum of the multiplication of each pair of data in the x and y dimensions) divided by (number of samples - 1) 
        /// DataAdjusted = { cov(x,x) cov(x,y) }
        ///              = { cov(y,x) cov(y,y) }
        /// }
        /// </summary>
        public double[,] CovarianceMatrix
        {
            get { return covarianceMatrix; }
        }

        /// <summary>
        /// The eigenvalues extracted from the covariance matrix from the determinant of |CovarianceMatrix - x*I|
        /// </summary>
        public double[] Eigenvalues
        {
            get { return eigenvalues; }
        }

        /// <summary>
        /// The eigenvectors extracted from the multiplication of (CovarianceMatrix - x*I) and the respective eigenvector for x eigenvalue
        /// </summary>
        public double[,] Eigenvectors
        {
            get { return eigenvectors; }
        }

        /// <summary>
        /// The final data resulted from multiplying DataAjusted and the descending order sorted eigenvectors
        /// </summary>
        public double[,] FinalData
        {
            get { return finalData; }
        }

        /// <summary>
        /// The first two components of the final data matrix
        /// </summary>
        public double[,] Data2D
        {
            get { return data2D; }
        }


        /// <summary>   
        /// Nethod to compute the PCA for the given data
        /// </summary>
        public void Compute()
        {
            initialData = initialData.Transpose();

            mean = initialData.Mean();

            dataAdjusted = initialData.Subtract(mean);
            covarianceMatrix = dataAdjusted.Covariance();

            evd = new EigenvalueDecomposition(covarianceMatrix);

            eigenvalues = evd.RealEigenvalues;
            eigenvectors = evd.Eigenvectors;

            eigenvectors = Matrix.Sort(eigenvalues, eigenvectors, new GeneralComparer(ComparerDirection.Descending, true));

            //Console.WriteLine("eigenvectors pca\n" + eigenvectors.ToString("+0.000;-0.000"));
            //Console.WriteLine("eigenvalues pca\n" + eigenvalues.ToString("+0.00000;-0.000000"));

            finalData = dataAdjusted.Dot(eigenvectors);

            data2D = finalData.GetColumns(0, 1);
        }


        /// <summary>
        /// Method to print the values of all the relevant PCA parts
        /// </summary>
        public void PrintToConsole()
        {
            Console.WriteLine("Original data");
            Console.WriteLine(initialData.ToString("+0.0000;-0.0000"));
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

        }

        /// <summary>
        /// Method to show the scatterplots for the original and resulted PCA data
        /// </summary>
        //public void ShowScatterplots()
        //{
        //    ScatterplotBox.Show("Original data", initialData);
        //    ScatterplotBox.Show("PCA final data", finalData);
        //}


        public double[] plotPointPCA(double[] row)
        {



            //Console.WriteLine(Eigenvectors.ToString("+0.00;-0.00"));

            var newCloudAdjusted = row.Subtract(this.mean);

            //Console.WriteLine("Mean: " + mean.ToString("+0.0;-0.0"));
            //Console.WriteLine("Cloud adjusted :" + newCloudAdjusted.ToString("+0.0;-0.0"));

            var yPrim = newCloudAdjusted.Dot(Eigenvectors.Transpose());

            //yPrim = Matrix.Sort(eigenvalues, yPrim, new GeneralComparer(ComparerDirection.Ascending, true));


            //Console.WriteLine("yprim :" + yPrim.ToString("+0.0;-0.0"));


            return yPrim;
        }

        //public double faceRecognition(double[] row)
        //{
        //    var yPrim = plotPointPCA(row);

        //    var min = Double.MaxValue;
        //    var minIndex = 0;
        //    for (int i = 0; i < yPrim.Length; i++)
        //    {
        //        var x = eigenvectors.GetColumn(0).Subtract(yPrim);
        //        double aux = 0;
        //        for (int j = 0; j < x.Length; j++)
        //            aux += Math.Pow(x[j], 2);
        //        if (Math.Sqrt(aux) < min)
        //        {
        //            min = Math.Sqrt(aux);
        //            minIndex = i;
        //        }
        //    }

        //    Console.WriteLine(min + "  " + minIndex);
        //    // Console.WriteLine(finalData[minIndex, 0] + " " + finalData[minIndex, 1]);
        //    // Console.WriteLine(initialData[minIndex, 0] + " " + initialData[minIndex, 1]);
        //    Console.WriteLine();
        //    return min;
        //}


        public void pointRecognition(double[] row)
        {
            var yPrim = plotPointPCA(row);

            Console.WriteLine();
            Console.WriteLine(yPrim.ToString("+0.00;-0.00"));

            var S = finalData.Subtract(yPrim.Transpose().GetRow(0), 0);

            Console.WriteLine();
            //Console.WriteLine(S.ToString("+0.00;-0.00"));

            var final = new double[S.Rows()];
            var min = Double.MaxValue;
            var minIndex = 0;
            for (int i = 0; i < final.Rows(); i++)
            {
                final[i] = Math.Sqrt(Math.Pow(S[i, 0], 2) + Math.Pow(S[i, 1], 2));
                if (final[i] < min)
                {
                    min = final[i];
                    minIndex = i;
                }
            }

            Console.WriteLine(min);
            Console.WriteLine(finalData[minIndex, 0] + " " + finalData[minIndex, 1]);
            Console.WriteLine(initialData[minIndex, 0] + " " + initialData[minIndex, 1]);



        }



        public double[,] KernelData
        {
            get;
            set;
        }

        public double Gamma
        {
            get;
            set;
        }

        public double KernelValues
        {
            get;
            set;
        }

        public double[] KernelVectors
        { get; set; }

        //public double plotPointKernelPCA2(double x, double y)
        //{
        //    var point = new double[1, 2];
        //    point[0, 0] = x;
        //    point[0, 1] = y;
        //    int nr = initialData.Rows();

        //    double[] distance = new double[nr];
        //    for (int i = 0; i < initialData.Rows(); i++)
        //    {
        //        distance[i] = (Math.Pow(x - (InitialData.GetRow(i)[0]), 2)) + (Math.Pow(y - (InitialData.GetRow(i)[1]), 2));
        //    }

        //    double[] k = new double[nr];
        //    double[] normalize = new double[nr];
        //    for (int i = 0; i < nr; i++)
        //    {
        //        //double aux = (-Gamma * Math.Pow(distance[i], 2));
        //        double aux = (-Gamma * distance[i]);
        //        k[i] = Math.Pow(Math.E, aux);
        //        normalize[i] = KernelVectors[i] / KernelValues;
        //    }

        //    var toReturn = k.Dot(normalize);
        //    return toReturn;
        //}

        public double plotPointKernelPCA(double[] row)
        {
            int nr = initialData.Rows();

            double[] distance = new double[nr];
            for (int i = 0; i < initialData.Rows(); i++)
            {
                double aux = 0;
                //Console.WriteLine("i" + i);
                for (int j = 0; j < row.Rows(); j++)
                {
                    aux += (Math.Pow(row[j] - (InitialData.GetRow(i)[j]), 2));
                }
                distance[i] = aux;
            }
            //Console.WriteLine("done");
            double[] k = new double[nr];
            double[] normalize = new double[nr];
            for (int i = 0; i < nr; i++)
            {
                //double aux = (-Gamma * Math.Pow(distance[i], 2));
                double aux = (-Gamma * distance[i]);
                k[i] = Math.Pow(Math.E, aux);
                normalize[i] = KernelVectors[i] / KernelValues;
            }

            var toReturn = k.Dot(normalize);
            return toReturn;
        }

        public double[] getDistances(double[] row)
        {


            var S = initialData.Subtract(row, 0);

            var final = new double[S.Rows()];
            for (int i = 0; i < initialData.Rows(); i++)
            {
                final[i] = 0;
                for (int j = 0; j < initialData.Columns(); j++)
                    final[i] += Math.Pow(S[i, j], 2);
                //Console.WriteLine(final[i] + " " + i);
                //final[i] = final[i] / Math.Pow(10, 5);
            }

            return final;
        }

        public void ComputeKernel()
        {



            double[,] distanceMatrix = new double[0, 0];
            for (int i = 0; i < initialData.Rows(); i++)
            {
                //Console.WriteLine(i);
                distanceMatrix = distanceMatrix.InsertRow(getDistances(initialData.GetRow(i)));
            }
            int nr = distanceMatrix.Rows();

            double[,] ones = new double[nr, nr];

            double[,] k = new double[nr, nr];
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nr; j++)
                {
                    //double x = (-Gamma * Math.Pow(distanceMatrix[i, j], 2));
                    double x = (-Gamma * distanceMatrix[i, j]);
                    k[i, j] = Math.Pow(Math.E, x);
                    ones[i, j] = 1.0 / nr;
                }
            }

            var p1 = ones.Dot(k);
            var p2 = k.Dot(ones);

            //var final = k.Subtract(p1).Subtract(p2).Add(p1.Dot(ones));
            var final = k.Subtract(ones.Dot(k)).Subtract(k.Dot(ones)).Add(ones.Dot(k).Dot(ones));
            //final = final.Transpose();

            var e = new EigenvalueDecomposition(final);
            var values = e.RealEigenvalues;
            var vectors = e.Eigenvectors;


            //StreamWriter sw = new StreamWriter("eigenvectorsunsorted.txt");
            //StreamWriter sw1 = new StreamWriter("eigenvaluesunsorted.txt");
            //for (int i = 0; i < vectors.Rows(); i++)
            //{
            //    sw.WriteLine(vectors.GetRow(i).ToString("+0.00000;-0.00000"));
            //    sw1.WriteLine(values[i]);
            //}
            //sw.Close();
            //sw1.Close();

            vectors = Matrix.Sort(values, vectors, new GeneralComparer(ComparerDirection.Descending, true));

            //var coloane = vectors.GetColumns(0, 1);

            //var kernelData = dataAdjusted.Dot(coloane);
            //for (int i = 0; i < dataAdjusted.Rows(); i++)
            //    Console.WriteLine(dataAdjusted[i, 0] + " " + vectors[i, 0] + " " + kernelData[i, 0] + " " + kernelData[i, 1]);


            //KernelData = new double[vectors.Rows(), vectors.Rows()];
            //for (int i = 0; i < vectors.Columns(); i++)
            //    KernelData.InsertColumn(vectors.GetColumn(i));
            KernelData = vectors;

            allKernelValues = values;
            allKernelVectors = vectors;
            allKernelData = vectors;

            KernelValues = values[0];
            KernelVectors = vectors.GetColumn(0);

            //sw = new StreamWriter("kerneldata.txt");
            //sw1 = new StreamWriter("eigenvaluessorted.txt");
            //for (int i = 0; i < vectors.Rows(); i++)
            //{
            //    sw.WriteLine(KernelData.GetRow(i).ToString("+0.00000;-0.00000"));
            //    sw1.WriteLine(values[i]);
            //}
            //sw.Close();
            //sw1.Close();

            //Console.WriteLine(distanceMatrix.ToString("+0.00;-0.00"));
            //Console.WriteLine();
            //Console.WriteLine(k.ToString("+0.00;-0.00"));
            //Console.WriteLine();
            //Console.WriteLine(vectors.ToString("+0.00;-0.00"));



            //ScatterplotBox.Show("kernelData", kernelData);
        }


        public double[,] kernelAccord()
        {

            var method = Accord.Statistics.Analysis.PrincipalComponentMethod.Center;
            var pca = new Accord.Statistics.Analysis.KernelPrincipalComponentAnalysis(new Accord.Statistics.Kernels.Gaussian(Gamma), method);
            pca.Learn(InitialData);

            pca.NumberOfOutputs = 50;
            double[,] actual = pca.Transform(InitialData);

            return actual;
        }

        public double[] allKernelValues { get; set; }

        public double[,] allKernelVectors { get; set; }

        public double[,] allKernelData { get; set; }


        double[] getNorms(double[,] matrix)
        {
            var norms = new double[matrix.Columns()];
            for (int i = 0; i < matrix.Columns(); i++)
            {
                double aux = 0;
                for (int j = 0; j < matrix.Rows(); j++)
                {
                    double x = matrix[j, i] * matrix[j, i];
                    aux += x;
                }

                norms[i] = Math.Sqrt(aux);
            }
            return norms;
        }


        public double[,] W
        {
            get;
            set;
        }

        public double[,] S
        {
            get;
            set;
        }
        public void take2()
        {
            var means = initialData.Mean(0);
            var matrix = initialData.Subtract(means, 1);
            var c = matrix.Dot(matrix.Transpose());
            c = c.Divide(initialData.Rows());

            var evd = new EigenvalueDecomposition(c);

            eigenvalues = evd.RealEigenvalues;
            eigenvectors = evd.Eigenvectors;

            eigenvectors = Matrix.Sort(eigenvalues, eigenvectors, new GeneralComparer(ComparerDirection.Descending, true));

            var norms = getNorms(eigenvectors);
            eigenvectors = eigenvectors.Divide(norms, 0);

            W = matrix.Transpose().Dot(eigenvectors);

            var x1 = eigenvectors.GetColumn(0).Transpose().Dot(matrix.GetRow(0).Transpose());
        }

        double sqEuclidianDistance(double[] row1, double[] row2)
        {
            double val = 0;
            for (int i = 0; i < row1.Length; i++)
                val += (row1[i] - row2[i]);
            return val;
        }

        public double projectImage(double[,] image)
        {
            var mean = image.Mean(0);
            image = image.Subtract(mean, 0);
            S = image.Dot(eigenvectors.GetColumn(0).Transpose());

            var diff = W.Subtract(S.GetColumn(0), 0);

            var norms = getNorms(diff);
            var minVal = double.MaxValue;
            var minIndex = int.MaxValue;
            for (int i = 0; i < norms.Length; i++)
                if (norms[i] < minVal)
                {
                    minVal = norms[i];
                    minIndex = i;
                }
            return minVal;
        }

        public double MaxValue 
        {
            get;
            set;
        }

        public void setMaxValue()
        {
            MaxValue = double.MinValue;
            double sum = 0;
            for (int i = 0; i < initialData.Rows(); i++)
            {
                double value = projectImage(initialData.GetRow(i).Transpose());
                //Console.WriteLine(value + " " +i);
                if (value > MaxValue)
                    MaxValue = value;
                sum += value;
            }
            Console.WriteLine("SET MAX VALUE");
            MaxValue = sum / initialData.Rows();
            //double[,] matrice = new double[W.Columns(), W.Columns()];
            //for (int i = 0; i < W.Columns(); i++)
            //    for (int j = 0; j < W.Columns(); j++)
            //        matrice[i, j] = sqEuclidianDistance(W.GetColumn(i), W.GetColumn(j));

            //var norms = getNorms(matrice);
            //int index = 0;
            //MaxValue = double.MinValue;
            //for (int i = 0; i < norms.Length; i++)
            //{
            //    for (int j = 0; j < norms.Length; j++)
            //        if (norms[i] - norms[j] > MaxValue)
            //        {
            //            MaxValue = norms[i] - norms[j];
            //        }
            //}
            //double maxValue = double.MinValue;
            //foreach (double d in norms)
            //    if (d > MaxValue)
            //        MaxValue = d;
        }
    }
}
