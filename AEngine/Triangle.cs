using System;
using System.Drawing;
using Aiv.Fast2D;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace AEngine
{
    public class Triangle
    {
        public Mesh Owner { get; set; }
        private readonly Vector3 va;
        private readonly Vector3 vb;
        private readonly Vector3 vc;
        private Sprite spriteA;
        private Sprite spriteB;
        private Sprite spriteC;
        private Texture texture;
        private Color4 color;

        public Triangle(Mesh owner, Vector3 va, Vector3 vb, Vector3 vc)
        {
            this.Owner = owner;
            this.va = va;
            this.vb = vb;
            this.vc = vc;
            this.texture = new Texture(1, 1);
            // a -> b
            this.spriteA = new Sprite(1, 1);
            // b -> c
            this.spriteB = new Sprite(1, 1);
            // c - > a
            this.spriteC = new Sprite(1, 1);
        }

        public Vector3 Scale { get; set; } = new Vector3(1f, 1f, 1f);
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Rotation { get; set; } = Vector3.Zero;

        public Color4 Color
        {
            get { return color; }
            set
            {
                if (value != color) { 
                    texture.Bitmap[0] = (byte)(value.R * 255);
                    texture.Bitmap[1] = (byte)(value.G * 255);
                    texture.Bitmap[2] = (byte)(value.B * 255);
                    texture.Bitmap[3] = (byte)(value.A * 255);
                    texture.Update();
                }
                color = value;
            }
        }

        public void Draw(Camera camera)
        {
            var point1 = ((va * Scale).Rotate(Rotation) + Position + camera.Position)
                .Rotate(camera.Rotation).Project(Owner.Engine, camera.Fov).FromNdc(Owner.Engine);
            var point2 = ((vb * Scale).Rotate(Rotation) + Position + camera.Position)
                .Rotate(camera.Rotation).Project(Owner.Engine, camera.Fov).FromNdc(Owner.Engine);
            var point3 = ((vc * Scale).Rotate(Rotation) + Position + camera.Position)
                .Rotate(camera.Rotation).Project(Owner.Engine, camera.Fov).FromNdc(Owner.Engine);

            //Console.WriteLine(point1 + " " + point2 + " " + point3);

            DrawHelper.DrawLine(Owner.Engine.WorkingTexture, point1, point2, Color);
            DrawHelper.DrawLine(Owner.Engine.WorkingTexture, point2, point3, Color);
            DrawHelper.DrawLine(Owner.Engine.WorkingTexture, point1, point3, Color);

            //// A
            //// a -> b
            //spriteA.position = point1;
            //float deltaX = point2.X - point1.X;
            //float deltaY = point2.Y - point1.Y;
            //spriteA.scale = new Vector2(1f, Math.Abs(deltaX) + Math.Abs(deltaY));
            //spriteA.Rotation = (float) (Math.Atan2(deltaY, deltaX) * 180f / Math.PI);
            //spriteA.DrawTexture(texture);

            //// B
            //// b -> c
            //spriteB.position = point2;
            //deltaX = point3.X - point2.X;
            //deltaY = point3.Y - point2.Y;
            //spriteB.scale = new Vector2(1f, Math.Abs(deltaX) + Math.Abs(deltaY));
            //spriteB.Rotation = (float) (Math.Atan2(deltaY, deltaX) * 180f / Math.PI);
            //spriteB.DrawTexture(texture);

            //// C
            //// c - > a
            //spriteC.position = point1;
            //deltaX = point3.X - point1.X;
            //deltaY = point3.Y - point1.Y;
            //spriteC.scale = new Vector2(1f, Math.Abs(deltaX) + Math.Abs(deltaY));
            //spriteC.Rotation = (float) (Math.Atan2(deltaY, deltaX) * 180f / Math.PI);
            //spriteC.DrawTexture(texture);
        }

        public Triangle Clone()
        {
            return new Triangle(Owner, va, vb, vc)
            {
                Scale = Scale,
                Position = Position,
                Rotation = Rotation,
                Color = Color
            };
        }
    }
}