using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    /// <summary>
    /// a FIFO/LILO data structure
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Queue<T> : IEnumerable<T>
    {
        protected List<T> _data;

        /// <summary>
        /// pushes a new item into the queue
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            _data.Insert(0, item);
        }

        /// <summary>
        /// returns the item at the end of the queue and then removes it from the queue
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            int pos = _data.Count - 1;
            T ret = _data[pos];
            _data.RemoveAt(pos);
            return ret;
        }

        /// <summary>
        /// returns a copy of the entire set of backing data for this queue as an array
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            return _data.ToArray();
        }

        /// <summary>
        /// returns the element at the requested position - does not modify the data set
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T Peek(int i)
        {
            return _data[i];
        }

        /// <summary>
        /// shifts everything in the queue by the amount specified - negative values are permitted for going the other direction
        /// </summary>
        /// <param name="i"></param>
        public void Shift(int i)
        {
            if (i > 0)
            {
                for (int j = 0; j < i; j++) //left
                {
                    int pos = _data.Count - 1;
                    _data.Insert(pos, _data[0]);
                    _data.RemoveAt(0);
                }
            }
            else if (i < 0) //right
            {
                for (int j = 0; j < Math.Abs(i); j++)
                {
                    Push(Pop()); //lol pushpop :P
                }
            }
            else throw new ArgumentOutOfRangeException("i", "You can't shift zero spaces, it would do nothing and be a waste of resources.");
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        protected IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }
}
