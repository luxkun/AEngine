using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEngine;
using Aiv.Fast2D;
using OpenTK;
using OpenTK.Graphics;
using Mesh = AEngine.Mesh;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine engine = new Engine("Example", 1280, 768, 60, false, false);
            engine.debugCollisions = true;
            engine.Camera.Position = new Vector3(0f, -15f, -40f);

            Asset.BasePath = "..\\..\\assets";
            engine.LoadAsset("monkey", new MeshAsset("monkey.obj"));

            var monkeyMesh = new Mesh((MeshAsset)engine.GetAsset("monkey"))
            {
                Color = new Color4(1f, 0.5f, 0f, 1f),
                Scale = new Vector3(15f, 15f, 15f),
                Position = new Vector3(0f, 0f, -20f)
            };
            monkeyMesh.AddHitBox("main", 10f, 2f, 15f);

            var inputManager = new GameObject();
            inputManager.OnUpdate += (object s) =>
            {
                var sender = (GameObject) s;
                float xM = 0;
                float yM = 0;
                float zM = 0;
                if (sender.Engine.IsKeyDown(KeyCode.W))
                    yM -= 1f;
                if (sender.Engine.IsKeyDown(KeyCode.S))
                    yM += 1f;
                if (sender.Engine.IsKeyDown(KeyCode.A))
                    xM -= 1f;
                if (sender.Engine.IsKeyDown(KeyCode.D))
                    xM += 1f;
                if (sender.Engine.IsKeyDown(KeyCode.E))
                    zM -= 1f;
                if (sender.Engine.IsKeyDown(KeyCode.F))
                    zM += 1f;
                var camera = sender.Engine.Camera;
                float M = 20f*sender.DeltaTime;
                camera.Position = new Vector3(
                    camera.Position.X + xM * M, camera.Position.Y + yM * M, camera.Position.Z + zM * M);

                float rotationModX =
                    (sender.Engine.MouseX - sender.Engine.Width/2f)/sender.Engine.Width/2f;
                rotationModX = (float) (sender.DeltaTime*rotationModX);
                float rotationModY =
                    (sender.Engine.MouseY - sender.Engine.Height/2f)/sender.Engine.Height/2f;
                rotationModY = (float) (sender.DeltaTime*rotationModY);
                camera.Rotation = new Vector3(
                    camera.Rotation.X + rotationModX, camera.Rotation.Y + rotationModY, camera.Rotation.Z);
            };

            engine.SpawnObject("monkeyMesh", monkeyMesh);
            engine.SpawnObject("inputManager", inputManager);

            engine.Run();
        }
    }
}
