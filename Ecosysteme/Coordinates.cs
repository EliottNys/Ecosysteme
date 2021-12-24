using System;
using System.Collections.Generic;
using System.Linq;

namespace Ecosysteme
{
    public static class Coordinates    //allows to handle coordinates, probabilities ...
    {
        //METHODS
        public static int Distance(int[] firstCoordinates, int[] secondCoordinates)     //calculates the distance between two points (rounded up)
        {
            return (int)Math.Ceiling(Math.Sqrt(Math.Pow(firstCoordinates[0] - secondCoordinates[0], 2) + Math.Pow(firstCoordinates[1] - secondCoordinates[1], 2)));
            //N.B.: I use Math.Ceiling to always round UP
        }
        public static int[] CloseBy(int[] coordinates, int radius, Random random)   //returns coordinates that are within the radius of the original coordinates
        {
            while (true)
            {
                int[] shift = new[] { random.Next(-radius, radius), random.Next(-radius, radius) };
                int[] newPoint = coordinates.Zip(shift, (x, y) => x + y).ToArray();
                if (Coordinates.Distance(coordinates, newPoint) <= radius)   //if the horizontal and vertical shift are both smaller than the radius, the point could still be outside the radius
                {
                    return newPoint;
                }
            }
        }
        public static int[] Direction(int[] departure, int[] destination)   //gives the direction you need to take to go from departure-point to destination-point
        {
            int[] vector = new int[] { destination[0] - departure[0], destination[1] - departure[1] };
            if (vector[0] == 0)
            {
                if (vector[1] > 0) { return new int[] { 0, 1 }; }
                else { return new int[] { 0, -1 }; }
            }
            if (vector[1] == 0)
            {
                if (vector[0] > 0) { return new int[] { 1, 0 }; }
                else { return new int[] { -1, 0 }; }
            }
            int sinus = (int)Math.Round((double)(1000 * (vector[1] / vector[0])));
            if (vector[0] > 0 && vector[1] > 0) //1st quadrant
            {
                if (sinus > 924) { return new int[] { 0, 1 }; } //N
                else if (sinus < 383) { return new int[] { 1, 0 }; }    //E
                else { return new int[] { 1, 1 }; } //NE
            }
            else if (vector[0] > 0 && vector[1] < 0) //2nd quadrant
            {
                if (sinus > -383) { return new int[] { 1, 0 }; }    //E
                else if (sinus < -924) { return new int[] { 0, -1 }; }  //S
                else { return new int[] { 1, -1 }; }    //SE
            }
            else if (vector[0] < 0 && vector[1] < 0) //3rd quadrant
            {
                if (sinus < -924) { return new int[] { 0, -1 }; }  //S
                else if (sinus > -383) { return new int[] { -1, 0 }; } //W
                else { return new int[] { -1, -1 }; }   //SW
            }
            else  //4th quadrant
            {
                if (sinus < 383) { return new int[] { -1, 0 }; }    //W
                else if (sinus > 924) { return new int[] { 0, 1 }; }    //N
                else { return new int[] { -1, 1 }; }    //NW
            }
        }
        public static List<int[]> Area(int[] coordinates, int radius)   //returns all coordinates within range
        {
            List<int[]> answer = new List<int[]>();
            for (int i = coordinates[0] - radius; i <= coordinates[0] + radius; i++)
            {
                for (int j = coordinates[1] - radius; j <= coordinates[1] + radius; j++)
                {
                    if (Coordinates.Distance(new int[] { i, j }, coordinates) <= radius) { answer.Add(new int[] { i, j }); }    //square => (pseudo-)circle
                }
            }
            return answer;
        }
    }
}