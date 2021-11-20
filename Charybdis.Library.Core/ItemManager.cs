//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Charybdis.Library.Core
//{
//    public static class ItemManager
//    {
//        public static Item Dummy = new Item("Error", null, 0) { Description = "You should never see this item!" }; //Should get ID 0.

//        public static List<Item> _items = new List<Item>();
//        public static List<Item> Items
//        {
//            get
//            {
//                return _items;
//            }
//        }

//        public static Item Get(string s)
//        {
//            try
//            {
//                return _items.Single(i => i.Name == s);
//            }
//            catch
//            {
//                return Dummy;
//            }
//        }

//        public static Item Get(int id)
//        {
//            try
//            {
//                return _items.Single(i => i.ID == id);
//            }
//            catch
//            {
//                return Dummy;
//            }
//        }

//        public static void Add(Item item)
//        {
//            if (!_items.Contains(item))
//            {
//                _items.Add(item);
//            }
//            else Console.WriteLine("Attempted to add duplicate item, ID #" + item.ID + " \"" + item.Name + "\".");
//        }

//        public static void Remove(Item item)
//        {
//            if (_items.Contains(item))
//            {
//                _items.Remove(item);
//            }
//            else Console.WriteLine("Attempted to remove an item that was not present - ID # " + item.ID + " \"" + item.Name + "\".");
//        }
//    }
//}
