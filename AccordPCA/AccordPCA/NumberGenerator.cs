using System;
using System.IO;

namespace AccordPCA {
    class NumberGenerator {
        private struct Point {
            public double x, y;
        }

        public NumberGenerator()
        {
            random = new Random();
        }


        public void WriteTofile(string name, int rows, int columns, double x1, double y1, double radius1, double x2, double y2, double radius2)
        {
            StreamWriter sw = new StreamWriter(name);
            sw.WriteLine(rows * 2 + " " + columns);
            for (int i = 0; i < rows; i++) {
                double angle = getDouble(0, 1);
                double radius = getDouble(0, 2 * Math.PI);
                //double valx = angle * Math.Cos(radius) * radius1;
                //double valy = angle * Math.Sin(radius) * radius1;

                double valx = Math.Sqrt(angle) * Math.Cos(radius) * radius1;
                double valy = Math.Sqrt(angle) * Math.Sin(radius) * radius1;

                double x = x1 + valx;
                double y = y1 + valy;

                sw.WriteLine(x + " " + y);

            }

            for (int i = 0; i < rows; i++) {
                double angle = getDouble(0, 1);
                double radius = getDouble(0, 2 * Math.PI);

                //double valx = angle * Math.Cos(radius) * radius2;
                //double valy = angle * Math.Sin(radius) * radius2;

                double valx = Math.Sqrt(angle) * Math.Cos(radius) * radius2;
                double valy = Math.Sqrt(angle) * Math.Sin(radius) * radius2;

                double x = x2 + valx;
                double y = y2 + valy;

                sw.WriteLine(x + " " + y);

            }
            sw.Close();
        }



        private Point getNextRandomPoint(Point basePoint, double radius)
        {
            Point newPoint;
            double x = getDouble(0, radius);
            double y = getDouble(0, radius);
            //if (random.Next(0, 2) == 0)
            //    x = -x;
            //if (random.Next(0, 2) == 0)
            //    y = -y;
            newPoint.x = Math.Round(basePoint.x + x, 5);
            newPoint.y = Math.Round(basePoint.y + y, 5);
            return newPoint;
            
        }

        private double getDouble(double min, double max)
        {

            return min + (random.NextDouble() * (max - min));
        }

        private Random random;
    }
}
