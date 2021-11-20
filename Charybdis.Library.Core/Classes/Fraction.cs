using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public class Fraction<T>
        where T : struct
    {
        public T Current;
        private T _max;
        public T Max
        {
            get
            {
                return _max;
            }
            private set
            {
                _max = value;
            }
        }

        protected Fraction()
        {
#if PARANOIA
            if (!(new Type[]
                {
                    typeof(sbyte),
                    typeof(short),
                    typeof(int),
                    typeof(long),
                    typeof(byte),
                    typeof(ushort),
                    typeof(uint),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }.Contains(typeof(T))))
                throw new ArgumentException("You must use a numeric type with this class for the type parameter 'T'.", "T");
#endif
        }

        public Fraction(T max)
        {
            _max = max;
            Current = max;
        }

        public Fraction(T current, T max)
        {
            Current = current;
            _max = max;
        }
    }
}
