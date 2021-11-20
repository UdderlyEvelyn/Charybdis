//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Runtime.Serialization;

//namespace Charybdis.Library.Core
//{
//    [Serializable]
//    public class Material : ISerializable
//    {
//        public string Name = "Unknown";
//        public MaterialType Type = MaterialType.None;
//        public float Reflectivity = 0;
//        public float Hardness = 0;

//        public enum MaterialType : byte
//        {
//            None,
//            Matte,
//            Organic,
//            Metallic,
//            Liquid,
//        }

//        public static Material[] Materials = new Material[]
//        {
//            new Material { Type = MaterialType.None, Name = "Gas", Hardness = 0, Reflectivity = 0 },
//            new Material { Type = MaterialType.Metallic, Name = "Iron", Hardness = 10, Reflectivity = .3f },
//            new Material { Type = MaterialType.Metallic, Name = "Gold", Hardness = 3, Reflectivity = .8f },
//            new Material { Type = MaterialType.Organic, Name = "Dirt", Hardness = 1, Reflectivity = 0 },
//            new Material { Type = MaterialType.Matte, Name = "Stone", Hardness = 4, Reflectivity = .1f },
//            new Material { Type = MaterialType.Organic, Name = "Wood", Hardness = 2, Reflectivity = .2f },
//            new Material { Type = MaterialType.Organic, Name = "Plant", Hardness = 0, Reflectivity = .4f },
//            new Material { Type = MaterialType.Liquid, Name = "Water", Hardness = 0, Reflectivity = .9f },
//        };

//        public static Material Get(string s)
//        {
//            return Materials.Single(bt => bt.Name == s);
//        }

//        public Material() { } //parameterless constructor

//        protected Material(SerializationInfo info, StreamingContext ctxt)
//        {
//            Name = info.GetString("Name");
//            Type = (MaterialType)info.GetValue("Type", typeof(MaterialType));
//            Reflectivity = info.GetSingle("Reflectivity");
//            Hardness = info.GetSingle("Hardness");
//        }

//        public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
//        {
//            info.AddValue("Name", Name);
//            info.AddValue("Type", Type);
//            info.AddValue("Reflectivity", Reflectivity);
//            info.AddValue("Hardness", Hardness);
//        }
//    }
//}
