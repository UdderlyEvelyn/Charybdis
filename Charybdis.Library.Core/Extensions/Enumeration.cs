using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    //Semantic trickery to allow type constraining to an Enum for static methods.
    public class Enumeration : _enumerationWrapper<Enum> 
    {
        private Enumeration() {} //Don't allow instantiation.
    }
    public abstract class _enumerationWrapper<TClass>
    where TClass : class
    {
        protected _enumerationWrapper() {} //Only allow descendants to instantiate.

        /// <summary>
        /// Get the actual enum object of the provided type for a given name.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.OverflowException"/>
        public static E GetValue<E>(string name, bool caseInsensitive = false)
            where E : struct, TClass
        {
            return (E)Enum.Parse(typeof(E), name, caseInsensitive);
        }

        /// <summary>
        /// Get the actual enum object of the provided type for a given name - if not found, returns null.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static E? GetValueOrNull<E>(string name, bool caseInsensitive = false)
            where E : struct, TClass
        {
            E result;
            if (Enum.TryParse<E>(name, caseInsensitive, out result))
                return result;
            else
                return null;
        }

#pragma warning disable IDE0004

        //https://stackoverflow.com/questions/4171140/iterate-over-values-in-flags-enum
        public static IEnumerable<E> GetFlags<E>(Enum input)
            where E : struct, TClass
        {
            foreach (Enum value in Enum.GetValues(typeof(E)))
                if (input.HasFlag(value))
                    yield return (E)(object)value;
        }
    }

#pragma warning restore IDE0004

    public static class EnumerationExtensions
    {
        /// <summary>
        /// Returns the name of the provided enum value.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.ArgumentException"/>
        public static string GetName(this Enum e, bool expandCamelCaseToSpace = false)
        {
            string name = Enum.GetName(e.GetType(), e);
            return expandCamelCaseToSpace ? Processing.FromCamelCase(name) : name;
        }
    }
}