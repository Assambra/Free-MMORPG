using System;
using UnityEngine;

namespace Den.Tools
{
    /* 
    Fast Ray-Box Intersection
    by Andrew Woo
    from "Graphics Gems", Academic Press, 1990
    Ported to C#
    */


    static public partial class Collisions
    {
        public static bool RayhitBoundingBox(Ray ray, Vector3 minB, Vector3 maxB, out Vector3 coord)
        {
            Vector3 origin = ray.origin;
            Vector3 dir = ray.direction.normalized;
            coord = new Vector3();

            const int NUMDIR = 3; //number of dimentions

            const int RIGHT = 0;
            const int LEFT = 1;
            const int MIDDLE = 2;

            bool inside = true;
            Vector3Int quadrant = new Vector3Int();
            int whichPlane;
            Vector3 maxT = new Vector3();
            Vector3 candidatePlane = new Vector3();

            // Find candidate planes; this loop can be avoided if
            // rays cast all from the eye (assume perspective view)
            for (int i = 0; i < NUMDIR; i++)
            {
                if (origin[i] < minB[i])
                {
                    quadrant[i] = LEFT;
                    candidatePlane[i] = minB[i];
                    inside = false;
                }
                else if (origin[i] > maxB[i])
                {
                    quadrant[i] = RIGHT;
                    candidatePlane[i] = maxB[i];
                    inside = false;
                }
                else
                {
                    quadrant[i] = MIDDLE;
                }
            }

            // Ray origin inside bounding box
            if (inside)
            {
                coord = origin;
                return true;
            }

            // Calculate T distances to candidate planes
            for (int i = 0; i < NUMDIR; i++)
            {
                if (quadrant[i] != MIDDLE && dir[i] != 0.0)
                {
                    maxT[i] = (candidatePlane[i] - origin[i]) / dir[i];
                }
                else
                {
                    maxT[i] = -1;
                }
            }

            // Get the largest of the maxT's for the final choice of intersection
            whichPlane = 0;
            for (int i = 1; i < NUMDIR; i++)
            {
                if (maxT[whichPlane] < maxT[i])
                {
                    whichPlane = i;
                }
            }

            // Check if the final candidate is actually inside the box
            if (maxT[whichPlane] < 0.0)
            {
                return false;
            }

            for (int i = 0; i < NUMDIR; i++)
            {
                if (whichPlane != i)
                {
                    coord[i] = origin[i] + maxT[whichPlane] * dir[i];
                    if (coord[i] < minB[i] || coord[i] > maxB[i])
                    {
                        return false;
                    }
                }
                else
                {
                    coord[i] = candidatePlane[i];
                }
            }

            return true; // Ray hits the box
        }

        /*static void Main()
        {
            // Example usage:
            double[] minB = { 0.0, 0.0, 0.0 };
            double[] maxB = { 1.0, 1.0, 1.0 };
            double[] origin = { 0.2, 0.2, 0.2 };
            double[] dir = { 1.0, 0.0, 0.0 };
            double[] coord = new double[3];

            bool hit = RayhitBoundingBox(minB, maxB, origin, dir, coord);

            if (hit)
            {
                Console.WriteLine("Ray hits the box. Intersection point: [{0}, {1}, {2}]", coord[0], coord[1], coord[2]);
            }
            else
            {
                Console.WriteLine("Ray does not hit the box.");
            }
        }*/
    }
}