﻿using Loyc.Collections;
using System.Collections.Generic;


namespace DissertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    /// <summary>
    /// Note: I did not write this algorithm and do not take credit for it, it was taken fromt the following website:
    /// http://loyc.net/2014/2d-convex-hull-in-cs.html 
    /// very cool algorithm though
    /// </summary>
    public class ConvexHull
    {
        /// <summary>
        /// Computes the convex hull of a polygon, in clockwise order in a Y-up 
        /// coordinate system (counterclockwise in a Y-down coordinate system).
        /// <remarks>Uses the Monotone Chain algorithm, a.k.a. Andrew's Algorithm.</remarks>
        public static IListSource<ConvexHullPoint> ComputeConvexHull(IEnumerable<ConvexHullPoint> ConvexHullPoints)
        {
            var list = new List<ConvexHullPoint>(ConvexHullPoints);
            return ComputeConvexHull(list, true);
        }
        public static IListSource<ConvexHullPoint> ComputeConvexHull(List<ConvexHullPoint> ConvexHullPoints, bool sortInPlace)
        {
            if (!sortInPlace)
                ConvexHullPoints = new List<ConvexHullPoint>(ConvexHullPoints);
            ConvexHullPoints.Sort((a, b) =>
              a.X == b.X ? a.Y.CompareTo(b.Y) : (a.X > b.X ? 1 : -1));

            // Importantly, DList provides O(1) insertion at beginning and end
            DList<ConvexHullPoint> hull = new DList<ConvexHullPoint>();
            int L = 0, U = 0; // size of lower and upper hulls

            // Builds a hull such that the output polygon starts at the leftmost ConvexHullPoint.
            for (int i = ConvexHullPoints.Count - 1; i >= 0; i--)
            {
                ConvexHullPoint p = ConvexHullPoints[i], p1;

                // build lower hull (at end of output list)
                while (L >= 2 && (p1 = hull.Last).Sub(hull[hull.Count - 2]).Cross(p.Sub(p1)) >= 0)
                {
                    hull.RemoveAt(hull.Count - 1);
                    L--;
                }
                hull.PushLast(p);
                L++;

                // build upper hull (at beginning of output list)
                while (U >= 2 && (p1 = hull.First).Sub(hull[1]).Cross(p.Sub(p1)) <= 0)
                {
                    hull.RemoveAt(0);
                    U--;
                }
                if (U != 0) // share the ConvexHullPoint added above, except in the first iteration
                    hull.PushFirst(p);
                U++;
            }
            hull.RemoveAt(hull.Count - 1);

            if (hull.Count != 4)
            {
                // Console.WriteLine("What???");
            }
            return hull;
        }
    }
}
