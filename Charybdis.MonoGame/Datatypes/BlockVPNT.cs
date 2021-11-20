using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Charybdis.MonoGame.Framework;
using VertexPositionNormalTexture = Microsoft.Xna.Framework.Graphics.VertexPositionNormalTexture;

namespace Charybdis.MonoGame.Datatypes
{
    /*
    public class Block : ICollidable3
    {
        private static ulong _nextID = 0;
        public ulong ID;

        private Vec3 _position;
        public Vec3 Position 
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                BoundingBox = new Microsoft.Xna.Framework.BoundingBox(_position - HalfSize, _position + HalfSize);
            }
        }
        public float Size { get; set; }
        public float HalfSize { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public BoundingBox BoundingBox { get; set; }

        public static Vec2 TexTL = new Vec2(0, 0);
        public static Vec2 TexTR = new Vec2(1, 0);
        public static Vec2 TexBL = new Vec2(0, 1);
        public static Vec2 TexBR = new Vec2(1, 1);

        public static Vec2 TexTopTL;
        public static Vec2 TexTopTR;
        public static Vec2 TexTopBL;
        public static Vec2 TexTopBR;
        //Side texture is upside-down.
        public static Vec2 TexSideTL;
        public static Vec2 TexSideTR;
        public static Vec2 TexSideBL;
        public static Vec2 TexSideBR;
        public static Vec2 TexBottomTL;
        public static Vec2 TexBottomTR;
        public static Vec2 TexBottomBL;
        public static Vec2 TexBottomBR;

        public static Plane NegXPlane = new Plane(-1, 0, 0, 0);
        public static Plane PosYPlane = new Plane(0, -1, 0, 0);
        public static Plane PosZPlane = new Plane(0, 0, -1, 0);
        public static Plane PosXPlane = new Plane(1, 0, 0, 1);
        public static Plane NegYPlane = new Plane(0, 1, 0, 1);
        public static Plane NegZPlane = new Plane(0, 0, 1, 1);

        public Vec4 NegXOcclusion = new Vec4(0);
        public Vec4 PosXOcclusion = new Vec4(0);
        public Vec4 NegYOcclusion = new Vec4(0);
        public Vec4 PosYOcclusion = new Vec4(0);
        public Vec4 NegZOcclusion = new Vec4(0);
        public Vec4 PosZOcclusion = new Vec4(0);

        public VertexPositionNormalTexture[] NegXVertices
        {
            get
            {
                return new VertexPositionNormalTexture[6]
                {
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NNN, Vec3.NNN, TexSideTL), //0 1
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NNP, Vec3.NNP, TexSideTR), //1 2
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NPP, Vec3.NPP, TexSideBR), //3 3
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NNN, Vec3.NNN, TexSideTL), //0 13
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NPP, Vec3.NPP, TexSideBR), //3 14
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NPN, Vec3.NPN, TexSideBL), //2 15
                };
            }
        }

        public VertexPositionNormalTexture[] NegYVertices
        {
            get
            {
                return new VertexPositionNormalTexture[6]
                {
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PNP, Vec3.PNP, TexBottomBR), //5 7
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NNN, Vec3.NNN, TexBottomTL), //0 8
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PNN, Vec3.PNN, TexBottomTR), //4 9
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PNP, Vec3.PNP, TexBottomBR), //5 16
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NNP, Vec3.NNP, TexBottomBL), //1 17
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NNN, Vec3.NNN, TexBottomTL), //0 18
                };
            }
        }

        public VertexPositionNormalTexture[] NegZVertices
        {
            get
            {
                return new VertexPositionNormalTexture[6]
                {
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PPN, Vec3.PPN, TexSideBL), //6 4
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NNN, Vec3.NNN, TexSideTR), //0 5
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NPN, Vec3.NPN, TexSideBR), //2 6
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PPN, Vec3.PPN, TexSideBL), //6 10
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PNN, Vec3.PNN, TexSideTL), //4 11
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NNN, Vec3.NNN, TexSideTR), //0 12
                };
            }
        }

        public VertexPositionNormalTexture[] PosZVertices
        {
            get
            {
                return new VertexPositionNormalTexture[6]
                {
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NPP, Vec3.NPP, TexSideBL), //3 19
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NNP, Vec3.NNP, TexSideTL), //1 20
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PNP, Vec3.PNP, TexSideTR), //5 21
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PPP, Vec3.PPP, TexSideBR), //7 34
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NPP, Vec3.NPP, TexSideBL), //3 35
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PNP, Vec3.PNP, TexSideTR), //5 36
                };
            }
        }

        public VertexPositionNormalTexture[] PosXVertices
        {
            get
            {
                return new VertexPositionNormalTexture[6]
                {
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PPP, Vec3.PPP, TexSideBL), //7 22
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PNN, Vec3.PNN, TexSideTR), //4 23
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PPN, Vec3.PPN, TexSideBR), //6 24
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PNN, Vec3.PNN, TexSideTR), //4 25
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PPP, Vec3.PPP, TexSideBL), //7 26
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PNP, Vec3.PNP, TexSideTL), //5 27
                };
            }
        }

        public VertexPositionNormalTexture[] PosYVertices
        {
            get
            {
                return new VertexPositionNormalTexture[6]
                {
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PPP, Vec3.PPP, TexTopTR), //7 28
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PPN, Vec3.PPN, TexTopBR), //6 29
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NPN, Vec3.NPN, TexTopBL), //2 30
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.PPP, Vec3.PPP, TexTopTR), //7 31
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NPN, Vec3.NPN, TexTopBL), //2 32
                    new VertexPositionNormalTexture(Position + HalfSize * Vec3.NPP, Vec3.NPP, TexTopTL), //3 33
                };
            }
        }

        public bool IsVisible { get; set; }

        public Block(Vec3 position, BlockType type, float size = 1, float yaw = 0, float pitch = 0, float roll = 0)
        {
            ID = _nextID++;
            Size = size;
            HalfSize = size / 2;
            BoundingBox = new Microsoft.Xna.Framework.BoundingBox(position - HalfSize, position + HalfSize);
            Yaw = yaw;
            Pitch = pitch;
            Roll = roll;
            Type = type;
            if (Type.BottomTextureCoordinates != null)
            {
                TexBottomTL = new Vec2(Type.BottomTextureCoordinates.X, Type.BottomTextureCoordinates.Y);
                TexBottomTR = new Vec2(Type.BottomTextureCoordinates.Z, Type.BottomTextureCoordinates.Y);
                TexBottomBL = new Vec2(Type.BottomTextureCoordinates.X, Type.BottomTextureCoordinates.W);
                TexBottomBR = new Vec2(Type.BottomTextureCoordinates.Z, Type.BottomTextureCoordinates.W);
            }
            if (Type.SideTextureCoordinates != null)
            {
                TexSideTL = new Vec2(Type.SideTextureCoordinates.X, Type.SideTextureCoordinates.Y);
                TexSideTR = new Vec2(Type.SideTextureCoordinates.Z, Type.SideTextureCoordinates.Y);
                TexSideBL = new Vec2(Type.SideTextureCoordinates.X, Type.SideTextureCoordinates.W);
                TexSideBR = new Vec2(Type.SideTextureCoordinates.Z, Type.SideTextureCoordinates.W);
            }
            if (Type.TopTextureCoordinates != null)
            {
                TexTopTL = new Vec2(Type.TopTextureCoordinates.X, Type.TopTextureCoordinates.Y);
                TexTopTR = new Vec2(Type.TopTextureCoordinates.Z, Type.TopTextureCoordinates.Y);
                TexTopBL = new Vec2(Type.TopTextureCoordinates.X, Type.TopTextureCoordinates.W);
                TexTopBR = new Vec2(Type.TopTextureCoordinates.Z, Type.TopTextureCoordinates.W);
            }
            Position = position;
            IsVisible = true;
        }

        [Flags]
        public enum Facings : byte
        {
            None = 0,
            PosX = 1,
            NegX = 2,
            NegY = 4,
            PosY = 8,
            NegZ = 16,
            PosZ = 32,
            All = 127,
        };

        public static Block New(Vec3 position, BlockType type = null, float size = 1, float yaw = 0, float pitch = 0, float roll = 0)
        {
            return new Block(position, type, size, yaw, pitch, roll);
        }

        public VertexPositionNormalTexture[] GetVertices()
        {
            List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
            if (Facing.HasFlag(Facings.NegX)) vertices.AddRange(NegXVertices);
            if (Facing.HasFlag(Facings.PosX)) vertices.AddRange(PosXVertices);
            if (Facing.HasFlag(Facings.NegY)) vertices.AddRange(NegYVertices);
            if (Facing.HasFlag(Facings.PosY)) vertices.AddRange(PosYVertices);
            if (Facing.HasFlag(Facings.NegZ)) vertices.AddRange(NegZVertices);
            if (Facing.HasFlag(Facings.PosZ)) vertices.AddRange(PosZVertices);
            return vertices.ToArray();
        }

        public BlockType Type;
        public Facings Facing = Facings.None;
        public float Light = 0;
    }
     */
}
