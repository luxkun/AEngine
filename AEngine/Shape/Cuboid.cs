using OpenTK;

namespace AEngine.Shapes
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

        private void CreateTriangles()
        {
            // face 1
            var a = new Vector3(Min.X, Min.Y, Min.Z);
            var b = new Vector3(Max.X, Min.Y, Min.Z);
            var c = new Vector3(Min.X, Max.Y, Min.Z);
            var d = new Vector3(Max.X, Max.Y, Min.Z);
            TriangleList.Add(new Triangle(this, a, b, d));
            TriangleList.Add(new Triangle(this, a, d, c));

            // lateral 1
            a = new Vector3(Min.X, Min.Y, Min.Z);
            b = new Vector3(Min.X, Min.Y, Max.Z);
            c = new Vector3(Min.X, Max.Y, Min.Z);
            d = new Vector3(Min.X, Max.Y, Max.Z);
            TriangleList.Add(new Triangle(this, a, b, d));
            TriangleList.Add(new Triangle(this, a, d, c));

            // lateral 2
            a = new Vector3(Max.X, Min.Y, Min.Z);
            b = new Vector3(Max.X, Min.Y, Max.Z);
            c = new Vector3(Max.X, Max.Y, Min.Z);
            d = new Vector3(Max.X, Max.Y, Max.Z);
            TriangleList.Add(new Triangle(this, a, b, d));
            TriangleList.Add(new Triangle(this, a, d, c));

            // face 2
            a = new Vector3(Min.X, Min.Y, Max.Z);
            b = new Vector3(Max.X, Min.Y, Max.Z);
            c = new Vector3(Min.X, Max.Y, Max.Z);
            d = new Vector3(Max.X, Max.Y, Max.Z);
            TriangleList.Add(new Triangle(this, a, b, d));
            TriangleList.Add(new Triangle(this, a, d, c));
        }
    }
}