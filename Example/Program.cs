using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEngine;
using OpenTK;
using OpenTK.Graphics;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine engine = new Engine("Example", 1280, 768, 60, false, false);
            engine.Camera.Position = new Vector3(0f, -15f, -40f);

            Asset.BasePath = "..\\..\\assets";
            engine.LoadAsset("monkey", new MeshAsset("monkey.obj"));

            var monkeyMesh = new Mesh((MeshAsset) engine.GetAsset("monkey"))
            {
                Color = new Color4(0f, 1f, 0f, 1f),
                Scale = new Vector3(15f, 15f, 15f),
                Position = new Vector3(0f, 0f, -20f)
            };

            engine.SpawnObject("monkeyMesh", monkeyMesh);

            engine.Run();
        }
    }
}
