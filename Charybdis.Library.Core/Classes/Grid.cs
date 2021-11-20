using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core.Classes
{
    public class Grid<T>
    {
        public int Columns
        {
            get
            {
                return _data.Count;
            }
        }

        public int Rows
        {
            get
            {
                return _data.Max(l => l.Count);
            }
        }

        List<List<T>> _data = new List<List<T>>();

        public void AddColumn()
        {
            _data.Add(new List<T>());
        }

        public void AddRowToColumn(int columnIndex, T value)
        {
            _data[columnIndex].Add(value);
        }

        public override string ToString()
        {
            List<string> stringRows = new List<string>(Rows);
            for (int r = 0; r < Rows; r++)
            {
                string stringRow = "R" + r + " | ";
                for (int c = 0; c < Columns; c++)
                    if (_data.Count > c && _data[c].Count > r)
                        stringRow += _data[c][r] + " | ";
                    else
                        stringRow += "NULL | ";
                stringRows.Add(stringRow);
            }
            return stringRows.Join('\n');
        }
    }
}
