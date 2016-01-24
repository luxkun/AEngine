using OpenTK;

namespace AEngine
{
    public class Camera
    {
        public float Fov { get; set; } = 60f;
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
    }
}