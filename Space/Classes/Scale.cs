using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space
{
    public static class Scale
    {
        #region Pixel<->Distance Scale (Ratio 1u:Npx)

        #region Solar Pixel Scale

        //Kilometers
        private static double _solarPixelsToKilometersRatio;
        public static double SolarPixelsToKilometersRatio
        {
            get
            {
                return _solarPixelsToKilometersRatio;
            }
            set
            {
                _solarPixelsToKilometersRatio = value;
                _solarPixelsToAstronomicalUnitsRatio = value / KILOMETERS_PER_ASTRONOMICAL_UNIT;
                _solarPixelsToLightyearsRatio = value / KILOMETERS_PER_LIGHTYEAR;
                _solarPixelsToParsecsRatio = value / KILOMETERS_PER_PARSEC;
            }
        }

        //Astronomical Units
        private static double _solarPixelsToAstronomicalUnitsRatio;
        public static double SolarPixelsToAstronomicalUnitsRatio
        {
            get
            {
                return _solarPixelsToAstronomicalUnitsRatio;
            }
            set
            {
                _solarPixelsToKilometersRatio = value * KILOMETERS_PER_ASTRONOMICAL_UNIT;
                _solarPixelsToAstronomicalUnitsRatio = value;
                _solarPixelsToLightyearsRatio = value / ASTRONOMICAL_UNITS_PER_LIGHTYEAR;
                _solarPixelsToParsecsRatio = value / ASTRONOMICAL_UNITS_PER_PARSEC;
            }
        }

        //Lightyears
        private static double _solarPixelsToLightyearsRatio;
        public static double SolarPixelsToLightyearsRatio
        {
            get
            {
                return _solarPixelsToLightyearsRatio;
            }
            set
            {
                _solarPixelsToKilometersRatio = value * KILOMETERS_PER_LIGHTYEAR;
                _solarPixelsToAstronomicalUnitsRatio = value * ASTRONOMICAL_UNITS_PER_LIGHTYEAR;
                _solarPixelsToLightyearsRatio = value;
                _solarPixelsToParsecsRatio = value / LIGHTYEARS_PER_PARSEC;
            }
        }

        //Parsecs
        private static double _solarPixelsToParsecsRatio;
        public static double SolarPixelsToParsecsRatio
        {
            get
            {
                return _solarPixelsToParsecsRatio;
            }
            set
            {
                _solarPixelsToKilometersRatio = value * KILOMETERS_PER_PARSEC;
                _solarPixelsToAstronomicalUnitsRatio = value * ASTRONOMICAL_UNITS_PER_PARSEC;
                _solarPixelsToLightyearsRatio = value * LIGHTYEARS_PER_PARSEC;
                _solarPixelsToParsecsRatio = value;
            }
        }

        #endregion

        #region Galactic Pixel Scale

        //Kilometers
        private static double _galacticPixelsToKilometersRatio;
        public static double GalacticPixelsToKilometersRatio
        {
            get
            {
                return _galacticPixelsToKilometersRatio;
            }
            set
            {
                _galacticPixelsToKilometersRatio = value;
                _galacticPixelsToAstronomicalUnitsRatio = value / KILOMETERS_PER_ASTRONOMICAL_UNIT;
                _galacticPixelsToLightyearsRatio = value / KILOMETERS_PER_LIGHTYEAR;
                _galacticPixelsToParsecsRatio = value / KILOMETERS_PER_PARSEC;
            }
        }

        //Astronomical Units
        private static double _galacticPixelsToAstronomicalUnitsRatio;
        public static double GalacticPixelsToAstronomicalUnitsRatio
        {
            get
            {
                return _galacticPixelsToAstronomicalUnitsRatio;
            }
            set
            {
                _galacticPixelsToKilometersRatio = value * KILOMETERS_PER_ASTRONOMICAL_UNIT;
                _galacticPixelsToAstronomicalUnitsRatio = value;
                _galacticPixelsToLightyearsRatio = value / ASTRONOMICAL_UNITS_PER_LIGHTYEAR;
                _galacticPixelsToParsecsRatio = value / ASTRONOMICAL_UNITS_PER_PARSEC;
            }
        }

        //Lightyears
        private static double _galacticPixelsToLightyearsRatio;
        public static double GalacticPixelsToLightyearsRatio
        {
            get
            {
                return _galacticPixelsToLightyearsRatio;
            }
            set
            {
                _galacticPixelsToKilometersRatio = value * KILOMETERS_PER_LIGHTYEAR;
                _galacticPixelsToAstronomicalUnitsRatio = value * ASTRONOMICAL_UNITS_PER_LIGHTYEAR;
                _galacticPixelsToLightyearsRatio = value;
                _galacticPixelsToParsecsRatio = value / LIGHTYEARS_PER_PARSEC;
            }
        }

        //Parsecs
        private static double _galacticPixelsToParsecsRatio;
        public static double GalacticPixelsToParsecsRatio
        {
            get
            {
                return _galacticPixelsToLightyearsRatio;
            }
            set
            {
                _galacticPixelsToKilometersRatio = value * KILOMETERS_PER_PARSEC;
                _galacticPixelsToAstronomicalUnitsRatio = value * ASTRONOMICAL_UNITS_PER_PARSEC;
                _galacticPixelsToLightyearsRatio = value * LIGHTYEARS_PER_PARSEC;
                _galacticPixelsToParsecsRatio = value;
            }
        }

        #endregion

        #region Solar Pixel Conversion

        #region To Solar Pixels

        public static double KilometersToSolarPixels(double kilometers)
        {
            return kilometers * SolarPixelsToKilometersRatio;
        }

        public static double AstronomicalUnitsToSolarPixels(double astronomicalUnits)
        {
            return astronomicalUnits * SolarPixelsToAstronomicalUnitsRatio;
        }

        public static double LightyearsToSolarPixels(double lightyears)
        {
            return lightyears * SolarPixelsToLightyearsRatio;
        }

        public static double ParsecsToSolarPixels(double parsecs)
        {
            return parsecs * SolarPixelsToParsecsRatio;
        }

        #endregion

        #region From Solar Pixels

        public static double SolarPixelsToKilometers(double solarPixels)
        {
            return solarPixels / SolarPixelsToKilometersRatio;
        }

        public static double SolarPixelsToAstronomicalUnits(double solarPixels)
        {
            return solarPixels / SolarPixelsToAstronomicalUnitsRatio;
        }

        public static double SolarPixelsToLightyears(double solarPixels)
        {
            return solarPixels / SolarPixelsToLightyearsRatio;
        }

        public static double SolarPixelsToParsecs(double solarPixels)
        {
            return solarPixels / SolarPixelsToParsecsRatio;
        }

        #endregion

        #endregion

        #region Galactic Pixel Conversion

        #region To Galactic Pixels

        public static double KilometersToGalacticPixels(double kilometers)
        {
            return kilometers * GalacticPixelsToKilometersRatio;
        }

        public static double AstronomicalUnitsToGalacticPixels(double astronomicalUnits)
        {
            return astronomicalUnits * GalacticPixelsToAstronomicalUnitsRatio;
        }

        public static double LightyearsToGalacticPixels(double lightyears)
        {
            return lightyears * GalacticPixelsToLightyearsRatio;
        }

        public static double ParsecsToGalacticPixels(double parsecs)
        {
            return parsecs * GalacticPixelsToParsecsRatio;
        }

        #endregion

        #region From Galactic Pixels

        public static double GalacticPixelsToKilometers(double galacticPixels)
        {
            return galacticPixels / GalacticPixelsToKilometersRatio;
        }

        public static double GalacticPixelsToAstronomicalUnits(double galacticPixels)
        {
            return galacticPixels / GalacticPixelsToAstronomicalUnitsRatio;
        }

        public static double GalacticPixelsToLightyears(double galacticPixels)
        {
            return galacticPixels / GalacticPixelsToLightyearsRatio;
        }

        public static double GalacticPixelsToParsecs(double galacticPixels)
        {
            return galacticPixels / GalacticPixelsToParsecsRatio;
        }

        #endregion

        #endregion

        #endregion

        #region Unit Conversion

        //For Reference:
        //Parsec > Lightyear > Astronomical Unit > Kilometer
        //Only upward ratios are stored in constants to avoid storing redundant information.

        #region Kilometer Conversion

        public const double KILOMETERS_PER_ASTRONOMICAL_UNIT = 149597870.7d;
        public const double KILOMETERS_PER_LIGHTYEAR = 9.4607e+15d;
        public const double KILOMETERS_PER_PARSEC = 30856776000000d;

        public static double KilometersToParsecs(double kilometers)
        {
            return kilometers / KILOMETERS_PER_PARSEC;
        }

        public static double KilometersToLightyears(double kilometers)
        {
            return kilometers / KILOMETERS_PER_LIGHTYEAR;
        }

        public static double KilometersToAstronomicalUnits(double kilometers)
        {
            return kilometers / KILOMETERS_PER_ASTRONOMICAL_UNIT;
        }

        #endregion

        #region Astronomical Unit Conversion

        public const double ASTRONOMICAL_UNITS_PER_LIGHTYEAR = 63241.08d;
        public const double ASTRONOMICAL_UNITS_PER_PARSEC = 206264.642116d;

        public static double AstronomicalUnitsToParsecs(double astronomicalUnits)
        {
            return astronomicalUnits / ASTRONOMICAL_UNITS_PER_PARSEC;
        }

        public static double AstronomicalUnitsToLightyears(double astronomicalUnits)
        {
            return astronomicalUnits / ASTRONOMICAL_UNITS_PER_LIGHTYEAR;
        }

        public static double AstronomicalUnitsToKilometers(double astronomicalUnits)
        {
            return astronomicalUnits * KILOMETERS_PER_ASTRONOMICAL_UNIT;
        }

        #endregion

        #region Lightyear Conversion

        public const double LIGHTYEARS_PER_PARSEC = 3.26156d;
        
        public static double LightyearsToParsecs(double lightyears)
        {
            return lightyears / LIGHTYEARS_PER_PARSEC;
        }

        public static double LightyearsToAstronomicalUnits(double lightyears)
        {
            return lightyears * ASTRONOMICAL_UNITS_PER_LIGHTYEAR;
        }

        public static double LightyearsToKilometers(double lightyears)
        {
            return lightyears * KILOMETERS_PER_LIGHTYEAR;
        }

        #endregion

        #region Parsec Conversion

        public static double ParsecsToKilometers(double parsecs)
        {
            return parsecs * KILOMETERS_PER_PARSEC;
        }

        public static double ParsecsToAstronomicalUnits(double parsecs)
        {
            return parsecs * ASTRONOMICAL_UNITS_PER_PARSEC;
        }

        public static double ParsecsToLightyears(double parsecs)
        {
            return parsecs * LIGHTYEARS_PER_PARSEC;
        }

        #endregion

        #endregion
    }
}
