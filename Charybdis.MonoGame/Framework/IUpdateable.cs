using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.MonoGame
{
    public interface IUpdateable
    {
        ulong Updates { get; }
        void Update();
    }
}
