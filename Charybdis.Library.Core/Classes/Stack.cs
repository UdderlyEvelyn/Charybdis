//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Charybdis.Library.Core
//{
//    /// <summary>
//    /// a FILO/LIFO data structure
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public class Stack<T> : IEnumerable<T>
//    {
//        protected List<T> _data;

//        /// <summary>
//        /// pushes a new item onto the stack
//        /// </summary>
//        /// <param name="item"></param>
//        public void Push(T item)
//        {
//            _data.Insert(0, item);
//        }

//        /// <summary>
//        /// returns the next item on the stack and subsequently removes it from the stack
//        /// </summary>
//        /// <returns></returns>
//        public T Pop()
//        {
//            T ret = _data[0];
//            _data.RemoveAt(0);
//            return ret;
//        }

//        /// <summary>
//        /// returns a copy of the entire set of backing data for this stack
//        /// </summary>
//        /// <returns></returns>
//        public T[] ToArray()
//        {
//            return _data.ToArray();
//        }

//        /// <summary>
//        /// returns the element at the requested position - does not modify the data set
//        /// </summary>
//        /// <param name="i"></param>
//        /// <returns></returns>
//        public T Peek(int i)
//        {
//            return _data[i];
//        }

//        /// <summary>
//        /// shifts everything in the stack by the amount specified - negative values are permitted for going the other direction
//        /// </summary>
//        /// <param name="i"></param>
//        public void Shift(int i)
//        {
//            if (i > 0)
//            {
//                for (int j = 0; j < i; j++)
//                {
//                    _data.Insert(_data.Count - 1, Pop());
//                }
//            }
//            else if (i < 0)
//            {
//                for (int j = 0; j < Math.Abs(i); j++)
//                {
//                    int pos = _data.Count - 1;
//                    _data.Insert(0, _data[pos]);
//                    _data.RemoveAt(pos);
//                }
//            }
//            else throw new ArgumentOutOfRangeException("i", "You can't shift zero spaces, it would do nothing and be a waste of resources.");
//        }

//        IEnumerator<T> IEnumerable<T>.GetEnumerator()
//        {
//            return _data.GetEnumerator();
//        }

//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//        {
//            return _data.GetEnumerator();
//        }

//        protected IEnumerator<T> GetEnumerator()
//        {
//            return _data.GetEnumerator();
//        }
//    }
//}
