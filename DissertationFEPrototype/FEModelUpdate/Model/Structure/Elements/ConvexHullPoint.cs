﻿using DissertationFEPrototype.FEModelUpdate.Model.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DissertationFEPrototype.FEModelUpdate.Model.Structure.Elements
{
    public class ConvexHullPoint
    {
        public double X;
        public double Y;
        public Node node3d;

        public ConvexHullPoint(Node node3d, double X, double Y)
        {
            this.X = X;
            this.Y = Y;
            this.node3d = node3d;
        }
        /// <summary>
        /// Do Cross product with a ConvexHullPoint
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public double Cross(ConvexHullPoint b)
        {
            return this.X * b.Y - this.Y * b.X;
        }
        /// <summary>
        /// Subtract ConvexHullPoint A from B
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public ConvexHullPoint Sub(ConvexHullPoint b)
        {
            return new ConvexHullPoint(null, X - b.X, Y - b.Y);
        }

        /// <summary>
        /// Compute the Distance from ConvexHull point A to B
        /// </summary>
        /// <param name="b">The second point</param>
        /// <returns>a distance in euclidean space as a double</returns>
        public double distanceTo(ConvexHullPoint b)
        {
            var delX = Math.Abs(this.X - b.X);
            var delY = Math.Abs(this.Y - b.Y);

            return Math.Sqrt(Math.Pow(delX, 2.0) + Math.Pow(delY, 2));
        }
    }
}
