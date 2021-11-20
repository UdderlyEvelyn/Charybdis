using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Charybdis.Library.Core
{
    [Serializable]
    public struct Change : ISerializable
    {
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
            }
        }

        private ulong _blockTypeID;
        public ulong BlockTypeID 
        {
            get
            {
                return _blockTypeID;
            }
            set
            {
                _blockTypeID = value;
            }
        }

        public Change(Vec3 position, ulong blockTypeID)
        {
            _position = position;
            _blockTypeID = blockTypeID;
        }

        private Change(SerializationInfo info, StreamingContext ctxt)
        {
            _position = (Vec3)info.GetValue("Position", typeof(Vec3));
            _blockTypeID = info.GetUInt64("BlockTypeID");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Position", _position);
            info.AddValue("BlockTypeID", _blockTypeID);
        }

        public string GetData()
        {
            return _position.Xi + "," + _position.Yi + "," + _position.Zi + ":" + _blockTypeID;
        }

        public Change(string data)
        {
            string[] d = data.Split(':');
            _blockTypeID = ulong.Parse(d[1]);
            d = d[0].Split(',');
            int x = int.Parse(d[0]);
            int y = int.Parse(d[1]);
            int z = int.Parse(d[2]);
            _position = new Vec3(x, y, z);
        }
    }
}
