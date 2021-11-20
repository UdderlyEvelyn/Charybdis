using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Charybdis.Library.Core
{
    [Serializable]
    public class Box : ISerializable
    {
        public Vec3 Center = new Vec3(.5f, .5f, .5f);
        //Distance from the center to any corner
        public Vec3 Extent = new Vec3(.5f, .5f, .5f);

        public Vec3 Min = new Vec3(0, 0, 0);
        public Vec3 Max = new Vec3(1, 1, 1);

        public float Radius
        {
            get
            {
                return Extent.Average();
            }
        }

        public Box(Vec3 center, Vec3 extent)
        {
            Center = center;
            Extent = extent;
            Min = center - extent;
            Max = center + extent;
        }

        public Box(float cx, float cy, float cz, float ex, float ey, float ez)
        {
            Center = new Vec3(cx, cy, cz);
            Extent = new Vec3(ex, ey, ez);
            Min = new Vec3(cx - ex, cy - ey, cz - ez);
            Max = new Vec3(cx + ex, cy + ey, cz + ez);
        }

        protected Box(SerializationInfo info, StreamingContext ctxt)
        {
            Center = (Vec3)info.GetValue("Center", typeof(Vec3));
            Extent = (Vec3)info.GetValue("Extent", typeof(Vec3));
            Min = Center - Extent;
            Max = Center + Extent;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Center", Center);
            info.AddValue("Extent", Extent);
        }

        public static Box FromMinMax(Vec3 min, Vec3 max)
        {
            //Can remove the division and compare to a multiplied result when culling, but complicates other usages and keeping track of which was entered which way.
            Vec3 center = (min + max) / 2;
            Vec3 extent = (max - min) / 2;
            return new Box(center, extent) { Min = min, Max = max };
        }

        public bool Contains(Vec3 v)
        {
            return ((v.X <= Max.X && v.X >= Min.X) && (v.Y <= Max.Y && v.Y >= Min.Y) && (v.Z <= Max.Z && v.Z >= Min.Z));
        }
    }
}
