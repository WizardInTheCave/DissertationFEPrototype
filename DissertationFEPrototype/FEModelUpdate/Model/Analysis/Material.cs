﻿using DissertationFEPrototype.Model.Analysis.MaterialProps;

namespace DissertationFEPrototype.Model.Analysis
{
    public class Material
    {
        int id;
        string name;
        Geometric geometric;
        Mechanical mechanical;

        public int GetId{ get{ return this.id; } }
        public string GetName{ get{ return this.name; } }

        public Geometric Geometric{ get {return this.geometric; } }
        public Mechanical Mechanical { get { return this.mechanical; } }

        public Material(int id, string name, Geometric geometric, Mechanical mechanical)
        {
            this.id = id;
            this.name = name;
            this.geometric = geometric;
            this.mechanical = mechanical;
        }
    }
}
