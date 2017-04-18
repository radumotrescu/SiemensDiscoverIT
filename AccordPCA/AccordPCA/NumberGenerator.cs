using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AccordPCA
{
    class NumberGenerator
    {
        private struct Point
        {
            public double x, y;
        }

        public NumberGenerator()
        {
            random = new Random();
        }

        public void WriteToFile(string name, int rows, int columns, double x1, double y1, double radius1, double x2, double y2, double radius2)
        {
            StreamWriter sw = new StreamWriter(name);
            sw.WriteLine(rows*2 + " " + columns);
            for (int i = 0; i < rows; i++)
            {
                Point basePoint;
                basePoint.x = x1;
                basePoint.y = y1;
                Point newPoint = getNextRandomPoint(basePoint, radius1);
                sw.WriteLine(newPoint.x + " " + newPoint.y);

            }
            for (int i = 0; i < rows; i++)
            {
                Point basePoint;
                basePoint.x = x2;
                basePoint.y = y2;
                Point newPoint = getNextRandomPoint(basePoint, radius2);
                sw.WriteLine(newPoint.x + " " + newPoint.y);

            }

            sw.Close();
        }

        private Point getNextRandomPoint(Point basePoint, double radius)
        {
            Point newPoint;
            double x = getDouble(0, radius);
            double y = getDouble(0, radius);
            if (random.Next(0, 2) == 0)
                x = -x;
            if (random.Next(0, 2) == 0)
                y = -y;
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
