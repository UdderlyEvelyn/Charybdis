using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace Charybdis.Library.Core
{
    /// <summary>
    /// Contains functions for processing data into other types of data, or performing generic/common actions on data - includes many convenience wrappers to make code more readable.
    /// </summary>
    public static class Processing
    {
        /// <summary>
        /// Represents a range of integers and provides methods for working with them.
        /// </summary>
        public class Range
        {
            public int Start { get; set; }
            public int End { get; set; }
            public int Length { get; protected set; }

            public Range(int start, int end)
            {
                Start = start;
                End = end;
                Length = end - start;
            }

            public bool Contains(int i)
            {
                return i >= Start && i <= End;
            }
        }

        /// <summary>
        /// Represents a set of ranges of characters assigned to ranges of integers.
        /// </summary>
        public class CharacterRangeSet : Dictionary<Range, ICharacterRange>, IDictionary<Range, ICharacterRange>
        {
            public string GetNext(string s)
            {
                var chars = s.ToCharArray();
                for (int i = s.Length - 1; i >= 0; i--)
                {
                    var cr = this.Single(ra => ra.Key.Contains(i)).Value;
                    if (chars[i] == cr.End)
                        chars[i] = cr.Start;
                    else
                    {
                        chars[i] = cr.GetNext(chars[i]);
                        return chars.AsString();
                    }
                }
                return ""; //No character to increment was found, return blank.
            }
        }

        /// <summary>
        /// The common interface for a range of characters.
        /// </summary>
        public interface ICharacterRange
        {
            char Start { get; set; }
            char End { get; set; }
            int Length { get; }
            char GetNext(char c);
        }

        /// <summary>
        /// A range of characters that fall in numerical order when incremented/decremented.
        /// </summary>
        public class ContiguousCharacterRange : ICharacterRange
        {
            public char Start { get; set; }
            public char End { get; set; }
            public int Length { get; protected set; }
            public char GetNext(char c)
            {
                c++; //Get next char.
                if (c > End) //If next char is out of bounds.
                    throw new ArgumentOutOfRangeException("No more characters left."); //ABORT!
                return c; //Return next char.
            }

            public ContiguousCharacterRange(char cStart, char cEnd)
            {
                Start = cStart;
                End = cEnd;
                Length = End - Start;
            }
        }

        /// <summary>
        /// A range of characters in a specified order with no necessary pre-existing relationship (use ContiguousCharacterRange if possible, as it will be faster and more efficient).
        /// </summary>
        public class CustomCharacterRange : ICharacterRange
        {
            public char Start { get; set; }
            public char End { get; set; }
            public int Length { get; protected set; }
            public char GetNext(char c)
            {
                int i = Array.IndexOf(_chars, c); //Get index of current char.
                i++; //Go to next.
                if (i >= Length) //If it's out of bounds.
                    throw new ArgumentOutOfRangeException("No more characters left."); //ABORT!
                return _chars[i]; //Return next char.
            }

            protected char[] _chars;

            public CustomCharacterRange(char[] chars)
            {
                Start = chars[0];
                Length = chars.Length;
                End = chars[Length - 1];
                _chars = chars;
            }
        }

        /// <summary>
        /// Returns the value from the dictionary if the key is present, otherwise returns null.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static V GetClassOrNull<K, V>(this Dictionary<K, V> dictionary, K key)
            where V : class
        {
            V result;
            if (dictionary.TryGetValue(key, out result))
                return result;
            else
                return null;
        }

        /// <summary>
        /// Returns the value from the dictionary if the key is present, otherwise returns null (returns a nullable form of the value type).
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static V? GetStructOrNull<K, V>(this Dictionary<K, V> dictionary, K key)
            where V : struct
        {
            V result;
            if (dictionary.TryGetValue(key, out result))
                return result;
            else
                return null;
        }

        /// <summary>
        /// Replace capitalization variations of the first string with correspoinding variations of the second string, returning the results.
        /// </summary>
        /// <param name="s">the string to replace within</param>
        /// <param name="s1">the string to replace</param>
        /// <param name="s2">the string to replace with</param>
        /// <returns></returns>
        public static string ReplaceCapitalizationVariants(this string s, string s1, string s2)
        {
            var tmp = s.Replace(s1, s2);
            tmp = tmp.Replace(s1.ToLower(), s2.ToLower());
            tmp = tmp.Replace(s1.ToUpper(), s2.ToUpper());
            return tmp.Replace(s1.Capitalize(), s2.Capitalize());
        }

        /// <summary>
        /// Return the string with the first letter as upper case and the rest in lower case.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Capitalize(this string s)
        {
            if (s == null) //If it's null, we can't operate on it..
                return null; //So return null.
            if (s.Length > 1) //If it's more than one in length..
                return s[0].ToUpper() + s.Substring(1).ToLower(); //Return the first char as upper, the rest as lower.
            return s[0].ToUpper().ToString(); //The only option left is single character, so return it as uppercase.
        }

        public static string ToTitleCase(this string s)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s);
        }

        /// <summary>
        /// Strip all occurrences of a character from a string.
        /// </summary>
        /// <param name="s">the string to operate on</param>
        /// <param name="c">the character to strip</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <example>
        /// Useful for sanitizing input/output.
        /// A phone number, suppose we need it without dashes since dashes are invalid where we're sending it.
        /// <c>"111-111-1111".Strip('-');</c>
        /// This would return "111111111".
        /// </example>
        public static string Strip(this string s, char c)
        {
            //We know that the original string's length is as long as it could be, so initialize it at that length to avoid resizing the internal buffer.
            return new StringBuilder(s.Length).Do(s, (sb, ch) => { if (ch != c) sb.Append(ch); }).ToString();
        }

        /// <summary>
        /// Strip all occurrences of any character in the array from a string.
        /// </summary>
        /// <param name="s">the string to operate on</param>
        /// <param name="cs">an array of characters to strip</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <example>
        /// Useful for sanitizing input or output:
        /// <c>"***Da&ta^F%ro@mAU!ser$".Strip(new char[] { '!', '@', '#', '$', '%', '^', '&', '*' });</c>
        /// This would return "DataFromAUser". 
        /// </example>
        public static string Strip(this string s, char[] cs)
        {
            //We know that the original string's length is as long as it could be, so initialize it at that length to avoid resizing the internal buffer.
            return new StringBuilder(s.Length).Do(s, (sb, ch) => { if (!cs.Contains(ch)) sb.Append(ch); }).ToString();
        }

        /// <summary>
        /// Split a string using another string as the delimiter.
        /// </summary>
        /// <param name="s">the string to split</param>
        /// <param name="str">the string to split by</param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"/>
        /// <example>
        /// Normally this functionality is only possible in C# with an array of strings for some reason, 
        /// so providing this method reduces the amount of code that needs to be written in the (very common)
        /// case like this example:
        /// <c>"Smith, John".Split(", ");</c>
        /// This would return a string[] of { "Smith", "John" }
        /// </example>
        public static string[] Split(this string s, string str, StringSplitOptions options = StringSplitOptions.None)
        {
            return s.Split(new string[] { str }, options);
        }

        /// <summary>
        /// Split a string using a single char as the delimiter.
        /// </summary>
        /// <param name="s">the string to split</param>
        /// <param name="c">the char to split by</param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"/>
        /// <example>
        /// Normally this functionality is only possible in C# with an array of strings for some reason, 
        /// so providing this method reduces the amount of code that needs to be written in the (very common)
        /// case like this example:
        /// <c>"Smith,John".Split(',');</c>
        /// This would return a string[] of { "Smith", "John" }
        /// </example>
        public static string[] Split(this string s, char c, StringSplitOptions options = StringSplitOptions.None)
        {
            return s.Split(new char[] { c }, options);
        }

        /// <summary>
        /// Returns the contant of the string from between the left and right substrings.
        /// </summary>
        /// <param name="s">the string to extract from</param>
        /// <param name="left">the left substring</param>
        /// <param name="right">the right substring</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.IndexOutOfRangeException"/>
        /// <example>
        /// This retrieves the text between two specific strings, useful for parsing XML/HTML or other data with known start/end tags.
        /// <c>"[TAG]DataDataData[/TAG]".Between("[TAG]", "[/TAG]");</c>
        /// The above would return "DataDataData" - obviously a simplistic example.
        /// Normally you'd be operating on a string variable from somewhere outside the code.
        /// </example>
        public static string Between(this string s, string left, string right, SplitMode splitMode = SplitMode.LastInstance)
        {
            switch (splitMode)
            {
                case SplitMode.LastInstance:
                    int endOfLeftToken = s.IndexOf(left) + left.Length;
                    int startOfRightToken = s.LastIndexOf(right) - endOfLeftToken;
                    return s.Substring(endOfLeftToken, startOfRightToken);
                case SplitMode.FirstInstance:
                    return s.Split(left)[1].Split(right)[0]; //Only accounts for first instance of right token, should pull from last instance.
                default:
                    throw new ArgumentException("Unrecognized SplitMode value.", "splitMode");
            }
        }

        public enum SplitMode
        {
            FirstInstance,
            LastInstance,
        }

        /// <summary>
        /// Returns the content of the string from between two instances of a substring.
        /// </summary>
        /// <param name="s">the string to extract from</param>
        /// <param name="separator">the substring to use as a delimiter</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.IndexOutOfRangeException"/>
        /// <example>
        /// This retrieves the text between two instances of the same string.
        /// <c>"JJJJThis is a test.JJJJ".Between("JJJJ");</c>
        /// This would return "This is a test." - this can be useful for parsing.
        /// Normally you'd be operating on a string variable from somewhere outside the code.
        /// </example>
        public static string Between(this string s, string separator)
        {
            return s.Between(separator, separator);
        }

        /// <summary>
        /// Returns the integer value of a month, given the English title-case name of the month as a string.
        /// </summary>
        /// <param name="monthString">title-case English name of the month</param>
        /// <returns>integer value of the given month</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <example>
        /// Suppose you have text input from a user that's in word form, and you need a number:
        /// <c>Processing.ParseMonth("July");</c>
        /// The above would return 7 (int).
        /// </example>
        public static int ParseMonth(string monthString)
        {
            int result = Data.Months.IndexOf(monthString);
            //Normally bad to throw exceptions in small routines like this one but Microsoft is using a magic return value for it (bad practice) so we have to compensate.
            if (result == -1) throw new ArgumentOutOfRangeException("monthString", "The provided month string was invalid.");
            else return result;
        }

        /// <summary>
        /// Returns the title-case English name of the month, given its month number as an integer.
        /// </summary>
        /// <param name="monthInt">an integer representing the month in question</param>
        /// <returns>the title-case English name of the month</returns>
        /// <exception cref="System.ArgumentxOutOfRangeException"/>
        /// <example>
        /// Suppose we've sourced an integer that we know is supposed to represent a month, but we want the string representation.
        /// <c>Processing.MonthString(7);</c>
        /// This would return "July".
        /// </example>
        public static string MonthString(int monthInt)
        {
            return Data.Months[monthInt];
        }

        /// <summary>
        /// Runs a function on a string as though it were another type, returning the result as a string (with optional format).
        /// </summary>
        /// <typeparam name="T">the type to treat the string as</typeparam>
        /// <param name="s">the string to perform the function on</param>
        /// <param name="transform">the function to use to modify the value as the passed-in type</param>
        /// <param name="format">the format to return the value in</param>
        /// <returns>string representing the original value passed through the transform function in the provided format</returns>
        /// <exception cref="Microsoft.CSharp.RuntimeBinder.RuntimeBinderException"/>
        /// <exception cref="System.InvalidCastException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="OverflowException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <example>
        /// Increment sample (a string) as an integer and then convert back for use as a formatted string:
        /// <c>Sample = Sample.As&lt;int&gt;(x => ++x, "000");</c>
        /// If sample was "004" it would now be "005", etc..
        /// </example>
        public static string As<T>(this string s, Func<T, T> transform, string format = null)
        {
            T result = transform((T)Convert.ChangeType(s, typeof(T)));
            return format == null ? result.ToString() : string.Format("{0:" + format + "}", result);
        }

        /// <summary>
        /// Convenience wrapper to create a StringBuilder initialized from a string in-line.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <example>
        /// This is syntactic sugar. Instead of needing to use <c>new StringBuilder(myStringVariable)</c>
        /// you can do <c>myStringVariable.Builder()</c> - this is similar in concept to LINQ's fluent syntax,
        /// allowing things like "Hello".Builder().Append(" my name is ").Append(name).ToString();
        /// If "name" was set to "Jim", this would return "Hello my name is Jim".
        /// </example>
        public static StringBuilder Builder(this string s)
        {
            return new StringBuilder(s);
        }

        /// <summary>
        /// Returns a range of integers from the first value to the second value (inclusive) with an optional interval.
        /// </summary>
        /// <param name="from">the value to start the range from</param>
        /// <param name="to">the value to end the range at</param>
        /// <param name="interval">the step to increment by when generating a new value in the range (defaults to 1)</param>
        /// <returns>range of integers from the first value to the second value (inclusive)</returns>
        /// <exception cref="System.ArgumentException"/>
        /// <example>
        /// Let's say we want every 5th number from 0 to 25:
        /// <c>0.To(25, 5);</c>
        /// That would return an enumerable { 0, 5, 10, 15, 20, 25 };
        /// </example>
        public static IEnumerable<int> To(this int from, int to, int interval = 1)
        {
            //return Enumerable.Range(from, (to - from)); //We don't use this way because it doesn't support interval - performance is almost identical anyway.
            List<int> ints = new List<int> { from };
            if (from < to)
            {
                for (int i = from + interval; i <= to; i += interval)
                {
                    ints.Add(i);
                }
            }
            else throw new ArgumentException("The \"to\" value cannot be smaller or the equal to the \"from\" value."); //Can't define behavior when this case comes up, so throw an exception.
            return ints;
        }

        /// <summary>
        /// Returns a range of integers from the second value to the first value (inclusive) with an optional interval.
        /// </summary>
        /// <param name="to">the value to end the range at</param>
        /// <param name="from">the value to start the range from</param>
        /// <returns>range of integers from the second value to the first value (inclusive)</returns>
        /// <exception cref="System.ArgumentException"/>
        /// <example>
        /// This is just a convenience inverse of "To".
        /// Let's say we want a every 10 from 100 down to 0:
        /// <c>100.From(0, 10);</c>
        /// This would return { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
        /// </example>
        //Perhaps this should be redone to provide the numbers in the opposite order but otherwise function like "To"?
        public static IEnumerable<int> From(this int to, int from, int interval = 1)
        {
            return from.To(to, interval);
        }

        /// <summary>
        /// Converts an enumerable of chars into a string via StringBuilder.
        /// </summary>
        /// <param name="chars">the chars to concatenate</param>
        /// <returns>string that is a concatenation of the input chars</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.OverflowException"/>
        /// <example>
        /// In many data processing situations you end up having an enumerable of chars to manipulate, but you need to store the "finished product" as a string.
        /// new char[] { 'I', ' ', 'a', 'm', ' ', 'c', 'h', 'a', 'r', 's', '.' }.AsString();
        /// The above would return "I am chars." (normally you would not pass in a compile-time set of data, of course).
        /// </example>
        public static string AsString(this IEnumerable<char> chars)
        {
            return new StringBuilder(chars.Count()).AppendEach(chars).ToString();
        }

        /// <summary>
        /// Converts an array of bytes into a hexadecimal string via StringBuilder.
        /// </summary>
        /// <param name="bytes">the bytes to convert</param>
        /// <returns>representation of the given bytes in hexadecimal</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <exception cref="System.OverflowException"/>
        /// <example>
        /// If you want to view byte data in the typical readable format, you need it in hex:
        /// <c>new byte[] { 50, 0x10, 255, 0, 1, 2 }.ToHex();</c>
        /// This would return "3210FF000102". Normally you'd use this on bytes from a file or etc..
        /// </example>
        public static string ToHex(this byte[] bytes)
        {
            return new StringBuilder(bytes.Count() * 2).Do(bytes, (sb, b) => sb.Append(b.ToHex())).ToString();
        }

        /// <summary>
        /// Appends each char in the enumerable to the StringBuilder's content.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <example>
        /// This was mainly created to be used for AsString(), but could be used on its own when building a complex string with StringBuilder and you have an enumerable of chars:
        /// <c>new StringBuilder("SomeOtherExistingData, HereAreTheChars:").AppendEach(new char[] { 'a', 'b', 'c' }).ToString()</c>
        /// The above would return "SomeOtherExistingData, HereAreTheChars:abc".
        /// </example>
        public static StringBuilder AppendEach(this StringBuilder sb, IEnumerable<char> e)
        {
            e.Do<char>(x => sb.Append(x));
            return sb;
        }

        /// <summary>
        /// Appends each char[] in the enumerable to the StringBuilder's content.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <example>
        /// If you have a set of char arrays this will add the contents of each to the StringBuilder like so:
        /// <c>new StringBuilder("abc").AppendEach(new char[][] { new char[] { 'd', 'e', 'f' }, new char[] { 'g', 'h', 'i' } }).ToString()</c>
        /// The above would return "abcdefghi".
        /// </example>
        public static StringBuilder AppendEach(this StringBuilder sb, IEnumerable<char[]> e)
        {
            e.Do<char[]>(x => sb.Append(x));
            return sb;
        }

        /// <summary>
        /// Appends each string in the enumerable to the StringBuilder's content.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <example>
        /// In this example we're building an XML string from a list of tag/content strings:
        /// <code>
        /// string[] tags = new string[]
        /// {
        ///     "<xml>",
        ///     "<root>",
        ///     "<item>stuff</item>",
        ///     "</root>",
        ///     "</xml>",
        /// };
        /// new StringBuilder().AppendEach(tags).ToString();
        /// </code>
        /// The above would output "<xml><root><item>stuff</item></root></xml>".
        /// </example>
        public static StringBuilder AppendEach(this StringBuilder sb, IEnumerable<string> e)
        {
            e.Do<string>(x => sb.Append(x));
            return sb;
        }

        /// <summary>
        /// Appends each object in the enumerable to the StringBuilder's content in the given format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sb"></param>
        /// <param name="e"></param>
        /// <param name="format"></param>
        /// <param name="selectors">an optional params argument specifying selector functions to choose which members to pass into the formatting from the passed-in object type</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.FormatException"/>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <example>
        /// public struct Sandwich
        /// {
        ///     public string Name;
        ///     public decimal Price;
        /// }
        /// 
        /// Sandwich[] sandwichMenu = new Sandwich[] 
        /// { 
        ///     new Sandwich { Name = "Grilled Cheese", Price = 5M }, 
        ///     new Sandwich { Name = "Reuben", Price = 6.5M }, 
        ///     new Sandwich { Name = "Burger", Price = 3M } 
        /// };
        /// 
        /// new StringBuilder("Sandwich Menu:\n").AppendEachFormat(sandwichMenu, "{0}: {1:$0.00}\n", s => s.Name, s => s.Price).ToString()
        /// </code>
        /// The last line would return:
        /// @"Sandwich Menu:
        ///   Grilled Cheese: $5.00
        ///   Reuben: $6.50
        ///   Burger: $3.00"
        /// </example>
        public static StringBuilder AppendEachFormat<T>(this StringBuilder sb, IEnumerable<T> e, string format, params Func<T, object>[] selectors)
        {
            e.Do(t => sb.AppendFormat(format, selectors.Select(s => s(t)).ToArray()));
            return sb;
        }

        /// <summary>
        /// Returns a hexadecimal representation of a byte.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <example>
        /// <c>byte.MaxValue.ToHex();</c>
        /// The above would return "FF".
        /// </example>
        public static string ToHex(this byte b)
        {
            return b.ToString("X2");
        }

        /// <summary>
        /// Concatenates byte arrays into one byte array of their combined size.
        /// </summary>
        /// <param name="arrays"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <exception cref="System.OverflowException"/>
        /// <example>
        /// You're attaching several data buffers together to turn into hex and display to the user or store somewhere:
        /// <code>
        /// byte[] buffer1 = new byte[] { 0xFF, 0xAB, 0xDE, 0x55 };
        /// byte[] buffer2 = new byte[] { 0x00, 0x01, 0x8E, 0xF3 };
        /// byte[] buffer3 = new byte[] { 0x30, 0x75, 0x99, 0xCD };
        /// Processing.Concat(buffer1, buffer2, buffer3).ToHex();
        /// </code>
        /// The last line of code would return "FFABDE5500018EF3307599CD".
        /// </example>
        //This is the most efficient way possible to do this without hardcoding the number of arrays to remove the .Sum() call
        //by hardcoding each successive length and save on math in the offset calculations.
        public static byte[] Concat(params byte[][] arrays)
        {
            byte[] result = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                array.Transfer(result, 0, offset, array.Length);
                offset += array.Length;
            }
            return result;
        }        
        
        /// <summary>
        /// Concatenates byte arrays into one byte array of their combined size (extension variant).
        /// </summary>
        /// <param name="arrays"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <exception cref="System.OverflowException"/>
        /// <example>
        /// You're attaching several data buffers together to turn into hex and display to the user or store somewhere:
        /// <code>
        /// byte[] buffer1 = new byte[] { 0xFF, 0xAB, 0xDE, 0x55 };
        /// byte[] buffer2 = new byte[] { 0x00, 0x01, 0x8E, 0xF3 };
        /// byte[] buffer3 = new byte[] { 0x30, 0x75, 0x99, 0xCD };
        /// buffer1.Concat(buffer2, buffer3).ToHex();
        /// </code>
        /// The last line of code would return "FFABDE5500018EF3307599CD".
        /// </example>
        //This is functionally identical to the other Processing.Concat that only takes a params byte[][], but for syntactic sugar/easy discovery is an extension method.
        public static byte[] Concat(this byte[] firstArray, params byte[][] arrays)
        {
            byte[] result = new byte[firstArray.Length + arrays.Sum(a => a.Length)];
            int offset = firstArray.Length;
            firstArray.Transfer(result, length: offset);
            foreach (byte[] array in arrays)
            {
                array.Transfer(result, 0, offset, array.Length);
                offset += array.Length;
            }
            return result;
        }

        /// <summary>
        /// Concatenates a byte array with a MemoryStream's underlying byte array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <exception cref="System.UnauthorizedAccessException"/>
        /// <exception cref="System.OverflowException"/>
        /// <example>
        /// Adding a header to some byte data in a stream to then store somewhere:
        /// byte[] header = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
        /// MemoryStream ms = new MemoryStream();
        /// /*Some stuff writes to the memory stream here..*/
        /// byte[] data = header.Concat(ms);
        /// </example>
        public static byte[] Concat(this byte[] array, MemoryStream stream)
        {
            int streamLength = Convert.ToInt32(stream.Length); //This line uses Convert.ToInt32 so that overflow exception is thrown if the long is too large to fit in the int (which we want failure for).
            byte[] result = new byte[array.Length + streamLength];
            array.Transfer(result, length: array.Length);
            stream.GetBuffer().Transfer(result, 0, array.Length, streamLength); //Have to specify length because the array is longer than the actual data to accomodate growth (Microsoft stream functions account for this).
            return result;
        }

        /// <summary>
        /// Concatenates a MemoryStream's underlying byte array with a byte array.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <exception cref="System.UnauthorizedAccessException"/>
        /// <exception cref="System.OverflowException"/>
        /// <example>
        /// Adding a footer to some byte data in a stream to then store somewhere:
        /// byte[] footer = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
        /// MemoryStream ms = new MemoryStream();
        /// /*Some stuff writes to the memory stream here..*/
        /// byte[] data = ms.Concat(footer);
        /// </example>
        public static byte[] Concat(this MemoryStream stream, byte[] array)
        {
            int streamLength = Convert.ToInt32(stream.Length); //This line uses Convert.ToInt32 so that overflow exception is thrown if the long is too large to fit in the int (which we want failure for).
            byte[] result = new byte[array.Length + streamLength];
            stream.GetBuffer().Transfer(result, length: streamLength); //Have to specify length because the array is longer than the actual data to accomodate growth (Microsoft stream functions account for this).
            array.Transfer(result, 0, streamLength, array.Length);
            return result;
        }

        /// <summary>
        /// Encapsulates a MemoryStream's underlying byte array between two other byte arrays as one result.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="header"></param>
        /// <param name="footer"></param>
        /// <returns></returns>
        /// <example>
        /// Adding a header and footer to a stream's data.
        /// <code>
        /// byte[] header = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        /// byte[] footer = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
        /// MemoryStream ms = new MemoryStream();
        /// /*Some stuff writes to the memory stream here..*/
        /// byte[] data = ms.Encapsulate(header, footer);
        /// </code>
        /// </example>
        public static byte[] Encapsulate(this MemoryStream stream, byte[] header, byte[] footer)
        {
            int streamLength = Convert.ToInt32(stream.Length); //This line uses Convert.ToInt32 so that overflow exception is thrown if the long is too large to fit in the int (which we want failure for).
            int headerAndStreamLength = header.Length + streamLength;
            byte[] result = new byte[headerAndStreamLength + footer.Length];
            header.Transfer(result, length: header.Length);
            stream.GetBuffer().Transfer(result, 0, header.Length, streamLength); //Have to specify length because the array is longer than the actual data to accomodate growth (Microsoft stream functions account for this).
            footer.Transfer(result, 0, headerAndStreamLength, footer.Length);
            return result;
        }

        /// <summary>
        /// Encapsulates a byte array between two other byte arrays as one result.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="header"></param>
        /// <param name="footer"></param>
        /// <returns></returns>
        /// <example>
        /// Adding a header and footer to a stream's data.
        /// <code>
        /// byte[] header = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        /// byte[] content = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x02, 0x00, 0x00, 0x01, 0x08 };
        /// byte[] footer = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
        /// byte[] data = content.Encapsulate(header, footer);
        /// </code>
        /// At the end, data would be { 0x01, 0x02, 0x03, 0x04, 0x00, 0x00, 0x00, 0x01, 0x02, 0x00, 0x00, 0x01, 0x08, 0xDE, 0xAD, 0xBE, 0xEF };
        /// </example>
        public static byte[] Encapsulate(this byte[] content, byte[] header, byte[] footer)
        {
            int headerAndContentLength = header.Length + content.Length;
            byte[] result = new byte[headerAndContentLength + footer.Length];
            header.Transfer(result, length: header.Length);
            content.Transfer(result, 0, header.Length, content.Length);
            footer.Transfer(result, 0, headerAndContentLength, footer.Length);
            return result;
        }

        /// <summary>
        /// Copies bytes from a byte array into another byte array at a specific destination offset starting at a given source offset.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="sourceOffset"></param>
        /// <param name="destinationOffset"></param>
        /// <param name="length">number of bytes to copy, defaults to destination array's length</param>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <example>
        /// <code>
        /// byte[] source = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
        /// byte[] dest = new byte[8];
        /// source.Transfer(dest, 0, 4, source.Length);
        /// </code>
        /// The content of dest afterward would be { 0x00, 0x00, 0x00, 0x00, 0xDE, 0xAD, 0xBE, 0xEF };
        /// </example>
        public static void Transfer(this byte[] source, byte[] destination, int sourceOffset = 0, int destinationOffset = 0, int length = -1)
        {
            Buffer.BlockCopy(source, sourceOffset, destination, destinationOffset, length == -1 ? destination.Length : length);
        }

        /// <summary>
        /// Extracts a length of bytes from a byte array at a given offset.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <example>
        /// Let's take 2 bytes out of an array starting 2 bytes in:
        /// <code>
        /// byte[] source = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0x00, 0x00 };
        /// byte[] bytesWeWanted = source.Extract(2, 2);
        /// </code>
        /// After this code, bytesWeWanted would be { 0xBE, 0xEF };
        /// </example>
        public static byte[] Extract(this byte[] source, int offset, int length)
        {
            byte[] result = new byte[length];
            source.Transfer(result, offset, 0, length);
            return result;
        }

        /// <summary>
        /// Extracts a length of bytes from the internal byte array of a MemoryStream at a given offset.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <exception cref="System.UnauthorizedAccessException"/>
        /// <example>
        /// Let's say we want the last 16 bytes in a MemoryStream's underlying data:
        /// <code>
        /// MemoryStream ms = new MemoryStream();
        /// /*Something writes to the stream*/
        /// int msLength = Convert.ToInt32(ms.Length);
        /// byte[] desiredBytes = ms.Extract(msLength - 16, 16);
        /// </code>
        /// </example>
        //Using .ToArray() does the same thing except it can only return the whole array. 
        //In order to get a slice of the array using ToArray you end up with an extra copy 
        //of the original array - this function allows you to avoid that.
        public static byte[] Extract(this MemoryStream stream, int offset, int length)
        {
            byte[] result = new byte[length];
            byte[] buffer = stream.GetBuffer();
            buffer.Transfer(result, offset, 0, length);
            return result;
        }

        /// <summary>
        /// Repeats the byte array's contents a certain number of times into a new byte array and returns that array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="repeatCount"></param>
        /// <returns></returns>
        public static byte[] Repeat(this byte[] sequence, int repeatCount)
        {
            int totalLength = sequence.Length * repeatCount;
            byte[] result = new byte[totalLength];
            for (int i = 0; i < repeatCount; i++)
            {
                Processing.Transfer(sequence, result, 0, i * sequence.Length, sequence.Length);
            }
            return result;
        }

        /// <summary>
        /// Makes a copy of the original byte array and returns it.
        /// </summary>
        /// <param name="data">the original byte array</param>
        /// <returns></returns>
        public static byte[] Duplicate(this byte[] data)
        {
            byte[] clone = new byte[data.Length];
            data.Transfer(clone);
            return clone;
        }

        /// <summary>
        /// Performs Encoding.GetString on the internal byte array of a MemoryStream with all peculiarities taken care of.
        /// </summary>
        /// <param name="e">the encoding to use to interpret the bytes</param>
        /// <param name="stream">the stream to access (note that it must be open or an exception will be thrown</param>
        /// <returns></returns>
        /// <exception cref="System.OverflowException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        /// <exception cref="System.Text.DecoderFallbackException"/>
        /// <exception cref="System.UnauthorizedAccessException"/>
        /// <example>
        /// <code>
        /// MemoryStream ms = new MemoryStream();
        /// /*Something writes text data to the stream in ASCII*/
        /// string stringResult = Encoding.ASCII.GetString(ms);
        /// </code>
        /// stringResult contains the correct original string data.
        /// </example>
        public static string GetString(this Encoding e, MemoryStream stream)
        {
            int streamLength = Convert.ToInt32(stream.Length); //This line uses Convert.ToInt32 so that overflow exception is thrown if the long is too large to fit in the int (which we want failure for).
            return e.GetString(stream.GetBuffer(), 0, streamLength); //Have to specify length because the array is longer than the actual data to accomodate growth (Microsoft stream functions account for this).
        }

        /// <summary>
        /// Subtracts the specified number of milliseconds from the provided DateTime and returns the resulting DateTime.
        /// </summary>
        /// <param name="dt">the DateTime to operate on</param>
        /// <param name="count">the number of milliseconds to subtract</param>
        /// <returns>the modified DateTime</returns>
        /// <example>
        /// With this we can subtract rather than add, making it a bit more readable.
        /// 
        /// Before:
        /// <c>DateTime.Now.AddMilliseconds(-100);</c>
        /// 
        /// After:
        /// <c>DateTime.Now.SubtractMilliseconds(100);</c>
        /// 
        /// </example>
        //Convenience function, works exactly like AddMilliseconds with a negative number but is more readable.
        public static DateTime SubtractMilliseconds(this DateTime dt, double count)
        {
            return dt.AddMilliseconds(-count);
        }

        /// <summary>
        /// Subtracts the specified number of seconds from the provided DateTime and returns the resulting DateTime.
        /// </summary>
        /// <param name="dt">the DateTime to operate on</param>
        /// <param name="count">the number of seconds to subtract</param>
        /// <returns>the modified DateTime</returns>
        /// <example>
        /// With this we can subtract rather than add, making it a bit more readable.
        /// 
        /// Before:
        /// <c>DateTime.Now.AddSeconds(-10);</c>
        /// 
        /// After:
        /// <c>DateTime.Now.SubtractSeconds(10);</c>
        /// 
        /// </example>
        //Convenience function, works exactly like AddSeconds with a negative number but is more readable.
        public static DateTime SubtractSeconds(this DateTime dt, double count)
        {
            return dt.AddSeconds(-count);
        }

        /// <summary>
        /// Subtracts the specified number of minutes from the provided DateTime and returns the resulting DateTime.
        /// </summary>
        /// <param name="dt">the DateTime to operate on</param>
        /// <param name="count">the number of minutes to subtract</param>
        /// <returns>the modified DateTime</returns>
        /// <example>
        /// With this we can subtract rather than add, making it a bit more readable.
        /// 
        /// Before:
        /// <c>DateTime.Now.AddMinutes(-1);</c>
        /// 
        /// After:
        /// <c>DateTime.Now.SubtractMinutes(1);</c>
        /// 
        /// </example>
        //Convenience function, works exactly like AddMinutes with a negative number but is more readable.
        public static DateTime SubtractMinutes(this DateTime dt, double count)
        {
            return dt.AddMinutes(-count);
        }

        /// <summary>
        /// Subtracts the specified number of hours from the provided DateTime and returns the resulting DateTime.
        /// </summary>
        /// <param name="dt">the DateTime to operate on</param>
        /// <param name="count">the number of hours to subtract</param>
        /// <returns>the modified DateTime</returns>
        /// <example>
        /// With this we can subtract rather than add, making it a bit more readable.
        /// 
        /// Before:
        /// <c>DateTime.Now.AddHours(-1);</c>
        /// 
        /// After:
        /// <c>DateTime.Now.SubtractHours(1);</c>
        /// 
        /// </example>
        //Convenience function, works exactly like AddHours with a negative number but is more readable.
        public static DateTime SubtractHours(this DateTime dt, double count)
        {
            return dt.AddHours(-count);
        }

        /// <summary>
        /// Subtracts the specified number of days from the provided DateTime and returns the resulting DateTime.
        /// </summary>
        /// <param name="dt">the DateTime to operate on</param>
        /// <param name="count">the number of days to subtract</param>
        /// <returns>the modified DateTime</returns>
        /// <example>
        /// With this we can subtract rather than add, making it a bit more readable.
        /// 
        /// Before:
        /// <c>DateTime.Now.AddDays(-7);</c>
        /// 
        /// After:
        /// <c>DateTime.Now.SubtractDays(7);</c>
        /// 
        /// </example>
        //Convenience function, works exactly like AddDays with a negative number but is more readable.
        public static DateTime SubtractDays(this DateTime dt, double count)
        {
            return dt.AddDays(-count);
        }

        /// <summary>
        /// Subtracts the specified number of months from the provided DateTime and returns the resulting DateTime.
        /// </summary>
        /// <param name="dt">the DateTime to operate on</param>
        /// <param name="count">the number of months to subtract</param>
        /// <returns>the modified DateTime</returns>
        /// <example>
        /// With this we can subtract rather than add, making it a bit more readable.
        /// 
        /// Before:
        /// <c>DateTime.Now.AddMonths(-2);</c>
        /// 
        /// After:
        /// <c>DateTime.Now.SubtractMonths(2);</c>
        /// 
        /// </example>
        //Convenience function, works exactly like AddMonths with a negative number but is more readable.
        public static DateTime SubtractMonths(this DateTime dt, int count)
        {
            return dt.AddMonths(-count);
        }

        /// <summary>
        /// Subtracts the specified number of years from the provided DateTime and returns the resulting DateTime.
        /// </summary>
        /// <param name="dt">the DateTime to operate on</param>
        /// <param name="count">the number of years to subtract</param>
        /// <returns>the modified DateTime</returns>
        /// <example>
        /// With this we can subtract rather than add, making it a bit more readable.
        /// 
        /// Before:
        /// <c>DateTime.Now.AddYears(-4);</c>
        /// 
        /// After:
        /// <c>DateTime.Now.SubtractYears(4);</c>
        /// 
        /// </example>
        //Convenience function, works exactly like AddYears with a negative number but is more readable.
        public static DateTime SubtractYears(this DateTime dt, int count)
        {
            return dt.AddYears(-count);
        }

        /// <summary>
        /// Clears the MemoryStream's contents without reallocating memory or breaking any dependencies on the stream object.
        /// </summary>
        /// <param name="ms">the MemoryStream to clear</param>
        /// <exception cref="System.UnauthorizedAccessException"/>
        /// <exception cref="System.ObjectDisposedException"/>
        /// <example>
        /// If you're reusing a MemoryStream and want to make sure you don't retrieve old data from it without wastefully creating a new one:
        /// <code>
        /// MemoryStream ms = new MemoryStream();
        /// /*Something writes to MemoryStream*/
        /// /*Something uses data in MemoryStream*/
        /// ms.Clear();
        /// /*Something else writes to MemoryStream*/
        /// /*Something uses data in MemoryStream without getting any old data mixed in*/
        /// </code>
        /// </example>
        public static void Clear(this MemoryStream ms)
        {
            byte[] buffer = ms.GetBuffer();
            Array.Clear(buffer, 0, buffer.Length);
            ms.Position = 0;
            ms.SetLength(0);
        }

        /// <summary>
        /// The type of size.
        /// </summary>
        public enum SizeType
        {
            /// <summary>
            /// The traditional calculations for size on computers (based on divisibility by 1024 and bytes as the smallest unit).
            /// </summary>
            Traditional,
            /// <summary>
            /// The SI variation on the traditional calculations (based on divisibility by 1000 and bytes as the smallest unit).
            /// </summary>
            SI,
            /// <summary>
            /// The measurement of size used in marketing to inflate the values (based on divisibility by 1000 and bits as the smallest unit).
            /// </summary>
            Bit,
        }

        internal static string[] traditionalSizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        internal static string[] siSizeSuffixes = { "B", "kB", "mB", "gB", "tB", "pB", "eB", "zB", "yB" };
        internal static string[] bitSizeSuffixes = { "bit", "kbit", "mbit", "gbit", "tbit", "pbit", "ebit", "zbit", "ybit" };

        /// <summary>
        /// Returns a human-readable size string for the byte value, converting to larger units as appropriate and labeling it with the unit used.
        /// </summary>
        /// <param name="sizeInBytes">the number we're working on (should be in bytes)</param>
        /// <param name="sizeType">the type of size to use</param>
        /// <param name="decimalPlaces">the number of decimal places to preserve (ignored if roundingType is RoundingType.None)</param>
        /// <param name="roundingType">the rounding type to use (use RoundingType.None if you don't want any rounding)</param>
        /// <returns>a human-readable size string</returns>
        /// <example>
        /// You've got a filesize (from somewhere, usually not hardcoded like this) and want to display it in a human-readable form:
        /// <code>
        /// ulong fileSizeInBytes = 4770000000;
        /// string readable = Processing.GetSizeString(fileSizeInBytes, Processing.SizeType.SI);
        /// </code>
        /// readable would contain "4.77gB".
        /// </example>
        public static string GetSizeString(ulong sizeInBytes, SizeType sizeType = SizeType.Traditional, RoundingType roundingType = RoundingType.Traditional, int decimalPlaces = 2)
        {
            if (roundingType != RoundingType.None && decimalPlaces < 0) throw new ArgumentException("If you are performing rounding, you must have zero or greater decimal places.", "decimalPlaces");
            int unitBase;
            string[] sizeSuffixes;
            //Depending on which type of size we're using, set up the base and the suffixes.
            switch (sizeType)
            {
                case SizeType.Traditional:
                    unitBase = 1024;
                    sizeSuffixes = traditionalSizeSuffixes;
                    break;
                case SizeType.SI:
                    unitBase = 1000;
                    sizeSuffixes = siSizeSuffixes;
                    break;
                case SizeType.Bit:
                    unitBase = 1000;
                    sizeSuffixes = bitSizeSuffixes;
                    break;
                default:
                    throw new ArgumentException("Invalid SizeType.", "sizeType");
            }
            //If we're doing this in bits, multiply by 8 to convert from bytes to bits, otherwise just pass the size along.
            var size = sizeType == SizeType.Bit ? sizeInBytes * 8 : sizeInBytes;
            //Determine which unit we'll be using.
            var unitTier = (int)Math.Log(size, unitBase);
            //If the unit is out of range of our list of suffixes, pick the biggest.
            if (unitTier >= sizeSuffixes.Length) unitTier = sizeSuffixes.Length - 1;
            //Scale the value to the unit we're using.
            var scaledSize = size / Math.Pow(unitBase, unitTier);
            //Perform rounding of the requested type if it was requested at all in the parameters.
            if (roundingType != RoundingType.None)
                scaledSize = scaledSize.Round(roundingType, decimalPlaces);
            //Return a string consisting of the scaled value and the suffix.
            return scaledSize + sizeSuffixes[unitTier];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="selector"></param>
        /// <param name="destinationIndex"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.InvalidOperationException"/>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        public static List<T> Move<T>(this List<T> list, Func<T, bool> selector, int destinationIndex)
        {
            T item = list.Single(selector);
            list.Remove(item);
            list.Insert(destinationIndex, item);
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.InvalidOperationException"/>
        public static List<T> MoveToStart<T>(this List<T> list, Func<T, bool> selector)
        {
            T item = list.Single(selector);
            list.Remove(item);
            list.Insert(0, item);
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.InvalidOperationException"/>
        public static List<T> MoveToEnd<T>(this List<T> list, Func<T, bool> selector)
        {
            T item = list.Single(selector);
            list.Remove(item);
            list.Insert(list.Count, item);
            return list;
        }

        /// <summary>
        /// Returns the grammatically correct verb based on the value provided, either "is" when the value is 1, otherwise "are" - optionally with enclosing space characters.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="enclosedInSpaces">if true (the default) it returns the answer with leading/trailing spaces for common case convenience</param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// int potatoes = 1;
        /// MessageBox.Show("There" + Processing.GetIsOrAre(potatoes) + potatoes + ".");
        /// </code>
        /// This will show "There is 1."
        /// <code>
        /// int potatoes = 3;
        /// MessageBox.Show("There " + Processing.GetIsOrAre(potatoes, false) + " " + potatoes + ".");
        /// </code>
        /// This will show "There are 3."
        /// </example>
        public static string GetIsOrAre<T>(T value, bool enclosedInSpaces = true)
            where T : IEquatable<T>
        {
            if (enclosedInSpaces)
                return value.Equals(1) ? " is " : " are ";
            else
                return value.Equals(1) ? "is" : "are";
        }

        /// <summary>
        /// Returns the grammatically correct word based on the word provided, either "an" when the word starts with a vowel, otherwise "a" - optionally with enclosing space characters.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="enclosedInSpaces">if true (the default) it returns the answer with leading/trailing spaces for common case convenience</param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// string s = "potato";
        /// MessageBox.Show("It's" + Processing.GetAOrAn(s) + s + ".");
        /// </code>
        /// This will show "It's a potato."
        /// <code>
        /// string s = "illusion";
        /// MessageBox.Show("It's " + Processing.GetAOrAn(s, false) + " " + s + ".");
        /// </code>
        /// This will show "It's an illusion."
        /// </example>
        public static string GetAOrAn(string word, bool enclosedInSpaces = true)
        {
            if (enclosedInSpaces)
                return word.First().IsVowel(false) ? " an " : " a ";
            else
                return word.First().IsVowel(false) ? "an" : "a";
        }

        /// <summary>
        /// Returns the grammatically correct phrase based on the word provided, either "an [word]" when the word starts with a vowel, otherwise "a [word]" - optionally with enclosing space characters.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="enclosedInSpaces">if true (the default) it returns the answer with leading/trailing spaces for common case convenience</param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// string s = "potato";
        /// MessageBox.Show("It's" + Processing.GetAOrAnPhrase(s) + ".");
        /// </code>
        /// This will show "It's a potato."
        /// <code>
        /// string s = "illusion";
        /// MessageBox.Show("It's " + Processing.GetAOrAnPhrase(s) + ".");
        /// </code>
        /// This will show "It's an illusion."
        /// </example>
        public static string GetAOrAnPhrase(string word)
        {
            return GetAOrAn(word, false) + ' ' + word;
        }

        //Note that different rules are in place for proper nouns and this is not designed to handle them at all.
        /// <summary>
        /// Pluralizes the input string (intended to take a single word as input).
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Pluralize(this string s)
        {
            if (s.Length == 1)
                return s + "'s"; //Plural characters end in "'s" traditionally.
            char last = s.Last();
            char secondToLast = s[s.Length - 2];
            char thirdToLast = s[s.Length - 3];
            //If it's a case where the plural is the same as the singular, return the word.
            if (Data.SameWhenPlural.Contains(s) || Data.SameWhenPluralSuffixes.Any(s.EndsWith))
                return s;
            //If it's a special case, exception to a rule, etc..
            if (Data.SpecialCasePlurals.ContainsKey(s))
                return Data.SpecialCasePlurals[s];
            //Special suffix cases go here..
            if (s.EndsWith("man"))
            {
                return s.SkipFromEnd(3) + "men"; //Woman, tradesman, etc..
            }
            switch (s.Last())
            {
                case 'm':
                    switch (secondToLast)
                    {
                        case 'u': //ends with "um"
                            switch (thirdToLast)
                            {
                                case 't': //ends with 'tum'
                                    return s.SkipFromEnd(2) + "a";
                                default:
                                    return s + "s";
                            }
                        default:
                            return s + "s";
                    }
                case 'z':
                    switch (secondToLast)
                    {
                        case 'z': //ends with 'zz'
                            return s + "es";
                        default:
                            return s + "zes";
                    }
                case 's':
                    switch (secondToLast)
                    {
                        case 'u': //ends with "us"
                            char fourthToLast = s[s.Length - 4];
                            switch (thirdToLast)
                            {
                                case 'p': //ends with "pus"
                                    return s + "es";
                                case 'n': //ends with "nus"
                                    if (fourthToLast == 'm') //ends with "mnus"
                                        return s.SkipFromEnd(2) + "i";
                                    else if (Data.Vowels.Contains(fourthToLast)) //ends with "anus", "enus", "inus", "onus", "unus", "ynus"
                                        return s + "es";
                                    else //ends with "nus", preceded by any consonant except 'm'
                                        return s.SkipFromEnd(2) + "era";
                                case 'c': //ends with "cus" - there are exceptions but not commonly used in English, i.e., viscus->viscera where the singular is almost never used.
                                case 'i': //ends with "ius"
                                case 'l': //ends with "lus"
                                case 'g': //ends with "gus"
                                case 'm': //ends with "mus"
                                    return s.SkipFromEnd(2) + "i";
                                case 'r': //ends with "rus"
                                case 's': //ends with "sus"
                                case 't': //ends with "tus" - some words have alternative correct forms, i.e. cactus->cacti, but this is correct more often and cactus->cactuses is still valid.
                                default:
                                    return s + "es";
                            }
                        case 'i': //ends with "is"
                            return s.SkipFromEnd(2) + "es";
                        default:
                            return s + "es";
                    }
                case 'a':
                    switch (secondToLast)
                    {
                        case 'n': //ends with "na"
                        case 'l': //ends with "la"
                            return s + "e";
                        default:
                            return s + "s";
                    }
                case 'e':
                    switch (secondToLast)
                    {
                        case 'f': //ends with "fe"
                            if (Data.Vowels.Contains(thirdToLast))
                                return s.SkipFromEnd(2) + "ves";
                            else
                                return s + "s";
                        case 's': //ends with "se"
                        default:
                            return s + "s";
                    }
                case 'x':
                    switch (secondToLast)
                    {
                        case 'e': //ends with "ex"
                            switch (thirdToLast)
                            {
                                case 'd': //ends with 'dex'
                                case 't': //ends with 'tex'
                                    return s.SkipFromEnd(2) + "ices";
                                default:
                                    return s + "es";

                            }
                        case 'i': //ends with "ix"
                            return s.SkipFromEnd(2) + "ices";
                        default:
                            return s + "es";
                    }
                case 'g':
                    if (Data.Vowels.Contains(secondToLast) || secondToLast == 'n') //ends with "ag", "eg", "ig", "og", "ug", "yg", "ng"
                        return s + "s";
                    else
                        return s + "es";
                case 'o':
                    switch (secondToLast)
                    {
                        case 't': //ends with 'to'
                            return s + "es";
                        default:
                            return s + "s";
                    }
                case 'h':
                    switch (secondToLast)
                    {
                        case 't': //ends with "th"
                        case 'p': //ends with "ph"
                        case 'g': //ends with "gh"
                            return s + "s";
                        default:
                            return s + "es";
                    }
                case 'y':
                    if (Data.Vowels.Contains(secondToLast)) //ends with "ay", "ey", "iy", "oy", "uy", "yy"
                        return s + "s";
                    else
                        return s.SkipFromEnd(1) + "ies";
                case 'f':
                    switch (secondToLast)
                    {
                        case 'r': //ends with "rf"
                        case 'o': //ends with "of"
                        case 'l': //ends with "lf"
                            return s.SkipFromEnd(1) + "ves";
                        case 'f': //ends with "ff" - has some side cases but they are context sensitive, such as "staff" as in a pole being "staves", but "staff" as in people being "staffs".
                        default:
                            return s + "s";
                    }
                case 'j':
                    return s + "es";
                default:
                    return s + "s";
            }
        }

        /// <summary>
        /// Pluralizes the string if the count is not 1.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception>System.ArgumentException</exception>
        public static string PluralizeIfNecessary(this string s, int count)
        {
            if (count == 1)
                return s;
            else
                return s.Pluralize();
        }

        /// <summary>
        /// Returns a string describing the number of something.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetCountString(int count, string name)
        {
            return count + " " + name.PluralizeIfNecessary(count);
        }

        /// <summary>
        /// Returns a string describing the number of something in sentence form.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="name"></param>
        /// <param name="appendPeriod"></param>
        /// <returns></returns>
        public static string GetCountSentence(ulong count, string name, bool appendPeriod = true)
        {
            return "There" + GetIsOrAre(count) + (count == 0 ? "no" : count.ToString()) + " " + name.PluralizeIfNecessary(count) + (appendPeriod ? "." : "");
        }

        /// <summary>
        /// Pluralizes the string if the count is not set to 1. Throws an exception if count is less than zero.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception>System.ArgumentException</exception>
        public static string PluralizeIfNecessary(this string s, ulong count)
        {
            if (count == 1)
                return s;
            else
                return s.Pluralize();
        }

        /// <summary>
        /// Returns a string describing the number of something.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetCountString(ulong count, string name)
        {
            return count + " " + name.PluralizeIfNecessary(count);
        }

        /// <summary>
        /// Returns a string describing the number of something in sentence form.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="name"></param>
        /// <param name="appendPeriod"></param>
        /// <returns></returns>
        public static string GetCountSentence(int count, string name, bool appendPeriod = true)
        {
            return "There" + GetIsOrAre(count) + (count == 0 ? "no" : count.ToString()) + " " + name.PluralizeIfNecessary(count) + (appendPeriod ? "." : "");
        }

        /// <summary>
        /// Returns a number of items from the end of an IEumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> TakeFromEnd<T>(this IEnumerable<T> e, int count)
        {
            return e.Skip(e.Count() - count).Take(count);
        }

        /// <summary>
        /// Returns all items from the enumerable except a number from the end.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> SkipFromEnd<T>(this IEnumerable<T> e, int count)
        {
            return e.Take(e.Count() - count);
        }

        /// <summary>
        /// Returns a substring consisting of a number of chars from the end.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string TakeFromEnd(this string s, int count)
        {
            return s.Substring(s.Length - count, count);
        }

        /// <summary>
        /// Returns a string without a number of chars from the end.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string SkipFromEnd(this string s, int count)
        {
            return s.Substring(0, s.Length - count);
        }

        /// <summary>
        /// Translates a camelCase string into a regular string, optionally capitalizing the first char (each capital letter is made lowercase, and a space is inserted before it).
        /// </summary>
        /// <param name="s"></param>
        /// <param name="firstCharCapitalized"></param>
        /// <returns></returns>
        public static string FromCamelCase(string s, bool firstCharCapitalized = true)
        {
            if (s == null)
                return "";
            //The most we could need is if every character was an upper case one, so make the buffer that size for performance (avoids resizing).
            //Start the StringBuilder with the first character in the buffer, since we don't care what it is as it will always be put into the buffer directly.
            //Capitalize the first character if the parameter firstCharCapitalized is true.
            StringBuilder sb = new StringBuilder((firstCharCapitalized ? char.ToUpper(s[0]) : s[0]).ToString(), s.Length * 2); 
            for (int i = 1; i < s.Length; i++) //We start with the second character as the first is always put in at the start and is handled outside of the loop.
            {
                char c = s[i]; //Get character via index.
                if (c.IsUpper()) //If it's upper case..
                    sb.Append(' '); //Preface it with a space.
                sb.Append(c); //Append the character.
            }
            return sb.ToString();
        }

        /// <summary>
        /// Translates a regular string into a camelCase string, removing spaces and capitalizing the letters that immediately followed them - optionally makes the first character lowercase.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="firstCharLowered"></param>
        /// <returns></returns>
        public static string ToCamelCase(string s, bool firstCharLowered = true)
        {
            int spaceCount = s.Count(c => c == ' ');
            if (spaceCount < 1) //If there are no spaces..
                return s; //..just return the string.
            StringBuilder sb = new StringBuilder(s.Length - spaceCount);
            bool makeNextUpperCase = false;
            foreach (char c in s)
            {
                if (c == ' ') //If the character is a space..
                {
                    makeNextUpperCase = true; //The next character should be upper case (and we're done in this loop iteration).
                }
                else //The character isn't a space..
                {
                    if (firstCharLowered) //If first char should be lowered and hasn't already been.
                    {
                        sb.Append(char.ToLower(c)); //Append lowered character.
                        firstCharLowered = false; //Ensure no more characters get lowered.
                    }
                    else if (makeNextUpperCase) //If the last character was a space..
                    {
                        sb.Append(char.ToUpper(c)); //Append the character in its upper case form (if applicable, otherwise append the character).
                        makeNextUpperCase = false; //Ensure no extra characters get capitalized.
                    }
                    else //Nothing special..
                        sb.Append(c); //Append the character.
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns the last digit of the year, followed by the day of the year.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        //FTI has been referring to what this method returns (ex. 10/20/16 -> 6294) as "Julian Day" apparently, 
        //but it has nothing to do with an actual Julian Day Number or the Julian Calendar in general. -JDS 10/20/16
        public static int GetLastDigitAndDayOfYear(this DateTime dt)
        {
            //Get the last digit of the year as a string (can't slice an int), look it up to get an int quickly, multiply it by 1000 to put it in the thousands place, and add the day of the year.
            return Data.Digits[dt.Year.ToString()[3]] * 1000 + dt.DayOfYear;
        }

        /// <summary>
        /// Returns the last digit of the year, followed by the day of the year - in string format (more performant than doing it in int format if you need a string in the end anyway).
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        //FTI has been referring to what this method returns (ex. 10/20/16 -> "6294") as "Julian Day" apparently, 
        //but it has nothing to do with an actual Julian Day Number or the Julian Calendar in general. -JDS 10/20/16
        public static string GetLastDigitAndDayOfYearString(this DateTime dt)
        {
            //Get the last digit of the year and tack it onto a string representation of dt.DayOfYear forced to 3 digits to form the desired format as a string.
            return dt.Year.ToString()[3] + dt.DayOfYear.ToString("000");
        }

        /// <summary>
        /// Get the digit count of an integer.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        //The if/else statements are to prevent the majority of cases from having to do the more expensive (but infinitely supported) for loop - better performance and less resource usage.
        //More if/else can be added if we need more than 8 digits very often, and the "digits" variable must be incremented and the "j" counter adjusted for each one added. - JDS 12/5/16
        /*public static int GetDigitLength(this int i)
        {
            if (i > 9) //More than 1 digit?
                if (i > 99) //More than 2 digits?
                    if (i > 999) //More than 3 digits?
                        if (i > 9999) //More than 4 digits?
                            if (i > 99999) //More than 5 digits?
                                if (i > 999999) //More than 6 digits?
                                    if (i > 9999999) //More than 7 digits?
                                        if (i > 99999999) //More than 8 digits?
                                        {
                                            int digits = 9; //Start at 9 digits, since we know it's more than 8.
                                            int j = i / 10 / 10 / 10 / 10 / 10 / 10 / 10 / 10 / 10; //Start our counter at the right place for 9 digits.
                                            for (; j > 9; digits++) //Infinite loop until the number left is one digit long, incrementing "digits" for each iteration.
                                            {
                                                j /= 10; //Remove a digit.
                                            }
                                            return digits; //Return result.
                                        }
                                        else
                                            return 8;
                                    else
                                        return 7;
                                else
                                    return 6;
                            else
                                return 5;
                        else
                            return 4;
                    else
                        return 3;
                else
                    return 2;
            else
                return 1;
        }*/

        /// <summary>
        /// Treat the string as an integer and increment it, then convert it back into a string (with optional format).
        /// </summary>
        /// <param name="s"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string IncrementAsInteger(this string s, string format = null)
        {
            return format == null ? s.As<int>(x => ++x) : s.As<int>(x => ++x, format);
        }

        /// <summary>
        /// Treat the string as an integer and deccrement it, then convert it back into a string (with optional format).
        /// </summary>
        /// <param name="s"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string DecrementAsInteger(this string s, string format = null)
        {
            return format == null ? s.As<int>(x => --x) : s.As<int>(x => --x, format);
        }

        /// <summary>
        /// Converts the char to an uppercase version of itself.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        //Not sure what happens if it has no uppercase equivalent, should check, likely returns itself.
        public static char ToUpper(this char c)
        {
            return char.ToUpper(c);
        }
        /// <summary>
        /// Converts the char to an lowercase version of itself.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        //Not sure what happens if it has no lowercase equivalent, should check, likely returns itself.
        public static char ToLower(this char c)
        {
            return char.ToLower(c);
        }
        /// <summary>
        /// ParseProfileNameIntoCaseSpecimen.
        /// This method take 4 parameters to parse out caseNumber, specimenNumber from profile name
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="caseNumber"></param>
        /// <param name="specimenNumber"></param>
        /// <param name="caseDelimiter"></param>
        public static void ParseProfileNameIntoCaseSpecimen(string profileName, ref string caseNumber, ref string specimenNumber, string caseDelimiter)
        {
            // this is zero base index
            int _caseDelimiterPosition = profileName.IndexOf(caseDelimiter);
            if (_caseDelimiterPosition > 0)
            {
                caseNumber = profileName.Substring(0, _caseDelimiterPosition);
                specimenNumber = profileName.Substring(_caseDelimiterPosition + 1, profileName.Length - _caseDelimiterPosition - 1);
            }
            else
            {
                caseNumber = profileName;
                specimenNumber = "";
            }
        }
        /// <summary>
        /// NextBODECaseSequenceNumber.
        /// This method take 1 parameter to calculate the next sequence number.
        /// based on this format AA0,AA1..AA9,AB0,AB1..AB9,etc and input can be of any length
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string NextBODECaseSequenceNumber(string s)
        {
            CharacterRangeSet BODE = new CharacterRangeSet
            {
                { new Range(2, 2), new ContiguousCharacterRange('0', '9') },
                { new Range(0, 1), new ContiguousCharacterRange('A', 'Z') },
            };
            string _nextSequenceNumber = String.Empty;
            while (s != "")
                _nextSequenceNumber += BODE.GetNext(s);
            return _nextSequenceNumber;
        }

    }
}
