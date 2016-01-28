using System.Numerics;

namespace AEngine
{
    public class Camera
    {
        public float Fov { get; set; } = 60f;
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public ProjectionType Type { get; set; } = ProjectionType.Prespective;

        public enum ProjectionType
        {
            Prespective, Orthographic
        }
    }
}