using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Charybdis.Library.Core
{
    public static class ThreadManager
    {
        public static List<Thread> Threads = new List<Thread>();

        public static Thread New(Action action)
        {
            Thread t = new Thread(new ThreadStart(action));
            Threads.Add(t);
            t.Start();
            return t;
        }

        public static Thread New(Action<object> action, object parameter = null)
        {
            Thread t = new Thread(new ParameterizedThreadStart((object obj) => action(obj)));
            Threads.Add(t);
            t.Start(parameter);
            return t;
        }
    }
}
