using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Neural
{
    public interface IMutable
    {
        double Mutability { get; set; }
        void Mutate();
    }

    public interface IMutable<T> : IMutable
    {
        T Value { get; set; }
        IMutable<T> GetCopy();
        IMutable<T> GetMutatedCopy();
    }
}
