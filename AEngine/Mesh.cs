using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OpenTK;
using OpenTK.Graphics;

namespace AEngine
{
    public class Mesh : GameObject
    {
        public List<Vector3> NormalsList { get; set; }
        public List<Triangle> TriangleList { get; set; }
        public List<Vector2> UvList { get; set; }
        //protected List<Vector3> VertexList { get; private set; }

        public Mesh()
        {
            //VertexList = new List<Vector3>();
            UvList = new List<Vector2>();
            NormalsList = new List<Vector3>();
            TriangleList = new List<Triangle>();
        }

        public Mesh(MeshAsset meshAsset) : this()
        {
            meshAsset.LoadInto(this);
        }

        public Vector3 Rotation { get; set; } = Vector3.Zero;

        public Color4 Color { get; set; }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);
            foreach (var triangle in TriangleList)
            {
                triangle.Scale = Scale;
                triangle.Position = Position;
                triangle.Rotation = Rotation;
                triangle.Color = Color;

                triangle.Draw(camera);
            }
        }
    }
}