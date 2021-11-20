using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Charybdis.Library.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Index
    {
        public uint A;
        public uint B;
        public uint C;

        public Index(uint a, uint b, uint c)
        {
            A = a;
            B = b;
            C = c;
        }
    }
}
