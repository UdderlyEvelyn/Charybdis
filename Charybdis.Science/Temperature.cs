using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.Science
{
    public class Temperature
    {
        public double Value;

        public TemperatureScale Scale;

        public Temperature(double value, TemperatureScale scale = TemperatureScale.K)
        {
            Value = value;
            Scale = scale;
        }

        public enum TemperatureScale : int
        {
            F, //Fahrenheit
            C, //Celsius/Centigrade
            K, //Kelvin
        }

        //Constants sourced from http://www.zombieprototypes.com/?p=210
        const double RED_A = 351.97690566805693;
        const double RED_B = .114206453784165;
        const double RED_C = -40.25366309332127;
        const double GREEN_10_TO_66_A = -155.25485562709179;
        const double GREEN_10_TO_66_B = -.44596950469579133;
        const double GREEN_10_TO_66_C = 104.49216199393888;
        const double GREEN_66_PLUS_A = 325.4494125711974;
        const double GREEN_66_PLUS_B = .07943456536662342;
        const double GREEN_66_PLUS_C = -28.0852963507957;
        const double BLUE_A = -254.76935184120902;
        const double BLUE_B = .8274096064007395;
        const double BLUE_C = 11567994401066147;

        //http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/ (Original)
        //http://www.zombieprototypes.com/?p=210 (Corrected Fit Curve)
        //Rewritten by UdderlyEvelyn for efficiency/readability.
        //UdderlyEvelyn 10/3/18 - Refactored for multiple temperature scale support, added code to handle Fahrenheit.
        /// <summary>
        /// Converts temperature in degrees Kelvin or Fahrenheit to an RGB Col3.
        /// </summary>
        /// <returns></returns>
        public Col3 GetColor()
        {
            double r = 0;
            double g = 0;
            double b = 0;

            switch (Scale)
            {
                case TemperatureScale.K:
                    //Clamp between 1000K and 40000K, divide by 100 (required by algorithm), discard decimal places
                    double tmpValue = Math.Round(Maths.Clamp(Value, 1000, 40000) / 100);

                    double redX = tmpValue - 55;
                    double green10to66X = tmpValue - 2;
                    double green66plusX = tmpValue - 50;
                    double blueX = tmpValue - 10;

                    if (tmpValue <= 66) //<=6600K
                    {
                        r = 255;
                        g = colorChannelFunction(GREEN_10_TO_66_A, GREEN_10_TO_66_B, GREEN_10_TO_66_C, green10to66X);
                        if (tmpValue <= 19) //<2000K
                            b = 0;
                        else //2000K<=N<=6600K
                            b = colorChannelFunction(BLUE_A, BLUE_B, BLUE_C, blueX);
                    }
                    else //>6600K
                    {
                        r = colorChannelFunction(RED_A, RED_B, RED_C, redX);
                        g = colorChannelFunction(GREEN_66_PLUS_A, GREEN_66_PLUS_B, GREEN_66_PLUS_C, green66plusX);
                        b = 255;
                    }
                    break;
                case TemperatureScale.F:
                    if (Value > 137)
                        return Col3.Red;
                    if (Value < -15)
                        return Col3.Blue;
                    else
                    {
                        r = Maths.Clamp(Math.Tan((Value - 60) / 50) * 255, 0, 255);
                        b = Maths.Clamp(-Math.Tan((Value - 60) / 60) * 255, 0, 255); //r=tan(x-60/60)
                        g = Maths.Clamp(Math.Cos((Value - 60) / 50) * 255, 0, 255);
                    }
                    break;
                default:
                    throw new NotImplementedException("The GetColor method has not been implemented for the temperature scale \"" + Scale.GetName() + "\".");
            }

            //Loss of accuracy as we go from double->float.
            return new Col3((float)r, (float)g, (float)b);
        }

        /// <summary>
        /// Returns a+bx+cln(x) clamped between min and max.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="x"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private double colorChannelFunction(double a, double b, double c, double x, double min = 0, double max = 255)
        {
            //I know some of the parenthesis are unnecessary but it aids readability/comprehension.
            return Maths.Clamp((a + (b * x) + (c * Math.Log(x))), min, max);
        }

        /// <summary>
        /// Returns a string representation of the temperature and scale, rounding to the nearest integer for the order of magnitude in the case of Kelvin.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Scale == TemperatureScale.K)
                return Value.Round(RoundingType.OrderOfMagnitude).ToString() + Scale.ToString();
            else
                return Value.Round().ToString() + Scale.ToString();
        }

        #region Operators

        #region Division

        public static Temperature operator /(Temperature t, Temperature t2)
        {
            if (t.Scale != t2.Scale)
                throw new NotSupportedException("Performing math on temperatures in different scales is not yet implemented.");
            return new Temperature(t.Value / t2.Value, t.Scale);
        }

        public static Temperature operator /(Temperature t, double d)
        {
            return new Temperature(t.Value / d, t.Scale);
        }

        #endregion

        #region Multiplication

        public static Temperature operator *(Temperature t, Temperature t2)
        {
            if (t.Scale != t2.Scale)
                throw new NotSupportedException("Performing math on temperatures in different scales is not yet implemented.");
            return new Temperature(t.Value * t2.Value, t.Scale);
        }

        public static Temperature operator *(Temperature t, double d)
        {
            return new Temperature(t.Value * d, t.Scale);
        }

        #endregion

        #region Addition

        public static Temperature operator +(Temperature t, Temperature t2)
        {
            if (t.Scale != t2.Scale)
                throw new NotSupportedException("Performing math on temperatures in different scales is not yet implemented.");
            return new Temperature(t.Value + t2.Value, t.Scale);
        }

        public static Temperature operator +(Temperature t, double d)
        {
            return new Temperature(t.Value + d, t.Scale);
        }

        #endregion

        #region Subtraction

        public static Temperature operator -(Temperature t, Temperature t2)
        {
            if (t.Scale != t2.Scale)
                throw new NotSupportedException("Performing math on temperatures in different scales is not yet implemented.");
            return new Temperature(t.Value - t2.Value, t.Scale);
        }

        public static Temperature operator -(Temperature t, double d)
        {
            return new Temperature(t.Value - d, t.Scale);
        }

        #endregion

        #endregion
    }

    public class TemperatureF : Temperature
    {
        public TemperatureF(double value)
            : base(value, TemperatureScale.F)
        {

        }

        public static readonly TemperatureF AbsoluteZero = new TemperatureF(-459.67);
    }

    public class TemperatureC : Temperature
    {
        public TemperatureC(double value)
            : base(value, TemperatureScale.C)
        {

        }
    }

    public class TemperatureK : Temperature
    {
        public TemperatureK(double value)
            : base(value, TemperatureScale.K)
        {

        }
    }
}
