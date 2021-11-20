using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public class Operations
    {
        public Action<object> Update;
        public Action<string, object> Add;
        public Action<object> Delete;
        public Action Save;

        public IOperations<T> ForType<T>()
            where T : class
        {
            return new Operations<T>
            {
                Update = o => Update(o),
                Add = o => Add(typeof(T).Name, o),
                Delete = o => Delete(o),
                Save = Save,
            };
        }
    }

    public class Operations<T> : IOperations<T>
        where T : class
    {
        public Action<T> Update { get; internal set; }
        public Action<T> Add { get; internal set; }
        public Action<T> Delete { get; internal set; }
        public Action Save { get; internal set; }

        public static Operations<T> Define(Action<T> update, Action<T> add, Action<T> delete, Action save)
        {
            return new Operations<T>
            {
                Update = update,
                Add = add,
                Delete = delete,
                Save = save,
            };
        }
    }
}
