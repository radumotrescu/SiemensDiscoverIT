using System;
using System.Collections.Generic;
using Accord.Controls;

//double valx = angle * Math.Cos(radius) * radius1;
//double valy = angle * Math.Sin(radius) * radius1;

namespace AccordPCA {
	class PointCloud {

		/// <summary>
		/// Structure to be used for specifying a point in a (x,y) coordonate system
		/// </summary>
		private struct Point {
			public double x, y;
		}

		private double maxRadius;
		private Random random;
		private List<Point> pointList;
		private Point basePoint;
		private int count;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="count">The number of points in the cloud</param>
		/// <param name="maxRadius">The radius of the cloud</param>
		/// <param name="x">The x coordinate of the base cloud point</param>
		/// <param name="y">The y coordinate of the base cloud point</param>
		public PointCloud(int count, int maxRadius, double x, double y)
		{
			random = new Random();
			pointList = new List<Point>();
			Count = count;
			MaxRadius = maxRadius;
			basePoint = new Point { x = x, y = y };

		}


		/// <summary>
		/// Represents the radius of the cloud point
		/// </summary>
		public double MaxRadius {
			get { return maxRadius; }
			set { maxRadius = value; }
		}

		/// <summary>
		/// Represents the number of points in the cloud
		/// </summary>
		public int Count {
			get { return count; }
			set { count = value; }
		}

		/// <summary>
		/// Method to generate the cloud point with uniform spread
		/// </summary>
        private void generateRoundCloud()
        {
            for (var i = 0; i < count; i++)
            {
                var angle = getDouble(0, 1);
                var radius = getDouble(0, 2 * Math.PI);


                var valx = Math.Sqrt(angle) * Math.Cos(radius) * maxRadius;
                var valy = Math.Sqrt(angle) * Math.Sin(radius) * maxRadius;

                var newPoint = new Point { x = basePoint.x + valx, y = basePoint.y + valy };
                pointList.Add(newPoint);
            }
        }

		/// <summary>
		/// Method to return the matrix representation of the cloud point
		/// </summary>
		/// <returns></returns>
		public double[,] ReturnDoubleMatrix()
		{
			generateRoundCloud();
			var data = new double[count, 2];
			for (var i = 0; i < count; i++) {
				data[i, 0] = pointList[i].x;
				data[i, 1] = pointList[i].y;
			}
            //ScatterplotBox.Show(data);
			return data;

		}

		/// <summary>
		/// Method to return a random double value in the specified range
		/// </summary>
		/// <param name="min">The lower bound of the random number to be generated</param>
		/// <param name="max">The upper bound of the random number to be generated</param>
		/// <returns></returns>
		private double getDouble(double min, double max)
		{
			return min + (random.NextDouble() * (max - min));
		}


	}
}
