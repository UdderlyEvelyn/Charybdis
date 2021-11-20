using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Charybdis.Library.Core
{
    /// <summary>
    /// Contains static convenience functions to assist in writing validation methods.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Returns whether two sequences have the same set of values in any order (using the default comparer), ignoring the provided items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <param name="ignore"></param>
        /// <returns></returns>
        public static bool SequenceEqualIgnoring<T>(this IEnumerable<T> e1, IEnumerable<T> e2, params T[] ignore)
        {
            return e1.Except(ignore).SequenceEqual(e2.Except(ignore));
        }

        /// <summary>
        /// Returns wheteher all items in the second list are contained in the first list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        public static bool ContainsAll<T>(this List<T> container, List<T> contents)
        {
            return !contents.Except(container).Any(); //Are there any items in a list of the contents after any items in container have been removed? Return inverse.
        }

        /// <summary>
        /// Returns whether the two lists share any items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        public static bool ContainsAny<T>(this List<T> container, List<T> contents)
        {
            return contents.Any(t => container.Contains(t));
        }

        //Need to verify the directionality of -1/1, pretty sure it should be right, though. -JDS 3/23/17
        public static int CompareTo<T>(this T? t1, T? t2)
            where T : struct, IComparable
        {
            if (t1.HasValue && t2.HasValue) //Both have a value, compare and return result.
                return t1.Value.CompareTo(t2.Value);
            else if (t1.HasValue) //Only the first one has a value, return relevant result.
                return -1;
            else if (t2.HasValue) //Only the second one has a value, return relevant result.
                return 1;
            else //Neither have a value, so they are equal.
                return 0;
        }

        //Need to verify the directionality of -1/1, pretty sure it should be right, though. -JDS 3/23/17
        public static int CompareTo<T>(this T? t1, T t2)
            where T : struct, IComparable
        {
            if (t1.HasValue) //First one has a value, so compare and return result.
                return t1.Value.CompareTo(t2);
            else //First one is null, so return relevant result.
                return 1;
        }

        /// <summary>
        /// Returns whether a nullable boolean has a value set to true.
        /// </summary>
        /// <param name="b">the nullable boolean to check</param>
        /// <returns>whether a nullable boolean has either a value of false or no value set</returns>
        public static bool HasValueOfTrue(this bool? b)
        {
            return b.HasValue && b.Value;
        }

        /// <summary>
        /// Returns whether a nullable boolean has a value set to false.
        /// </summary>
        /// <param name="b">the nullable boolean to check</param>
        /// <returns>whether a nullable boolean has either a value of false or no value set</returns>
        public static bool HasValueOfFalse(this bool? b)
        {
            return b.HasValue && !b.Value;
        }

        /// <summary>
        /// Returns whether a nullable boolean has either a value of false or no value set.
        /// </summary>
        /// <param name="b">the nullable boolean to check</param>
        /// <returns>whether a nullable boolean has either a value of false or no value set</returns>
        public static bool HasNoValueOrFalse(this bool? b)
        {
            return !b.HasValue || !b.Value;
        }

        /// <summary>
        /// Returns whether the input string contains only digits.
        /// </summary>
        /// <param name="s">the string to analyze</param>
        /// <returns>whether the string contains only digits</returns>
        public static bool IsDigits(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false; //blank/null strings aren't digits
            return s.All(c => char.IsDigit(c));
        }

        /// <summary>
        /// Returns whether the input string contains only alphanumeric characters (according to Unicode, so non-English letters will pass this test!).
        /// </summary>
        /// <param name="s">the string to analyze</param>
        /// <returns>whether the string contains only alphanumeric characters</returns>
        public static bool IsAlphanumeric(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false; //blank/null strings aren't alphanumeric
            return s.All(c => char.IsLetterOrDigit(c));
        }

        /// <summary>
        /// Returns whether the input string contains only letter characters (according to Unicode, so non-English letters will pass this test!).
        /// </summary>
        /// <param name="s">the string to analyze</param>
        /// <returns>whether the string contains only letters</returns>
        public static bool IsLetters(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false; //blank/null strings aren't letters
            return s.All(c => char.IsLetter(c));
        }

        /// <summary>
        /// Returns whether the input string contains only digit characters or dash characters.
        /// </summary>
        /// <param name="s">the string to analyze</param>
        /// <returns>whether the string contains only digits or dashes</returns>
        public static bool IsDigitsOrDashes(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false; //blank/null strings aren't digits or dashes
            return !s.Any(c => !(char.IsDigit(c) || c == '-'));
        }

        /// <summary>
        /// Returns whether the input string contains only digit characters and precisely one dash character.
        /// </summary>
        /// <param name="s">the string to analyze</param>
        /// <returns>whether the string contains only digits and precisely one dash character</returns>
        public static bool IsDigitsWithOneDash(this string s)
        {
            return s.IsDigitsOrDashes() && s.Count(c => c == '-') == 1;
        }

        /// <summary>
        /// Returns whether the input string contains only digit characters and up to one dash character.
        /// </summary>
        /// <param name="s">the string to analyze</param>
        /// <returns>whether the string contains only digits and up to one dash character</returns>
        public static bool IsDigitsWithOptionalDash(this string s)
        {
            return s.IsDigitsOrDashes() && s.Count(c => c == '-') <= 1;
        }

        /// <summary>
        /// Returns whether the input string contains a valid decimal number (all digits with an optional single decimal point).
        /// </summary>
        /// <param name="s">the string to analyze</param>
        /// <returns>whether the string contains only digits and an optional single decimal point</returns>
        public static bool IsValidDecimalNumber(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false; //blank/null strings aren't valid decimal numbers
            return !s.Any(c => !(char.IsDigit(c) || c == '.')) && !(s.Count(c => c == '.') > 1);
        }

        /// <summary>
        /// Returns whether a string has content (non-null, and optionally non-whitespace).
        /// </summary>
        /// <param name="s"></param>
        /// <param name="countWhitespaceAsContent">if true, count whitespace as content (defaults to false)</param>
        /// <returns></returns>
        public static bool HasContent(this string s, bool countWhitespaceAsContent = false)
        {
            //Return the negation of a null-or-whitespace check, or if countWhitespaceAsContent is true, the negation of a null-or-empty check.
            return !(countWhitespaceAsContent ? s.IsNullOrEmpty() : s.IsNullOrWhitespace());
        }

        /// <summary>
        /// Returns whether a string is blank (non-null, containing only whitespace if anything).
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsBlank(this string s)
        {
            return s.IsEmpty() || s.All(c => char.IsWhiteSpace(c));
        }

        /// <summary>
        /// Returns whether a string contains no characters (zero-length).
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string s)
        {
            return s.Length == 0;
        }


        /// <summary>
        /// Convenience wrapper for string.IsNullOrEmpty as a string extension.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Convenience wrapper for string.IsNullOrWhiteSpace as a string extension.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrWhitespace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }        

        /// <summary>
        /// Convenience wrapper for char.IsWhiteSpace as a char extension.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsWhitespace(this char c)
        {
            return char.IsWhiteSpace(c);
        }
        
        /// <summary>
        /// Returns true when a value (x) is between the lower and upper bounds.
        /// </summary>
        /// <param name="x">The value to analyze.</param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <param name="inclusive">If true, allows the value to be equal to one of the bounds and pass (defaults to true).</param>
        /// <returns></returns>
        public static bool IsBetween<T>(this T x, T lowerBound, T upperBound, bool inclusive = true) where T : IComparable<T>
        {
            if (inclusive) return x.CompareTo(lowerBound) >= 0 && x.CompareTo(upperBound) <= 0;
            else return x.CompareTo(lowerBound) > 0 && x.CompareTo(upperBound) < 0;
        }

        /// <summary>
        /// Returns true if the IEnumerable contains no elements or is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> e)
        {
            return e == null || !e.Any();
        }

        /// <summary>
        /// Returns true if the source string contains (or consists entirely of) the target string, with a StringComparison parameter to take into account.
        /// </summary>
        /// <param name="s">the source string</param>
        /// <param name="target">the target string to find</param>
        /// <param name="comparisonType">comparison type when searching for the target in the source</param>
        /// <returns></returns>
        public static bool Contains(this string s, string target, StringComparison comparisonType)
        {
            return s.IndexOf(target, comparisonType) >= 0;
        }

        /// <summary>
        /// Returns true if the source string contains (or consists entirely of) the target string.
        /// </summary>
        /// <param name="s">the source string</param>
        /// <param name="target">the target string to find</param>
        /// <returns></returns>
        public static bool Contains(this string s, string target)
        {
            return s.IndexOf(target) >= 0;
        }

        /// <summary>
        /// Returns true if the char is an uppercase letter (convenience wrapper for char.IsUpper).
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsUpper(this char c)
        {
            return char.IsUpper(c);
        }

        /// <summary>
        /// Returns true if the char is an lowercase letter (convenience wrapper for char.IsLower).
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsLower(this char c)
        {
            return char.IsLower(c);
        }

        /// <summary>
        /// Returns true if the char is a letter (convenience wrapper for char.IsLetter).
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsLetter(this char c)
        {
            return char.IsLetter(c);
        }

        /// <summary>
        /// Returns true if the char is alphanumeric (convenience wrapper for char.IsAlphanumeric).
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsAlphanumeric(this char c)
        {
            return char.IsLetterOrDigit(c);
        }

        /// <summary>
        /// Returns true if the char is a digigt (convenience wrapper for char.IsDigit).
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsDigit(this char c)
        {
            return char.IsDigit(c);
        }

        
        /// <summary>
        /// Returns true if the char is an English-language vowel, optionally including 'y'.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="includeY"></param>
        /// <returns></returns>
        //For performance purposes it should be faster to do this than to do something like:
        //char c = 'a'; 
        //bool result = Data.Vowels.Contains(c);
        public static bool IsVowel(this char c, bool includeY = true)
        {
            switch (c)
            {
                case 'a':
                case 'A':
                case 'e':
                case 'E':
                case 'i':
                case 'I':
                case 'o':
                case 'O':
                case 'u':
                case 'U':
                    return true;
                case 'y':
                case 'Y':
                    return includeY;
                default:
                    return false;
            }
        }
    }
}
