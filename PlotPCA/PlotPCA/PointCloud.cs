using System;
using System.Collections.Generic;


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
        /// <param name="data">The data to initialise the point cloud with</param>
        /// <param name="count">Number of points</param>
        public PointCloud(double[,] data, int count)
        {
            pointList = new List<Point>();
            Count = count;
            for (int i = 0; i < count; i++)
            {
                var newPoint = new Point { x = data[i, 0], y = data[i, 1] };
                pointList.Add(newPoint);
            }
        }


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
        public double MaxRadius
        {
            get { return maxRadius; }
            set { maxRadius = value; }
        }

        /// <summary>
        /// Represents the number of points in the cloud
        /// </summary>
        public int Count
        {
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

        private void generateMoonCloud(bool up)
        {
            for (var i = 0; i < count; i++)
            {

                double r = maxRadius;

                var valx = getDouble(-r, r);

                double valy = Math.Sqrt(Math.Pow(r, 2) - Math.Pow(valx, 2));

                Point newPoint;
                if (up == false)
                {
                    newPoint = new Point { x = valx + basePoint.x, y = -valy + basePoint.y };
                }
                else
                {
                    newPoint = new Point { x = valx + basePoint.x, y = valy + basePoint.y };
                }

                pointList.Add(newPoint);

            }
        }

        private void generateEllipse(bool up)
        {
            double a=1, b=5;

            for (var i = 0; i < count; i++)
            {

                double r = a;

                var valx = getDouble(-r, r);

                //double valy = Math.Sqrt(Math.Pow(r, 2) - Math.Pow(valx, 2));

                double valy = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(valx, 2) * Math.Pow(b, 2)) / a;

                Point newPoint;
                if (up == false)
                {
                    newPoint = new Point { x = valx + basePoint.x, y = -valy + basePoint.y };
                }
                else
                {
                    newPoint = new Point { x = valx + basePoint.x, y = valy + basePoint.y };
                }

                pointList.Add(newPoint);

            }
        }


        //private void generateMoonCloud(bool up)
        //{
        //    for (var i = 0; i < count; i++)
        //    {

        //        double r = maxRadius;

        //        var valx = getDouble(-r, r);
        //        var valy = getDouble(0, r);


        //        double rmic = r;
        //        double x = Math.Sqrt(Math.Pow(r, 2) - Math.Pow(valx, 2));


        //        //while (valy >= x) {

        //        //    valy = getDouble(0, r);
        //        //}

        //        //if (valx >= -rmic && valx <= rmic) {
        //        //    double y = Math.Sqrt(Math.Pow(rmic, 2) - Math.Pow(valx, 2));
        //        //    while (valy <= y) {
        //        //        valy = getDouble(0, r);
        //        //    }
        //        //}
        //        Point newPoint;
        //        if (up == false)
        //        {
        //            newPoint = new Point { x = valx + basePoint.x, y = -valy + basePoint.y };
        //        }
        //        else
        //        {
        //            newPoint = new Point { x = valx + basePoint.x, y = valy + basePoint.y };
        //        }

        //        pointList.Add(newPoint);

        //    }
        //}

        private void generateCircleCloud()
        {
            for (var i = 0; i < count; i++)
            {

                double r = maxRadius;

                var valx = getDouble(-r, r);
                var valy = getDouble(-r, r);


                double rmic = r / 2;
                double x = Math.Sqrt(Math.Pow(r, 2) - Math.Pow(valx, 2));

                while (valy >= x || valy <= -x)
                {

                    valy = getDouble(-1, 1);
                }

                if (valx >= -rmic && valx <= rmic)
                {
                    double y = Math.Sqrt(Math.Pow(rmic, 2) - Math.Pow(valx, 2));
                    while (valy <= y && valy >= -y)
                    {
                        valy = getDouble(-r, r);
                    }
                }

                var newPoint = new Point { x = valx + basePoint.x, y = valy + basePoint.y };

                pointList.Add(newPoint);

            }
        }


        /// <summary>
        /// Method to return the matrix representation of the cloud point
        /// </summary>
        /// <returns></returns>
        public double[,] ReturnCloudDoubleMatrix()
        {
            generateRoundCloud();
            var data = new double[count, 2];
            for (var i = 0; i < count; i++)
            {
                data[i, 0] = pointList[i].x;
                data[i, 1] = pointList[i].y;
            }
            return data;

        }

        public double[,] ReturnCircleDoubleMatrix()
        {
            generateCircleCloud();
            var data = new double[count, 2];
            for (var i = 0; i < count; i++)
            {
                data[i, 0] = pointList[i].x;
                data[i, 1] = pointList[i].y;
            }
            return data;
        }

        public double[,] ReturnMoonDoubleMatrix(bool up)
        {
            generateMoonCloud(up);
            var data = new double[count, 2];
            for (var i = 0; i < count; i++)
            {
                data[i, 0] = pointList[i].x;
                data[i, 1] = pointList[i].y;
            }
            return data;
        }

        public double[,] ReturnEllipseMoonDoubleMatrix(bool up)
        {
            generateEllipse(up);
            var data = new double[count, 2];
            for (var i = 0; i < count; i++)
            {
                data[i, 0] = pointList[i].x;
                data[i, 1] = pointList[i].y;
            }
            return data;
        }


        public double[,] ReturnRawDoubleMatrix()
        {
            var data = new double[count, 2];
            for (var i = 0; i < count; i++)
            {
                data[i, 0] = pointList[i].x;
                data[i, 1] = pointList[i].y;
            }
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
