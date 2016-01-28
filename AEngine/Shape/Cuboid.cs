using System;
using System.Numerics;

namespace AEngine.Shape
{
    public class Cuboid : Mesh
    {
        //public Vector3 Max => new Vector3(Position.X + Length, Position.Y + Height, Position.Z + Depth);

        public Cuboid(Vector3 min, Vector3 max) : base()
        {
            Min = min;
            Max = max;
            CreateTriangles();
        }

        public Vector3 Max { get; }

        public Vector3 Min { get; }

        public bool CollideWith(Cuboid other)
        {
            //return !(Max.X < other.Position.X || Max.Y < other.Position.Y || Max.Z < other.Position.Z ||
            //         Position.X > other.Max.X || Position.Y > other.Max.Y || Position.Z > other.Max.Z);
            var min1 = Min + Position;
            var min2 = other.Min + other.Position;
            var max1 = Max + Position;
            var max2 = other.Max + other.Position;
            return ((min1.X <= min2.X && min2.X <= max1.X) || (min2.X <= min1.X && min1.X <= max2.X)) &&
                   ((min1.Y <= min2.Y && min2.Y <= max1.Y) || (min2.Y <= min1.Y && min1.Y <= max2.Y)) &&
                   ((min1.Z <= min2.Z && min2.Z <= max1.Z) || (min2.Z <= min1.Z && min1.Z <= max2.Z));
            //         if (
            //((min_x1 <= min_x2 && min_x2 <= max_x1) || (min_x2 <= min_x1 && min_x1 <= max_x2)) &&
            //((min_y1 <= min_y2 && min_y2 <= max_y1) || (min_y2 <= min_y1 && min_y1 <= max_y2)) &&
            //((min_z1 <= min_z2 && min_z2 <= max_z1) || (min_z2 <= min_z1 && min_z1 <= max_z2))
            //)
        }

        // from: http://www.opentk.com/node/869
        public Vector2 MinimumTranslation2D(Cuboid other)
        {
            Vector3 amin = this.Min;
            Vector3 amax = this.Max;
            Vector3 bmin = other.Min;
            Vector3 bmax = other.Max;

            Vector2 mtd = new Vector2();

            float left = (bmin.X - amax.X);
            float right = (bmax.X - amin.X);
            float top = (bmin.Y - amax.Y);
            float bottom = (bmax.Y - amin.Y);

            // box dont intersect   
            if (left > 0 || right < 0) return Vector2.Zero;
            if (top > 0 || bottom < 0) return Vector2.Zero;

            // box intersect. work out the mtd on both x and y axes.
            if (Math.Abs(left) < right)
                mtd.X = left;
            else
                mtd.X = right;

            if (Math.Abs(top) < bottom)
                mtd.Y = top;
            else
                mtd.Y = bottom;

            // 0 the axis with the largest mtd value.
            if (Math.Abs(mtd.X) < Math.Abs(mtd.Y))
                mtd.Y = 0;
            else
                mtd.X = 0;
            return mtd;
        }

        private void CreateTriangles()
        {
            // face 1
            var a = new Vertex3(new Vector3(Min.X, Min.Y, Min.Z));
            var b = new Vertex3(new Vector3(Max.X, Min.Y, Min.Z));
            var c = new Vertex3(new Vector3(Min.X, Max.Y, Min.Z));
            var d = new Vertex3(new Vector3(Max.X, Max.Y, Min.Z));
            TriangleList.Add(new Triangle(this, a, b, d));
            TriangleList.Add(new Triangle(this, a, d, c));

            // lateral 1
            a = new Vertex3(new Vector3(Min.X, Min.Y, Min.Z));
            b = new Vertex3(new Vector3(Min.X, Min.Y, Max.Z));
            c = new Vertex3(new Vector3(Min.X, Max.Y, Min.Z));
            d = new Vertex3(new Vector3(Min.X, Max.Y, Max.Z));
            TriangleList.Add(new Triangle(this, a, b, d));
            TriangleList.Add(new Triangle(this, a, d, c));

            // lateral 2
            a = new Vertex3(new Vector3(Max.X, Min.Y, Min.Z));
            b = new Vertex3(new Vector3(Max.X, Min.Y, Max.Z));
            c = new Vertex3(new Vector3(Max.X, Max.Y, Min.Z));
            d = new Vertex3(new Vector3(Max.X, Max.Y, Max.Z));
            TriangleList.Add(new Triangle(this, a, b, d));
            TriangleList.Add(new Triangle(this, a, d, c));

            // face 2
            a = new Vertex3(new Vector3(Min.X, Min.Y, Max.Z));
            b = new Vertex3(new Vector3(Max.X, Min.Y, Max.Z));
            c = new Vertex3(new Vector3(Min.X, Max.Y, Max.Z));
            d = new Vertex3(new Vector3(Max.X, Max.Y, Max.Z));
            TriangleList.Add(new Triangle(this, a, b, d));
            TriangleList.Add(new Triangle(this, a, d, c));
        }
    }
}