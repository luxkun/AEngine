using OpenTK;

namespace AEngine.Shapes
{
    public class Cuboid : Mesh
    {
        public readonly float Length;
        public readonly float Depth;
        public readonly float Height;

        public Vector3 Max => new Vector3(Position.X + Length, Position.Y + Height, Position.Z + Depth);

        public Cuboid(float length, float depth, float height) : base()
        {
            Initialize(length, depth, height);
            this.Length = length;
            this.Depth = depth;
            this.Height = height;
        }

        public bool CollideWith(Cuboid other)
        {
            //return !(Max.X < other.Position.X || Max.Y < other.Position.Y || Max.Z < other.Position.Z ||
            //         Position.X > other.Max.X || Position.Y > other.Max.Y || Position.Z > other.Max.Z);
            var min1 = Position;
            var min2 = other.Position;
            var max1 = Max;
            var max2 = other.Max;
            return ((min1.X <= min2.X && min2.X <= max1.X) || (min2.X <= min1.X && min1.X <= max2.X)) &&
                   ((min1.Y <= min2.Y && min2.Y <= max1.Y) || (min2.Y <= min1.Y && min1.Y <= max2.Y)) &&
                   ((min1.Z <= min2.Z && min2.Z <= max1.Z) || (min2.Z <= min1.Z && min1.Z <= max2.Z));
            //         if (
            //((min_x1 <= min_x2 && min_x2 <= max_x1) || (min_x2 <= min_x1 && min_x1 <= max_x2)) &&
            //((min_y1 <= min_y2 && min_y2 <= max_y1) || (min_y2 <= min_y1 && min_y1 <= max_y2)) &&
            //((min_z1 <= min_z2 && min_z2 <= max_z1) || (min_z2 <= min_z1 && min_z1 <= max_z2))
            //)
        }

        private void Initialize(float length, float depth, float height)
        {
            var A = Vector3.Zero;
            // face 1
            var a = new Vector3(A.X, A.Y, A.Z);
            var b = new Vector3(a.X + length, a.Y, a.Z);
            var c = new Vector3(a.X, a.Y + height, a.Z);
            var d = new Vector3(b.X, c.Y, a.Z);
            TriangleList.Add(new Triangle(this, a, b, d));
            TriangleList.Add(new Triangle(this, a, d, c));

            // lateral 1
            // same a, c
            b = new Vector3(A.X, A.Y, A.Z + depth);
            d = new Vector3(A.X, c.Y, b.Z);
            TriangleList.Add(new Triangle(this, a, b, d));
            TriangleList.Add(new Triangle(this, a, d, c));

            // lateral 2
            a = new Vector3(A.X + length, A.Y, A.Z);
            b = new Vector3(A.X, A.Y, A.Z + depth);
            c = new Vector3(A.X, A.Y + height, A.Z);
            d = new Vector3(A.X, c.Y, b.Z);
            TriangleList.Add(new Triangle(this, a, b, d));
            TriangleList.Add(new Triangle(this, a, d, c));

            // face 2
            a = new Vector3(A.X, A.Y, A.Z + depth);
            b = new Vector3(a.X + length, a.Y, a.Z);
            c = new Vector3(a.X, a.Y + height, a.Z);
            d = new Vector3(b.X, c.Y, a.Z);
            TriangleList.Add(new Triangle(this, a, b, d));
            TriangleList.Add(new Triangle(this, a, d, c));
        }
    }
}