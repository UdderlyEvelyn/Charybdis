using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinitum
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var kernel = new InfinitumKernel())
                kernel.Run();
        }
    }
}
