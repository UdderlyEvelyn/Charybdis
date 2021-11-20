using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace Charybdis.Library.Core
{
    public static class Dynamic
    {
        public static dynamic ToDynamicObject(this IDictionary<string, object> source)
        {
            ICollection<KeyValuePair<string, object>> eo = new ExpandoObject();
            foreach (var item in source)
                eo.Add(item);
            return eo;
        }
    }
}
