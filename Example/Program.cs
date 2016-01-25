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
                Position = new Vector3(40f, 0f, -20f)
            };
            monkeyMesh.AddHitBox("mass", Vector3.Zero, new Vector3(10f, 10f, 15f));

            var crazyMonkeyMesh = new Mesh((MeshAsset)engine.GetAsset("monkey"))
            {
                Color = new Color4(0f, 0.5f, 1f, 0.9f),
                Scale = new Vector3(15f, 15f, 15f),
                Position = new Vector3(0f, 2f, -16f)
            };
            crazyMonkeyMesh.AddHitBox("mass", Vector3.Zero, new Vector3(10f, 10f, 15f));

            float speed = -10f;
            monkeyMesh.OnUpdate += s =>
            {
                var sender = (Mesh)s;
                if (sender.HasCollisions())
                    Console.WriteLine("collides");
                else
                    Console.WriteLine("no collision");

                sender.Position = new Vector3(sender.Position.X + speed * sender.DeltaTime, sender.Position.Y, sender.Position.Z);
            };

            var inputManager = new GameObject();
            inputManager.OnUpdate += s =>
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
                var m = 20f*sender.DeltaTime;
                camera.Position = new Vector3(
                    camera.Position.X + xM * m, camera.Position.Y + yM * m, camera.Position.Z + zM * m);

                var rotationModX =
                    (sender.Engine.MouseX - sender.Engine.Width / 2f) / sender.Engine.Width / 2f;
                rotationModX = sender.DeltaTime * rotationModX;
                var rotationModY =
                    (sender.Engine.MouseY - sender.Engine.Height / 2f) / sender.Engine.Height / 2f;
                rotationModY = sender.DeltaTime * rotationModY;
                camera.Rotation = new Vector3(
                    camera.Rotation.X + rotationModX, camera.Rotation.Y + rotationModY, camera.Rotation.Z);
            };

            engine.SpawnObject("monkeyMesh", monkeyMesh);
            engine.SpawnObject("crazyMonkeyMesh", crazyMonkeyMesh);
            engine.SpawnObject("inputManager", inputManager);

            engine.Run();
        }
    }
}
