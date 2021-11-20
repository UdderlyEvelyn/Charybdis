using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.Library.Core
{
    public class CharybdisObject
    {
        //Simple auto-incrementing ID system.. (could be expanded to support reusing old IDs)
        internal static ulong _nextID = 1;
        public ulong ID = _nextID++;

        internal static Dictionary<string, CharybdisObject> _all = new Dictionary<string, CharybdisObject>();

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set //Enforce name uniqueness.
            {
                if (_name == null) //We're trying to assign a null unique name to this object..
                    _name = Guid.NewGuid().ToString(); //Give it a GUID so it has something unique.
                if (_name != value)
                {
                    if (_all.ContainsKey(_name))
                    {
                        _all.Remove(_name);
                    }
                        _name = value;
                        _all.Add(_name, this);
                }
            }
        }
        public string Description { get; set; }

        public CharybdisObject Owner { get; set; }
    }
}