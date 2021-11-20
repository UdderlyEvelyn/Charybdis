//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Runtime.Serialization;

//namespace Charybdis.Library.Core
//{
//    [Serializable]
//    public class BlockType : ISerializable
//    {
//        public ulong ID;
//        private static ulong nextID = 1; //0 represents "null" type, so we start at 1.

//        public string Name;
//        public Material Material;
//        public bool Drops;
//        public Vec4 TopTextureCoordinates;
//        public Vec4 SideTextureCoordinates;
//        public Vec4 BottomTextureCoordinates;
//        public bool Transparency = false;

//        public BlockType(string name, Material material, Vec4 texTop, Vec4 texBottom, Vec4 texSide, bool transparency = false, bool drops = true)
//        {
//            ID = nextID++;
//            Drops = drops;
//            Name = name;
//            Material = material;
//            TopTextureCoordinates = texTop;
//            BottomTextureCoordinates = texBottom;
//            SideTextureCoordinates = texSide;
//            Transparency = transparency;
//            if (BottomTextureCoordinates != Vec4.Zero)
//            {
//                TexBottomX = new Vec2(BottomTextureCoordinates.X, BottomTextureCoordinates.Y);
//                TexBottomY = new Vec2(BottomTextureCoordinates.Z, BottomTextureCoordinates.Y);
//                TexBottomZ = new Vec2(BottomTextureCoordinates.Z, BottomTextureCoordinates.W);
//                TexBottomW = new Vec2(BottomTextureCoordinates.X, BottomTextureCoordinates.W);
//            }
//            if (SideTextureCoordinates != Vec4.Zero)
//            {
//                TexSideX = new Vec2(SideTextureCoordinates.X, SideTextureCoordinates.Y);
//                TexSideY = new Vec2(SideTextureCoordinates.Z, SideTextureCoordinates.Y);
//                TexSideZ = new Vec2(SideTextureCoordinates.Z, SideTextureCoordinates.W);
//                TexSideW = new Vec2(SideTextureCoordinates.X, SideTextureCoordinates.W);
//            }
//            if (TopTextureCoordinates != Vec4.Zero)
//            {
//                TexTopX = new Vec2(TopTextureCoordinates.X, TopTextureCoordinates.Y);
//                TexTopY = new Vec2(TopTextureCoordinates.Z, TopTextureCoordinates.Y);
//                TexTopZ = new Vec2(TopTextureCoordinates.Z, TopTextureCoordinates.W);
//                TexTopW = new Vec2(TopTextureCoordinates.X, TopTextureCoordinates.W);
//            }
//        }

//        protected BlockType(SerializationInfo info, StreamingContext ctxt)
//        {
//            Name = info.GetString("Name");
//            Material = (Material)info.GetValue("Material", typeof(Material));
//            Drops = info.GetBoolean("Drops");
//            TopTextureCoordinates = (Vec4)info.GetValue("TopTextureCoordinates", typeof(Vec4));
//            SideTextureCoordinates = (Vec4)info.GetValue("SideTextureCoordinates", typeof(Vec4));
//            BottomTextureCoordinates = (Vec4)info.GetValue("BottomTextureCoordinates", typeof(Vec4));
//            Transparency = info.GetBoolean("Transparency");
//            if (BottomTextureCoordinates != Vec4.Zero)
//            {
//                TexBottomX = new Vec2(BottomTextureCoordinates.X, BottomTextureCoordinates.Y);
//                TexBottomY = new Vec2(BottomTextureCoordinates.Z, BottomTextureCoordinates.Y);
//                TexBottomZ = new Vec2(BottomTextureCoordinates.Z, BottomTextureCoordinates.W);
//                TexBottomW = new Vec2(BottomTextureCoordinates.X, BottomTextureCoordinates.W);
//            }
//            if (SideTextureCoordinates != Vec4.Zero)
//            {
//                TexSideX = new Vec2(SideTextureCoordinates.X, SideTextureCoordinates.Y);
//                TexSideY = new Vec2(SideTextureCoordinates.Z, SideTextureCoordinates.Y);
//                TexSideZ = new Vec2(SideTextureCoordinates.Z, SideTextureCoordinates.W);
//                TexSideW = new Vec2(SideTextureCoordinates.X, SideTextureCoordinates.W);
//            }
//            if (TopTextureCoordinates != Vec4.Zero)
//            {
//                TexTopX = new Vec2(TopTextureCoordinates.X, TopTextureCoordinates.Y);
//                TexTopY = new Vec2(TopTextureCoordinates.Z, TopTextureCoordinates.Y);
//                TexTopZ = new Vec2(TopTextureCoordinates.Z, TopTextureCoordinates.W);
//                TexTopW = new Vec2(TopTextureCoordinates.X, TopTextureCoordinates.W);
//            }
//        }

//        public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
//        {
//            info.AddValue("Name", Name);
//            info.AddValue("Material", Material);
//            info.AddValue("Drops", Drops);
//            info.AddValue("TopTextureCoordinates", TopTextureCoordinates);
//            info.AddValue("SideTextureCoordinates", SideTextureCoordinates);
//            info.AddValue("BottomTextureCoordinates", BottomTextureCoordinates);
//            info.AddValue("Transparency", Transparency);
//        }

//        static float tileSize = 16;

//        public static Dictionary<string, BlockType> Types = new Dictionary<string, BlockType>
//        {
//            //{ "Air", new BlockType("Air", Material.Get("Gas"), Vec4.Zero, Vec4.Zero, Vec4.Zero, false, false) },
//            { "Dirt", new BlockType("Dirt", Material.Get("Dirt"), new Vec4(0, 0, 1, 1) / tileSize, new Vec4(0, 0, 1, 1) / tileSize, new Vec4(0, 0, 1, 1) / tileSize ) },
//            { "Grass", new BlockType("Grass", Material.Get("Dirt"), new Vec4(1, 0, 2, 1) / tileSize, new Vec4(0, 0, 1, 1) / tileSize, new Vec4(0, 0, 1, 1) / tileSize) },
//            //new BlockType(Material.Get("Dirt"), Name = "Grass", new Vec4(14, 15, 15, 16) / tileSize, new Vec4(14, 15, 15, 16) / tileSize, new Vec4(15, 15, 16, 16) / tileSize },
//            { "Stone", new BlockType("Stone", Material.Get("Stone"), new Vec4(2, 0, 3, 1) / tileSize, new Vec4(2, 0, 3, 1) / tileSize, new Vec4(2, 0, 3, 1) / tileSize) },
//            { "Iron", new BlockType("Iron", Material.Get("Iron"), new Vec4(3, 0, 4, 1) / tileSize, new Vec4(3, 0, 4, 1) / tileSize, new Vec4(3, 0, 4, 1) / tileSize) },
//            { "Gold", new BlockType("Gold", Material.Get("Gold"), new Vec4(4, 0, 5, 1) / tileSize, new Vec4(4, 0, 5, 1) / tileSize, new Vec4(4, 0, 5, 1) / tileSize) },
//            { "Test", new BlockType("Test", Material.Get("Stone"), new Vec4(14, 15, 15, 16) / tileSize, new Vec4(14, 15, 15, 16) / tileSize, new Vec4(15, 15, 16, 16) / tileSize, false, false) },
//            { "Log", new BlockType("Log", Material.Get("Wood"), new Vec4(5, 0, 6, 1) / tileSize, new Vec4(5, 0, 6, 1) / tileSize, new Vec4(6, 0, 7, 1) / tileSize) },
//            { "Leaves", new BlockType("Leaves", Material.Get("Plant"), new Vec4(7, 0, 8, 1) / tileSize, new Vec4(7, 0, 8, 1) / tileSize, new Vec4(7, 0, 8, 1) / tileSize, true) },
//            { "Water", new BlockType("Water", Material.Get("Water"), new Vec4(8, 0, 9, 1) / tileSize, new Vec4(8, 0, 9, 1) / tileSize, new Vec4(8, 0, 9, 1) / tileSize, true, false) }
//        };

//        public Vec2 TexTopX;
//        public Vec2 TexTopY;
//        public Vec2 TexTopZ;
//        public Vec2 TexTopW;
//        public Vec2 TexSideX;
//        public Vec2 TexSideY;
//        public Vec2 TexSideZ;
//        public Vec2 TexSideW;
//        public Vec2 TexBottomX;
//        public Vec2 TexBottomY;
//        public Vec2 TexBottomZ;
//        public Vec2 TexBottomW;

//        public static BlockType Get(string s)
//        {
//            BlockType bt = null;
//            Types.TryGetValue(s, out bt);
//            return bt;
//        }

//        public static BlockType Get(ulong id)
//        {
//            return Types.Single(t => t.Value.ID == id).Value;
//        }
//    }
//}
