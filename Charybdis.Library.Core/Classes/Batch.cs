using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public class Batch
    {
        static byte _nextID = 0;
        public byte ID;

        public Batch(List<Task> tasks = null)
        {
            ID = _nextID++;
            if (tasks != null)
            {
                lock (_tasks) _tasks.AddRange(tasks);
            }
        }

        private static List<Task> _tasks = new List<Task>();

        public Task<T> Execute<T>(Func<T> f)
        {
            Task<T> task = new Task<T>(f);
            _tasks.Add(task);
            return task;
        }

        public Task Execute(Action a)
        {
            Task task = new Task(a);
            _tasks.Add(task);
            return task;
        }

        public bool Done
        {
            get
            {
                return _tasks.TrueForAll(t => t.IsCompleted);
            }
        }
    }
}
