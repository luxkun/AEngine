using System;
using OpenTK;

namespace AEngine
{
    public class Cuboid : Mesh
    {
        public readonly float Length;
        public readonly float Depth;
        public readonly float Height;

        public Cuboid(float length, float depth, float height) : base()
        {
            Initialize(length, depth, height);
            this.Length = length;
            this.Depth = depth;
            this.Height = height;
        }

        private void Initialize(float length, float depth, float height)
        {
            // face 1
            var a = Vector3.Zero;
            var b = new Vector3(a.X + length, a.Y, a.Z);
            var c = new Vector3(a.X, a.Y + height, a.Z);
            var d = new Vector3(b.X, c.Y, a.Z);
            TriangleList.Add(new Triangle(this, a, b, d));
            TriangleList.Add(new Triangle(this, a, d, c));

            // lateral 1
            // same a, c
            b = new Vector3(a.X, a.Y, b.Z + depth);
            d = new Vector3(c.X, c.Y, d.Z + depth);
            TriangleList.Add(new Triangle(this, a, b, d));
            TriangleList.Add(new Triangle(this, a, d, c));

            // lateral 2
            a = new Vector3(a.X + length, a.Y, a.Z);
            b = new Vector3(a.X, a.Y + height, a.Z);
            c = new Vector3(a.X, a.Y, a.Z + depth);
            d = new Vector3(c.X, b.Y, c.Z);
            TriangleList.Add(new Triangle(this, a, b, d));
            TriangleList.Add(new Triangle(this, a, d, c));

            // face 2
            a = new Vector3(a.X, a.Y, a.Z + depth);
            b = new Vector3(b.X, b.Y, b.Z + depth);
            c = new Vector3(c.X, c.Y, c.Z + depth);
            d = new Vector3(d.X, d.Y, d.Z + depth);
            TriangleList.Add(new Triangle(this, a, b, d));
            TriangleList.Add(new Triangle(this, a, d, c));
        }
    }
}