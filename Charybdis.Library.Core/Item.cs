//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Charybdis.Library.Core
//{
//    public class Item
//    {
//        private static int _nextID = 0;

//        private int _id;
//        public int ID
//        {
//            get
//            {
//                return _id;
//            }
//            private set
//            {
//                _id = value;
//            }
//        }

//        public string Name;
//        public string Description = "No description available.";
//        public ItemType Type = ItemType.None;

//        public enum ItemType : byte
//        {
//            None = 0,
//            Block = 1,
//            Tool = 2,
//            Usable = 4,
//            Item = 8,
//            All = 127,
//        }

//        /// <summary>
//        /// Filesystem path (relative to "Data" folder) of the icon image file for this item.
//        /// </summary>
//        public string IconPath = null;

//        /// <summary>
//        /// The most of this item type that can occupy a single slot in an inventory.
//        /// </summary>
//        public int MaxStack;

//        private object _actual = null;
//        public object Actual
//        {
//            get
//            {
//                return _actual;
//            }
//        }

//        public Item(string name, object actual, int maxStack = 1)
//        {
//            Name = name;
//            _actual = actual;
//            if (actual is BlockType) Type = ItemType.Block;
//            else Type = ItemType.Item;
//            MaxStack = maxStack;
//            ID = _nextID++; //Assign the ID and increment the counter.
//            if (_nextID == int.MaxValue)
//            {
//                string message = "Too many items are in the game, we've hit the limit! Consider changing item IDs to the \"long\" type from \"int\".";
//                Console.WriteLine(message);
//                throw new IndexOutOfRangeException(message);
//            }
//        }
//    }
//}
