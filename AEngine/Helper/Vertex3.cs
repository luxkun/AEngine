using System.Numerics;

namespace AEngine
{
    public struct Vertex3
    {
        public Vector3 Position { get; set; }
        public Vector2 Uv { get; set; }

        public float X => Position.X;
        public float Y => Position.Y;
        public float Z => Position.Z;
        public float U => Uv.X;
        public float V => Uv.Y;

        public Vertex3(Vector3 position)
        {
            Position = position;
            Uv = Vector2.Zero;
        }
        public Vertex3(Vector3 position, Vector2 uv) : this(position)
        {
            Uv = uv;
        }

        public void FromNdc(Engine engine)
        {
            Position = Position.FromNdc(engine);
        }
    }
}