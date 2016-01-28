using System;
using Aiv.Fast2D;
using System.Numerics;

namespace AEngine
{
    public static class VectorExtend
    {
        public static float Lerp(this float min, float max, float delta)
        {
            return min + (max - min)*delta;
        }
        public static Vector2 Lerp(this Vector2 min, Vector2 max, float delta)
        {
            return new Vector2(min.X + (max.X - min.X) * delta, min.Y + (max.Y - min.Y) * delta);
        }

        public static Vector3 Lerp(this Vector3 min, Vector3 max, float delta)
        {
            return new Vector3(
                min.X + (max.X - min.X) * delta,
                min.Y + (max.Y - min.Y) * delta,
                min.Z + (max.Z - min.Z) * delta);
        }

        //LERP min + (max - min) * delta
        public static Vector3 ToNdc(this Vector3 v, Engine engine)
        {
            return new Vector3(v.X / engine.Width * 2 - 1, v.Y / engine.Height * 2 - 1, v.Z);
        }

        public static Vector3 FromNdc(this Vector3 v, Engine engine)
        {
            return new Vector3((v.X + 1) / 2 * engine.Width, (v.Y + 1) / 2 * engine.Height, v.Z);
        }

        public static Vector3 RotateY(this Vector3 v, float radian)
        {
            return new Vector3(
                (float)(Math.Cos(radian) * v.X + Math.Sin(radian) * v.Z),
                v.Y,
                (float)(-Math.Sin(radian) * v.X + Math.Cos(radian) * v.Z)
                );
        }

        public static Vector3 RotateX(this Vector3 v, float radian)
        {
            return new Vector3(
                v.X,
                (float)(Math.Cos(radian) * v.Y - Math.Sin(radian) * v.Z),
                (float)(Math.Sin(radian) * v.Y + Math.Cos(radian) * v.Z)
                );
        }

        public static Vector3 RotateZ(this Vector3 v, float radian)
        {
            return new Vector3(
                (float)(Math.Cos(radian) * v.X - Math.Sin(radian) * v.Y),
                (float)(Math.Sin(radian) * v.X + Math.Cos(radian) * v.Y),
                v.Z
                );
        }

        public static Vector3 Rotate(this Vector3 v, Vector3 rotationVector)
        {
            if (rotationVector.Y != 0) //y rotation
                v = v.RotateY(rotationVector.Y);
            if (rotationVector.Z != 0)
                v = v.RotateZ(rotationVector.Z);
            if (rotationVector.X != 0)
                v = v.RotateX(rotationVector.X);
            return v;
        }

        public static Vector3 Project(this Vector3 v, Engine engine, Camera camera)
        {
            float vY, vX;
            if (camera.Type == Camera.ProjectionType.Prespective)
            {
                // double
                var aratio = (double)engine.Width/engine.Height;
                var rad = camera.Fov/2.0*(Math.PI/180.0); // angles to radians
                var tan = Math.Tan(rad);
                // float
                vY = (float) (v.Y/(tan*v.Z));
                vX = (float) (v.X/(tan*v.Z*aratio));
            }
            else
            {
                vY = v.Y/v.Z;
                vX = v.X/v.Z;
            }
            return new Vector3(vX, vY, v.Z);
        }
    }
}