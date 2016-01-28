using System;
using System.Drawing;
using Aiv.Fast2D;
using System.Numerics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace AEngine
{
    public class Triangle
    {
        public Mesh Owner { get; set; }

        private readonly Vertex3 va;
        private readonly Vertex3 vb;
        private readonly Vertex3 vc;

        public Triangle(Mesh owner, Vertex3 va, Vertex3 vb, Vertex3 vc)
        {
            Owner = owner;
            this.va = va;
            this.vb = vb;
            this.vc = vc;
        }

        public Vector3 Scale { get; set; } = new Vector3(1f, 1f, 1f);
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Rotation { get; set; } = Vector3.Zero;

        public Color4 Color { get; set; }

        private Color4 TextureMap(Vector2 uv)
        {
            int x = (int)Math.Abs(uv.X * Owner.Texture.Width) % Owner.Texture.Width;
            int y = (int)Math.Abs(uv.Y * Owner.Texture.Height) % Owner.Texture.Height;
            int pos = (int) ((y*Owner.Texture.Width*4) + (x* 4));
            return new Color4(
                Owner.Texture.Bitmap[pos] * 0.00392156862f,
                Owner.Texture.Bitmap[pos + 1] * 0.00392156862f,
                Owner.Texture.Bitmap[pos + 2] * 0.00392156862f,
                Owner.Texture.Bitmap[pos + 3] * 0.00392156862f);
        }

        public void Draw(Camera camera)
        {
            var point1 = ((va.Position * Scale).Rotate(Rotation) + Position + camera.Position).Rotate(camera.Rotation).Project(Owner.Engine, camera).FromNdc(Owner.Engine);
            var point2 = ((vb.Position * Scale).Rotate(Rotation) + Position + camera.Position).Rotate(camera.Rotation).Project(Owner.Engine, camera).FromNdc(Owner.Engine);
            var point3 = ((vc.Position * Scale).Rotate(Rotation) + Position + camera.Position).Rotate(camera.Rotation).Project(Owner.Engine, camera).FromNdc(Owner.Engine);
            var vertex1 = new Vertex3(point1, va.Uv);
            var vertex2 = new Vertex3(point2, vb.Uv);
            var vertex3 = new Vertex3(point3, vc.Uv);

            DrawTriangleScanLines(vertex1, vertex2, vertex3);
        }

        private void DrawScanline(int y, Vertex3 left1, Vertex3 left2, Vertex3 right1, Vertex3 right2)
        {
            float deltaL = 1f;
            float deltaR = 1f;
            if (left1.Y != left2.Y)
            {
                deltaL = (y - left1.Y)/(left2.Y - left1.Y);
            }
            if (right1.Y != right2.Y)
            {
                deltaR = (y - right1.Y) / (right2.Y - right1.Y);
            }

            int left = (int) left1.X.Lerp(left2.X, deltaL);
            int right = (int) right1.X.Lerp(right2.X, deltaR);
            float zLeft = left1.Z.Lerp(left2.Z, deltaL);
            float zRight = right1.Z.Lerp(right2.Z, deltaR);
            Vector2 uStart = Vector2.One;
            Vector2 uEnd = Vector2.One;
            if (left1.Uv != Vector2.Zero) { 
                uStart = left1.Uv.Lerp(left2.Uv, deltaL);
                uEnd = right1.Uv.Lerp(right2.Uv, deltaR);
            }

            for (int x = left > 0 ? left : 0; x < right; x++)
            {
                float deltaZ = ((float)x - left)/((float)right - left);
                float z = zLeft.Lerp(zRight, deltaZ);

                Color4 color;
                if (left1.Uv != Vector2.Zero)
                {
                    // texture
                    var uV = uStart.Lerp(uEnd, deltaZ);
                    uV.Y = 1 - uV.Y;
                    color = TextureMap(uV);
                }
                else
                {
                    color = Color;
                }

                DrawHelper.PutPixel(Owner.Engine.WorkingTexture, x, y, z, color);
            }
        }

        private void DrawTriangleScanLines(Vertex3 point1, Vertex3 point2, Vertex3 point3)
        {
            var p1 = point1;
            var p2 = point2;
            var p3 = point3;
            if (p1.Y > p2.Y)
            {
                var t = p1;
                p1 = p2;
                p2 = t;
            }
            if (p2.Y > p3.Y)
            {
                var t = p2;
                p2 = p3;
                p3 = t;
            }
            if (p1.Y > p2.Y)
            {
                var t = p1;
                p1 = p2;
                p2 = t;
            }

            // inversed slope = dX / dY
            float slopeP1P2 = (p2.X - p1.X) / (p2.Y - p1.Y);
            float slopeP1P3 = (p3.X - p1.X) / (p3.Y - p1.Y);
            if (slopeP1P3 > slopeP1P2) // p2 left
            {
                for (int y = (int) p1.Y; y <= (int) p3.Y; y++)
                {
                    if (y < p2.Y) // first part
                    {
                        DrawScanline(y, p1, p2, p1, p3);
                    } else // second part
                    {
                        DrawScanline(y, p2, p3, p1, p3);
                    }
                }
            }
            else // p2 right
            {
                for (int y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    if (y < p2.Y) // first part
                    {
                        DrawScanline(y, p1, p3, p1, p2);
                    }
                    else // second part
                    {
                        DrawScanline(y, p1, p3, p2, p3);
                    }
                }
            }

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