using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public interface ILoadable
    {
        bool Loaded { get; set; }
        void Unload();
        void Load();
    }
}
