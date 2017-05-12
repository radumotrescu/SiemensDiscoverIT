﻿using System;
using Accord.Math;
using Accord.Math.Comparers;
using Accord.Math.Decompositions;
using Accord.Statistics;
using System.Collections.Generic;

namespace AccordPCA {


    class ObjectPCA {
        /// <summary>
        /// ObjectPCA constructor
        /// </summary>
        /// <param name="data">Matrix to compute the PCA algorithm on</param>
        public ObjectPCA(double[,] data)
        {
            this.initialData = data;

        }

        private double[,] initialData;
        private double[] mean;
        private double[,] dataAdjusted;
        private double[,] covarianceMatrix;
        private EigenvalueDecomposition evd;
        private double[] eigenvalues;
        private double[,] eigenvectors;
        private double[,] finalData;

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
        public double[] Mean
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
        /// Nethod to compute the PCA for the given data
        /// </summary>
        public void Compute()
        {
            mean = initialData.Mean(0);

            dataAdjusted = initialData.Subtract(mean, 0);
            covarianceMatrix = dataAdjusted.Covariance();

            evd = new EigenvalueDecomposition(covarianceMatrix);

            eigenvalues = evd.RealEigenvalues;
            eigenvectors = evd.Eigenvectors;

            eigenvectors = Matrix.Sort(eigenvalues, eigenvectors, new GeneralComparer(ComparerDirection.Descending, true));

            finalData = dataAdjusted.Dot(eigenvectors);
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

        public void pointRecognition(double x, double y)
        {
            var point = new double[1, 2];
            point[0, 0] = x;
            point[0, 1] = y;

            Console.WriteLine(Eigenvectors.ToString("+0.00;-0.00"));

            var newCloudAdjusted = point.Subtract(this.mean, 0);

            var yPrim = Eigenvectors.Dot(newCloudAdjusted.Transpose());

            Console.WriteLine();
            Console.WriteLine(yPrim.ToString("+0.00;-0.00"));

            var S = finalData.Subtract(yPrim.Transpose().GetRow(0), 0);

            Console.WriteLine();
            //Console.WriteLine(S.ToString("+0.00;-0.00"));

            var final = new double[S.Rows()];
            var min = Double.MaxValue;
            var minIndex = 0;
            for (int i = 0; i < final.Rows(); i++) {
                final[i] = Math.Sqrt(Math.Pow(S[i, 0], 2) + Math.Pow(S[i, 1], 2));
                if (final[i] < min) {
                    min = final[i];
                    minIndex = i;
                }
            }

            Console.WriteLine(min);
            Console.WriteLine(finalData[minIndex, 0] + " " + finalData[minIndex, 1]);
            Console.WriteLine(initialData[minIndex, 0] + " " + initialData[minIndex, 1]);

        }
    }
}
