using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.Library.Core
{
    public class Instance
    {
        public Vec3 Position { get; private set; }
        public float Size { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public Model Parent { get; private set; }
        public TextureVertex[] PositionedVertices { get; private set; }
        public bool IsVisible { get; set; }

        public Instance(Model parent, Vec3 position, float size, float yaw, float pitch, float roll)
        {
            Parent = parent;
            Size = size;
            Yaw = yaw;
            Pitch = pitch;
            Roll = roll;
            SetPosition(position);
            IsVisible = true;
        }

        public void SetPosition(Vec3 position)
        {
            Position = position;
            PositionedVertices = new TextureVertex[Parent.Vertices.Count()];
            for (int i = 0; i < Parent.Vertices.Count(); i++)
            {
                PositionedVertices[i] = new TextureVertex(Parent.Vertices[i].Position + position, Parent.Vertices[i].TextureCoordinate); //Parent.Vertices[i].Color);
            }
        }
    }
}
