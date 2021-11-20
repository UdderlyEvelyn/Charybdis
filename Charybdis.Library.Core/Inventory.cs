//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Charybdis.Library.Core
//{
//    public class Inventory
//    {
//        public int Width = 0;
//        public int Height = 0;

//        private Item _selectedItem = ItemManager.Dummy;
//        public Item SelectedItem
//        {
//            get
//            {
//                return _selectedItem;
//            }
//        }

//        public void Select(int x, int y)
//        {
//            _selectedItem = ItemManager.Get(_layout.Get(x, y));
//        }

//        public void Select(Item i)
//        {
//            _selectedItem = i;
//        }

//        private Dictionary<Item, int> _items = new Dictionary<Item, int>();
//        public Dictionary<Item, int> Items 
//        { 
//            get
//            {
//                return _items; 
//            }
//        }

//        public int Slots
//        {
//            get
//            {
//                return _layout.Count;
//            }
//        }

//        private Array2<int> _layout;

//        public Inventory(int width, int height)
//        {
//            Width = width;
//            Height = height;
//            _layout = new Array2<int>(width, height, 0);
//        }

//        public bool Put(Item i)
//        {
//            if (_items.ContainsKey(i) && _items[i] % i.MaxStack > 0)
//            {
//                _items[i]++;
//                return true; //Already had a slot with space.
//            }
//            else
//            {
//                for (int x = 0; x < Slots; x++)
//                {
//                    if (IsVacant(x)) //0 is the default "nothing is here" ID.
//                    {
//                        _layout.Put(x, i.ID);
//                        if (_items.ContainsKey(i)) _items[i]++;
//                        else _items.Add(i, 1);
//                        return true; //Found a slot.
//                    }
//                }
//                return false; //Failed to find a slot.
//            }
//        }

//        public bool Use(Item i)
//        {
//            if (Count(i) > 0)
//            {
//                _items[i]--;
//                return true;
//            }
//            else return false;
//        }

//        public bool UseSelected()
//        {
//            return Use(_selectedItem);
//        }

//        public bool IsVacant(int x, int y)
//        {
//            return _layout.Get(x, y) == 0;
//        }

//        public bool IsVacant(int i)
//        {
//            return _layout.Get(i) == 0;
//        }

//        public int Count(Item i)
//        {
//            if (_items.ContainsKey(i))
//            {
//                return _items[i];
//            }
//            else return 0;
//        }
//    }
//}
