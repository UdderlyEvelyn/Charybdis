using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.Library.Core
{
    /// <summary>
    /// This class encapsulates extensions and functions (including LINQ extensions) that operate on numerical datatypes only.
    /// </summary>
    public static class Mathematics
    {
        internal const int DEFAULT_ROUNDING_PLACES = 2;

        /// <summary>
        /// Compares two doubles after converting them to decimal format. 
        /// This may sound wasteful, but there are precision caveats in floating point numbers and decimal numbers don't have this issue.
        /// Use with caution, as there will be a performance penalty.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <example>
        /// You are comparing 1/3 to a value of 1/3 that was calculated in some other way.
        /// <code> 
        /// double oneThirdToFifteenDigits = .333333333333333d;
        /// double oneThirdCalculated = 1d / 3d;
        /// bool regularResult = oneThirdToFifteenDigits == oneThirdCalculated;
        /// bool decimalComparisonResult = Mathematics.CompareDoublesAsDecimals(oneThirdToFifteenDigits, oneThirdCalculated);
        /// </code>
        /// regularResult will be false, while decimalComparisonResult will be true. This is not the only use case, just an example.
        /// </example>
        public static bool CompareDoublesAsDecimals(double a, double b)
        {
            return (decimal)a == (decimal)b;
        }

        /// <summary>
        /// Compares two doubles after converting them to ulongs.
        /// This may sound wasteful, but there are precision caveats in floating point numbers and putting them into ulong format 
        /// effectively compares the bytes raw as ulong and double are the same size but ulong is not a floating point nor signed.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <example>
        /// You are comparing 1/3 to a value of 1/3 that was calculated in some other way.
        /// <code> 
        /// double oneThirdToFifteenDigits = .333333333333333d;
        /// double oneThirdCalculated = 1d / 3d;
        /// bool regularResult = oneThirdToFifteenDigits == oneThirdCalculated;
        /// bool uLongComparisonResult = Mathematics.CompareDoublesAsUnsignedLongs(oneThirdToFifteenDigits, oneThirdCalculated);
        /// </code>
        /// regularResult will be false, while uLongComparisonResult will be true. This is not the only use case, just an example.
        /// </example>
        public static bool CompareDoublesAsUnsignedLongs(double a, double b)
        {
            return (ulong)a == (ulong)b;
        }

        /// <summary>
        /// Calculates the percentage difference between two values.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="roundingType">the type of rounding to use (if any)</param>
        /// <param name="places">the number of places to preserve during rounding</param>
        /// <returns></returns>
        /// <example>
        /// You want to display the percentage difference between two values to a user:
        /// <code>
        /// double value1 = 400;
        /// double value2 = 421;
        /// Console.WriteLine(value1 + " and " + value2 + " differ by " + value1.PercentageDifference(value2, RoundingType.Traditional) + "%.");
        /// </code>
        /// The line it outputs would be "400 and 421 differ by 5.12%."
        /// </example>
        public static double PercentageDifference(this double x, double y, RoundingType roundingType = RoundingType.None, int places = DEFAULT_ROUNDING_PLACES)
        {
            double result = (Math.Abs(x - y) / ((x + y) / 2)) * 100;
            if (roundingType != RoundingType.None)
                return Round(result, roundingType, places);
            else 
                return result;
        }

        /// <summary>
        /// Calculates and returns the deviations from a specified target value. If a value is not specified, it calculates the mean and uses that.
        /// </summary>
        /// <param name="data">The data to calculate the deviations of.</param>
        /// <param name="target">The target value to calculate the deviations from (if not provided, the mean is calculated and used).</param>
        /// <param name="absolute">If true, results are run through Math.Abs to get absolute deviations.</param>
        /// <param name="roundingType">the type of rounding to use (if any)</param>
        /// <param name="places">the number of places to preserve during rounding</param>
        /// <returns></returns>
        /// <example>
        /// You've got some data and you want to output a text table of the data with its deviation from the mean to the console:
        /// <code>
        /// var data = new double[] { 0, 1, 5, 3, 10, 9, 7, 6 };
        /// var deviations = data.Deviations();
        /// string[] deviationStrings = new string[data.Length];
        /// for (int i = 0; i < data.Length; i++)
        /// {
        ///     deviationStrings[i] = data[i] + ":" + deviations.ElementAt(i);
        /// }
        /// Console.WriteLine(deviationStrings.Join(", "));
        /// </code>
        /// The last line would print out "0:5.125, 1:4.125, 5:0.125, 3:2.125, 10:4.875, 9:3.875, 7:1.875, 6:0.875"
        /// </example>
        public static IEnumerable<double> Deviations(this IEnumerable<double> data, double target = double.NaN, bool absolute = true, RoundingType roundingType = RoundingType.None, int places = DEFAULT_ROUNDING_PLACES)
        {
            double mean = double.IsNaN(target) ? data.Sum() / data.Count() : target; //Use target if specified, otherwise calculate mean of the data.
            IEnumerable<double> deviations;
            if (roundingType != RoundingType.None)
            {
                if (absolute)
                    deviations = data.Select(n => Round(Math.Abs(n - mean), roundingType, places));
                else
                    deviations = data.Select(n => Round(n - mean, roundingType, places));
            }
            else
            {
                if (absolute)
                    deviations = data.Select(n => Math.Abs(n - mean));
                else
                    deviations = data.Select(n => n - mean);
            }
            return deviations;
        }

        /// <summary>
        /// Calculates the standard deviation of an IEnumerable of doubles.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mean">if you've already calculated the mean for other purposes, feed it in here to save resources</param>
        /// <param name="count">if you've already got the item count, feed it in here to save resources</param>
        /// <param name="roundingType">the type of rounding to use (if any)</param>
        /// <param name="places">the number of places to preserve during rounding</param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// var data = new double[] { 0, 1, 5, 3, 10, 9, 7, 6 };
        /// double stddev = data.StandardDeviation(roundingType: RoundingType.Traditional);
        /// </code>
        /// stddev would be 3.37 after this example.
        /// </example>
        public static double StandardDeviation(this IEnumerable<double> data, double mean = double.NaN, double count = double.NaN, RoundingType roundingType = RoundingType.None, int places = DEFAULT_ROUNDING_PLACES)
        {
            //Calculate data count if not provided.
            if (double.IsNaN(count)) count = data.Count();
            //Calculate mean if not provided.
            if (double.IsNaN(mean)) mean = data.Sum() / count;
            //Calculate deviations from mean, sum them, and divide by count to get variance.
            double v = data.Sum(x =>
                {
                    double d = x - mean; //Deviation of this value.
                    return d * d; //Square the deviation and put this value in the list.
                }
                ) / count; //Get the mean of the squared deviations.
            //Return standard deviation, the square root of the variance.
            if (roundingType != RoundingType.None)
                return Round(Math.Sqrt(v), roundingType, places);
            else 
                return Math.Sqrt(v);
        }

        /// <summary>
        /// Gets the nearest order of magnitude to the integer value.
        /// </summary>
        /// <param name="i"></param>
        /// <example>
        /// What order of magnitude is 341?
        /// <c>int result = 341.GetOrderOfMagnitude();</c>
        /// Result would be equal to 100.
        /// </example>
        /// <returns></returns>
        public static int GetOrderOfMagnitude(this int i, int places = DEFAULT_ROUNDING_PLACES)
        {
            int digitLength = i.GetDigitLength();
            int paddedPlaces = places + 1;
            int pow = digitLength - paddedPlaces;
            if (pow <= 0)
                return 1;
            return (int)Math.Pow(10, pow);
        }

        #region Averages

        public static double Median(this IEnumerable<double> data)
        {
            return data.Sort().ElementAt((int)((data.Count() / 2d).Round(places: 0)));
        }

        public static double Median<T>(this IEnumerable<T> data, Func<T, double> selector)
        {
            return data.Select(selector).Sort().ElementAt((int)((data.Count() / 2d).Round(places: 0)));
        }

        #endregion

        #region Rounding

        /// <summary>
        /// Rounds a number to the specified number of places with the specified type of rounding.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="places">the number of decimals to preserve</param>
        /// <param name="rt">the type of rounding to apply</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <example>
        /// You want to round a number towards zero, preserving the first three decimal places:
        /// <c>double result = -1.5395888d.Round(3, RoundingType.TowardsZero);</c>
        /// After this example, result would hold -1.539d.
        /// 
        /// Since this method has so many options, here's a more traditional example.
        /// 
        /// You want to round a number to the first two decimal places using the traditional rules:
        /// <c>double result = 1.0051d.Round(2, RoundingType.Traditional);</c>
        /// After this example, result would hold 1.01d.
        /// </example>
        public static double Round(this double d, RoundingType roundingType = RoundingType.Traditional, int places = DEFAULT_ROUNDING_PLACES)
        {
            switch (roundingType)
            {
                //This case is included to enable you to pass every call to a function with a rounding
                //option into the Round method without having to detect whether it was set to "None".
                case RoundingType.None: 
                    return d;
                case RoundingType.Banker:
                    return Math.Round(d, places, MidpointRounding.ToEven);
                case RoundingType.Ceiling:
                    return Ceiling(d, places);
                case RoundingType.Floor:
                    return Floor(d, places);
                case RoundingType.Traditional:
                    return Math.Round(d, places);
                case RoundingType.AwayFromZero:
                    return Math.Round(d, places, MidpointRounding.AwayFromZero);
                case RoundingType.Truncate:
                case RoundingType.TowardsZero:
                    double preservationFactor = Math.Pow(10, places);
                    return Math.Truncate(d * preservationFactor) / preservationFactor;
                case RoundingType.OrderOfMagnitude:
                    int integerValue = (int)d;
                    int oom = integerValue.GetOrderOfMagnitude(places);
                    return integerValue / oom * oom;
                default:
                    throw new ArgumentException("The rounding type is invalid.", "roundingType");
            }
        }

        /// <summary>
        /// Round up to the specified number of decimal places.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="places">the number of decimal places in the result</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidCastException"/>
        /// <example>
        /// You want to round a number up to the nearest first decimal place:
        /// <c>decimal result = 1.55M.Ceiling(1);</c>
        /// After the example, result would be equal to 1.6M.
        /// </example>
        public static decimal Ceiling(this decimal d, int places = DEFAULT_ROUNDING_PLACES)
        {
            decimal preservationFactor = (decimal)Math.Pow(10, places);
            return Math.Ceiling(d * preservationFactor) / preservationFactor;
        }

        /// <summary>
        /// Round down to the specified number of decimal places.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="places">the number of decimal places in the result</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidCastException"/>
        /// <example>
        /// You want to round a number down to the nearest first decimal place:
        /// <c>decimal result = 1.55M.Floor(1);</c>
        /// After the example, result would be equal to 1.5M.
        /// </example>
        public static decimal Floor(this decimal d, int places = DEFAULT_ROUNDING_PLACES)
        {
            decimal preservationFactor = (decimal)Math.Pow(10, places);
            return Math.Floor(d * preservationFactor) / preservationFactor;
        }

        /// <summary>
        /// Round up to the specified number of decimal places.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="places">the number of decimal places in the result</param>
        /// <returns></returns>
        /// <example>
        /// You want to round a number up to the nearest first decimal place:
        /// <c>double result = 1.55d.Ceiling(1);</c>
        /// After the example, result would be equal to 1.6d.
        /// </example>
        public static double Ceiling(this double d, int places = DEFAULT_ROUNDING_PLACES)
        {
            double preservationFactor = Math.Pow(10, places);
            return Math.Ceiling(d * preservationFactor) / preservationFactor;
        }

        /// <summary>
        /// Round down to the specified number of decimal places.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="places">the number of decimal places in the result</param>
        /// <returns></returns>
        /// <example>
        /// You want to round a number down to the nearest first decimal place:
        /// <c>double result = 1.55d.Floor(1);</c>
        /// After the example, result would be equal to 1.5d.
        /// </example>
        public static double Floor(this double d, int places = DEFAULT_ROUNDING_PLACES)
        {
            double preservationFactor = Math.Pow(10, places);
            return Math.Floor(d * preservationFactor) / preservationFactor;
        }
    }

    /// <summary>
    /// Types of rounding that can be performed.
    /// </summary>
    public enum RoundingType
    {
        /// <summary>
        /// Do not round.
        /// </summary>
        None,
        /// <summary>
        /// Always rounds down.
        /// </summary>
        Floor,
        /// <summary>
        /// Always rounds up.
        /// </summary>
        Ceiling,
        /// <summary>
        /// Rounds away from zero in the midpoint case.
        /// </summary>
        AwayFromZero,
        /// <summary>
        /// Rounds toward zero in the midpoint case.
        /// </summary>
        TowardsZero,
        /// <summary>
        /// Rounds toward the nearest integer, rounding up in the midpoint case.
        /// </summary>
        Traditional,
        /// <summary>
        /// Rounds toward the nearest integer, rounding toward the nearest even integer in the midpoint case.
        /// </summary>
        Banker,
        /// <summary>
        /// Preserves the decimals you specify and discards the rest.
        /// </summary>
        Truncate,
        /// <summary>
        /// Truncates to the nearest interval of the value's order of magnitude.
        /// </summary>
        OrderOfMagnitude,
    }

    #endregion
}
