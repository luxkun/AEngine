using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using OpenTK;
using OpenTK.Graphics;

namespace AEngine
{
    // replace everything with sprite rotations etc.
    public static class DrawHelper
    {
        private static Dictionary<Color4, byte[]> cachedBackground;
        // black faster

        private static Dictionary<Color4, byte[]> CachedBackground
        {
            get
            {
                if (cachedBackground == null)
                    cachedBackground = new Dictionary<Color4, byte[]>();
                return cachedBackground;
            }
            set { cachedBackground = value; }
        }

        public static void DrawRectangleOnTexture(Texture texture, int x, int y, int width, int height, Color4 color)
        {
            if (x + width < 0 || y + height < 0 || x > texture.Width || y > texture.Height)
                return;
            for (var by = 0; by < height; by++)
            {
                for (var bx = 0; bx < width; bx++)
                {
                    PutPixel(texture, bx + x, by + y, color);
                }
            }
        }

        public static void PutPixel(Texture texture, int x, int y, Color4 color)
        {
            if (x < 0 || y < 0 || x >= texture.Width || y >= texture.Height)
                return;
            var position = y * texture.Width * 4 + x * 4;
            if (position + 3 < texture.Bitmap.Length && position >= 0)
            {
                texture.Bitmap[position] = (byte)(color.R * 255);
                texture.Bitmap[position + 1] = (byte)(color.G * 255);
                texture.Bitmap[position + 2] = (byte)(color.B * 255);
                texture.Bitmap[position + 3] = (byte)(color.A * 255);
            }
        }

        public static void DrawLine(Texture texture, Vector2 from, Vector2 to, Color4 color)
        {
            var near = 300f;
            // not entirely correct but quite fast
            if (from.X < -near || to.X < -near || from.Y < -near || to.X < -near ||
                from.X > texture.Width + near || to.X > texture.Width + near ||
                from.Y > texture.Height + near || to.Y > texture.Height + near)
                return;
            var x = (int)from.X;
            var y = (int)from.Y;
            var x2 = (int)to.X;
            var y2 = (int)to.Y;
            var w = x2 - x;
            var h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1;
            else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1;
            else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1;
            else if (w > 0) dx2 = 1;
            var longest = Math.Abs(w);
            var shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1;
                else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            var numerator = longest >> 1;
            for (var i = 0; i <= longest && (x < texture.Width || y < texture.Height); i++)
            {
                PutPixel(texture, x, y, color);
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }

        public static void ChangeTextureColor(Texture texture, Color4 color)
        {
            //DrawRectangleOnTexture(texture, 0, 0, Manager.Texture.Width, Manager.Texture.Height, color);
            if (color.R != 0 || color.G != 0 || color.B != 0 || color.A != 1f)
            {
                if (CachedBackground.ContainsKey(color))
                    texture.Bitmap = (byte[])CachedBackground[color].Clone();
                else
                {
                    DrawRectangleOnTexture(texture, 0, 0, texture.Width, texture.Height, color);
                    CachedBackground[color] = (byte[])texture.Bitmap.Clone();
                }
            }
            else
            {
                texture.Bitmap = new byte[texture.Bitmap.Length];
            }
        }
    }
}
