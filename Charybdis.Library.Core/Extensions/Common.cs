using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace Charybdis.Library.Core
{
    public static class Common
    {
        public static void ToFile(this string str, string filename)
        {
            StreamWriter sw = new StreamWriter(File.OpenWrite(filename));
            sw.Write(str);
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// Returns the variable name of the object instance - this function only works in debug mode.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetName(this object obj)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                StackFrame sf = new StackTrace(true).GetFrame(1);
                StreamReader f = new StreamReader(sf.GetFileName());
                for (int i = 0; i < sf.GetFileLineNumber() - 1; i++) f.ReadLine();
                return f.ReadLine().Split(new char[] { '(', ')' })[1].Split(new char[] { '.' })[0];
            }
            else throw new Exception("This function only works in debug mode.");
        }

        public static string EnumerableToString(this IEnumerable<dynamic> e, string separator = " ")
        {
            string output = "";
            foreach (var item in e)
                output += item.ToString() + separator;
            return output;
        }

        /*public static string AsString(this IEnumerable<char> chars)
        {
            string result = "";
            foreach (char c in chars)
            {
                result += c;
            }
            return result;
        }*/
    }
}
