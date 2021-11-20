namespace Charybdis.Library.Core
{
    public struct Vertex
    {
        public Col3 color;
        public Vec3 vec3;

        public Vertex(Vec3 vec3, Col3 color)
        {
            this.vec3 = vec3;
            this.color = color;
        }
    }
}