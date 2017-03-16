﻿using DisertationFEPrototype.FEModelUpdate.Model.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisertationFEPrototype.FEModelUpdate.Model.Structure.Elements
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

        public double Cross(ConvexHullPoint b)
        {
            return this.X * b.Y - this.Y * b.X;
        }
        public ConvexHullPoint Sub(ConvexHullPoint b)
        {
            return new ConvexHullPoint(null, X - b.X, Y - b.Y);
        }
    }
}
