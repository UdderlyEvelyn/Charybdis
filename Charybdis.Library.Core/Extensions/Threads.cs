using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Charybdis.Library.Core
{
    public static class Threads
    {
        private static List<Batch> _jobs = new List<Batch>();
        public static List<Batch> Jobs
        {
            get
            {
                return _jobs;
            }
        }

        public static byte AddJob(Task task)
        {
            return AddJob(new List<Task> { task });
        }

        public static byte AddJob(List<Task> tasks)
        {
            Batch b = new Batch(tasks);
            _jobs.Add(b);
            return b.ID;
        }

        public static bool Check(byte id)
        {
            return _jobs.Single(b => b.ID == id).Done;
        }

        public static void ClearComplete()
        {
            Parallel.ForEach(Jobs.Where(b => b.Done), delegate(Batch b) { Jobs.Remove(b); });
        }
    }
}
