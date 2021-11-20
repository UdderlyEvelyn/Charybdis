using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Charybdis.Science;

namespace Charybdis.Fortress
{
    public class Tile : FortressObject
    {
        public Tile(Vec2 position, Random r, double initialTemperature)
            : this((Vec3)position, r, initialTemperature)
        {

        }

        public Tile(Vec3 position, Random r, double initialTemperature)
        {
            Temperature = new Temperature(initialTemperature, Temperature.TemperatureScale.F);
            Position = position;
        }

        public double Density { get; set; }

        private Temperature _temperature;
        public Temperature Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                _temperature = value;
            }
        }
    }
}
