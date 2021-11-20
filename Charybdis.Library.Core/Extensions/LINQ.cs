using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;
using System.Diagnostics.CodeAnalysis;
using System.Collections.ObjectModel;

namespace Charybdis.Library.Core
{
    /// <summary>
    /// This class encapsulates extensions to LINQ that will operate on any data type.
    /// </summary>
    public static class LINQ
    {

        /// <summary>
        /// Convenience wrapper for calling "Cast" followed by "ToList".
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> CastItemsToListOf<T>(this IEnumerable source)
        {
            return source.Cast<T>().ToList();
        }

        /// <summary>
        /// Convenience wrapper for calling "Cast" followed by "ToList".
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> CastItemsToListOf<T>(this IList source)
        {
            return source.Cast<T>().ToList();
        }

        /// <summary>
        /// Shuffles the enumerable, then takes a number of values from it and returns them.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="count">the number of values to return</param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage] //Can't really validate randomness.
        public static IEnumerable<T> Random<T>(this IEnumerable<T> items, int count)
        {
            return items.Randomize().Take(count);
        }

        /// <summary>
        /// Shuffles the enumerable using a secure random number generator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage] //Can't really validate randomness.
        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> items)
        {
            return items.OrderBy(x => RNG.GetInt());
        }

        /// <summary>
        /// Filters down to a page of items from the queryable using the given parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">the items to retrieve the page from</param>
        /// <param name="page">the page number to retrieve</param>
        /// <param name="pageSize">the number of items per page</param>
        /// <returns></returns>
        public static IQueryable<T> GetPage<T>(this IQueryable<T> items, int page, int pageSize)
        {
            return items.Skip(Math.Max(0, pageSize * (page - 1))).Take(pageSize);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate if a condition is true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">the queryable to operate on</param>
        /// <param name="predicate">the predicate to add</param>
        /// <param name="condition">the condition to check</param>
        /// <returns>the queryable with an added predicate, or if condition fails, then the original queryable</returns>
        /// <exception cref="System.ArgumentNullException"/>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> items, Expression<Func<T, bool>> predicate, bool condition)
        {
            return condition ? items.Where(predicate) : items;
        }


        /// <summary>
        /// Filters a sequence of values based on a predicate if a condition is true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">the queryable to operate on</param>
        /// <param name="predicate">the predicate to add</param>
        /// <param name="condition">the condition to check</param>
        /// <returns>the resulting enumerable - or if the condition fails, then the original queryable</returns>
        /// <exception cref="System.ArgumentNullException"/>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> items, Func<T, bool> predicate, bool condition)
        {
            return condition ? items.Where(predicate) : items;
        }

        public static void ActWhere<T>(this IQueryable<T> items, Expression<Func<T, bool>> predicate, Action<T> action)
        {
            var itemsToActOn = items.Where(predicate).ToList();
            foreach (var item in itemsToActOn)
                action(item);
        }

        /// <summary>
        /// Convenience wrapper for HasValue and a Value comparison on a nullable struct.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasValueEqualTo<T>(this T? t, T value)
            where T : struct
        {
            return t.HasValue && t.Value.Equals(value);
        }

        public static IQueryable<T> WhereDistinct<T, R>(this IQueryable<T> items, Func<T, R> selector)
            where R : IEquatable<R>
        {
            return items.Distinct(new GenericSingleMemberEqualityComparer<T, R>(
                delegate(T t) 
                {
                    return selector(t);
                },
                delegate(R r)
                {
                    return r.GetHashCode();
                }
            ));
        }

        public static IEnumerable<T> WhereDistinct<T, R>(this IEnumerable<T> items, Func<T, R> selector)
            where R : IEquatable<R>
        {
            return items.Distinct(new GenericSingleMemberEqualityComparer<T, R>(
                delegate(T t)
                {
                    return selector(t);
                },
                delegate(R r)
                {
                    return r.GetHashCode();
                }
            ));
        }

        public static IQueryable<R> SelectDistinct<T, R>(this IQueryable<T> items, Func<T, R> selector)
            where R : IEquatable<R>
        {
            return items.WhereDistinct(selector).Select(t => selector(t));
        }

        public static IEnumerable<R> SelectDistinct<T, R>(this IEnumerable<T> items, Func<T, R> selector)
            where R : IEquatable<R>
        {
            return items.WhereDistinct(selector).Select(t => selector(t));
        }

        public static bool AllSame<T, R>(this IQueryable<T> items, Func<T, R> selector)
            where R : IEquatable<R>
        {
            return items.SelectDistinct(selector).Count() == 1;
        }

        public static bool AllSame<T, R>(this IEnumerable<T> items, Func<T, R> selector)
            where R : IEquatable<R>
        {
            return items.SelectDistinct(selector).Count() == 1;
        }

        public static bool AllSame<T>(this IEnumerable<IEnumerable<T>> enumerables, Func<IEnumerable<T>, bool> logic)
            where T : IEquatable<T>
        {

            return !enumerables.Any(e => logic(e));
        }

        public class GenericSingleMemberEqualityComparer<T, R> : IEqualityComparer<T>
        {
            private Func<T, T, bool> _equalityCheck;
            private Func<T, R> _selector;
            private Func<R, int> _hash;

            public GenericSingleMemberEqualityComparer(Func<T, R> selector, Func<R, int> hash)
            {
                _selector = selector;
                _hash = hash;
                _equalityCheck = delegate(T x, T y)
                {
                    var a = selector(x);
                    var b = selector(y);
                    return a.Equals(b);
                };
            }

            public bool Equals(T x, T y)
            {
                return _equalityCheck(x, y);
            }

            public int GetHashCode(T t)
            {
                return _hash(_selector(t));
            }
        }

        /// <summary>
        /// In case of query failure, execute a function and return its result. Otherwise, return the query result as normal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">the query to act on</param>
        /// <param name="method">execution method resulting in one item being returned from the IQueryable</param>
        /// <param name="contingency">the function to perform in case of failure - takes an exception as input to optionally act on, and outputs T</param>
        /// <returns>the result of the query or the result of the contingency function on failure</returns>
        public static T WithContingency<T>(this IQueryable<T> items, Func<IQueryable<T>, T> method, Func<Exception, T> contingency)
        {
            try
            {
                return method(items);
            }
            catch (Exception e)
            {
                e.ThrowIfNot<
                    ArgumentException,
                    ArgumentNullException,
                    MethodAccessException,
                    InvalidOperationException
                    >("Unexpected failure invoking " + method.Method.Name + " on IQueryable<" + typeof(T).FullName + "> with " + Processing.GetCountString(items.Count(), "item") + ".");
                return contingency(e);
            }
        }        
        
        /// <summary>
        /// In case of query failure, do an action and return default(T). Otherwise, return the query result as normal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">the query to act on</param>
        /// <param name="method">execution method resulting in one item being returned from the IQueryable</param>
        /// <param name="contingency">the action to do in case of failure - takes an exception as input to optionally act on</param>
        /// <returns>the result of the query or default(T) on failure</returns>
        public static T WithContingency<T>(this IQueryable<T> items, Func<IQueryable<T>, T> method, Action<Exception> contingency)
        {
            try
            {
                return method(items);
            }
            catch (Exception e)
            {
                e.ThrowIfNot<
                    ArgumentException,
                    ArgumentNullException,
                    MethodAccessException,
                    InvalidOperationException
                    >("Unexpected failure invoking " + method.Method.Name + " on IQueryable<" + typeof(T).FullName + "> with " + Processing.GetCountString(items.Count(), "item") + ".");
                contingency(e);
                return default(T);
            }
        }

        /// <summary>
        /// Returns the enumerable if it is defined, but replaces it with an initialized empty enumerable if it is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> e)
        {
            return e ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Allows you to run code on a value in-line (note that if used on a collection it operates on the collection itself, not its content).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="action"></param>
        [ExcludeFromCodeCoverage] //Internal function doesn't need AUT.
        internal static void Do<T>(this T t, Action<T> action)
        {
            action(t);
        }        
        
        /// <summary>
        /// Allows you to run code on each value in a collection in-line.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="action"></param>
        [ExcludeFromCodeCoverage] //Internal function doesn't need AUT.
        internal static void Do<T>(this IEnumerable<T> e, Action<T> action)
        {
            foreach (T t in e) action(t);
        }

        /// <summary>
        /// Fluent foreach that provides access to an object during the loop which is then returned at the end.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="action"></param>
        [ExcludeFromCodeCoverage] //Internal function doesn't need AUT.
        internal static O Do<O, T>(this O o, IEnumerable<T> e, Action<O, T> action)
        {
            foreach (T t in e) action(o, t);
            return o;
        }

        /// <summary>
        /// Converts the object to the specified type if possible. If there is a failure, returns the default value or the one provided.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="defaultOverride">this value is used if there is a failure - defaults to default(T)</param>
        /// <returns></returns>
        public static T To<T>(this IConvertible obj, T defaultOverride = default(T))
        {
            try
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            catch (Exception e)
            {
                e.ThrowIfNot<
                    InvalidCastException, 
                    FormatException, 
                    OverflowException, 
                    ArgumentNullException>();
                return defaultOverride;
            }
        }

        /// <summary>
        /// Convenience wrapper for the "is" keyword in extension form.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool Is<T>(this object o) where T : class
        {
            return o is T;
        }

        /// <summary>
        /// Checks if the object is one of the provided types (may not behave the same as the "is" keyword).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool Is(this object o, params Type[] types)
        {
            return types.Any(t => t.IsInstanceOfType(o));
        }

        /// <summary>
        /// Checks if the object is one of the provided types (may not behave the same as the "is" keyword).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool Is(this object o, IEnumerable<Type> types)
        {
            return types.Any(t => t.IsInstanceOfType(o));
        }

        //Different overloads to accept any number of type parameters from A-Z (1-26), returns whether the passed-in object is (or is derived from) one of the passed-in types.
        //Note that these are checked in order and further checks are skipped, so order them most to least likely if possible.
        #region public static bool Is<A..Z>(this object o)
        //Add more if more are necessary. Should probably never be necessary to check for that many, though.

        //Could do the following on all/any of them, but since there is no way to get a "params" of type arguments there's no point in using less efficient generic code:
        //return MethodInfo.GetCurrentMethod().GetGenericArguments().Any(t => t.IsInstanceOfType(o));

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B>(this object o)
        {
            return o is A 
                || o is B;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C>(this object o)
        {
            return o is A
                || o is B
                || o is C;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J, K>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J
                || o is K;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J, K, L>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J
                || o is K
                || o is L;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J, K, L, M>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J
                || o is K
                || o is L
                || o is M;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J, K, L, M, N>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J
                || o is K
                || o is L
                || o is M
                || o is N;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J
                || o is K
                || o is L
                || o is M
                || o is N
                || o is O;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J
                || o is K
                || o is L
                || o is M
                || o is N
                || o is O
                || o is P;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J
                || o is K
                || o is L
                || o is M
                || o is N
                || o is O
                || o is P
                || o is Q;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J
                || o is K
                || o is L
                || o is M
                || o is N
                || o is O
                || o is P
                || o is Q
                || o is R;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J
                || o is K
                || o is L
                || o is M
                || o is N
                || o is O
                || o is P
                || o is Q
                || o is R
                || o is S;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J
                || o is K
                || o is L
                || o is M
                || o is N
                || o is O
                || o is P
                || o is Q
                || o is R
                || o is S
                || o is T;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J
                || o is K
                || o is L
                || o is M
                || o is N
                || o is O
                || o is P
                || o is Q
                || o is R
                || o is S
                || o is T
                || o is U;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J
                || o is K
                || o is L
                || o is M
                || o is N
                || o is O
                || o is P
                || o is Q
                || o is R
                || o is S
                || o is T
                || o is U
                || o is V;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J
                || o is K
                || o is L
                || o is M
                || o is N
                || o is O
                || o is P
                || o is Q
                || o is R
                || o is S
                || o is T
                || o is U
                || o is V
                || o is W;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J
                || o is K
                || o is L
                || o is M
                || o is N
                || o is O
                || o is P
                || o is Q
                || o is R
                || o is S
                || o is T
                || o is U
                || o is V
                || o is W
                || o is X;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J
                || o is K
                || o is L
                || o is M
                || o is N
                || o is O
                || o is P
                || o is Q
                || o is R
                || o is S
                || o is T
                || o is U
                || o is V
                || o is W
                || o is X
                || o is Y;
        }

        /// <summary>
        /// Tests whether the object is (or is derived from) one of the passed-in types (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        [ExcludeFromCodeCoverage] //No point in testing every variety of this, the single type argument option above is already tested.
        public static bool Is<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z>(this object o)
        {
            return o is A
                || o is B
                || o is C
                || o is D
                || o is E
                || o is F
                || o is G
                || o is H
                || o is I
                || o is J
                || o is K
                || o is L
                || o is M
                || o is N
                || o is O
                || o is P
                || o is Q
                || o is R
                || o is S
                || o is T
                || o is U
                || o is V
                || o is W
                || o is X
                || o is Y
                || o is Z;
        }

        #endregion

        /// <summary>
        /// Convenience wrapper for a negated "is" keyword in extension form.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsNot<T>(this object o) where T : class
        {
            return !(o is T);
        }

        /// <summary>
        /// Convenience wrapper for the "as" keyword in extension form.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static T As<T>(this object o) where T : class
        {
            return o as T;
        }

        /// <summary>
        /// Returns a string created by performing a transform function over each object in an enumerable and concatenating them via StringBuilder.
        /// Note that this does not use the most efficient calls to StringBuilder.Append, it uses only the object parameter one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e">an enumerable of objects to work on</param>
        /// <param name="transform">transform function from the objects in the enumerable</param>
        /// <returns>concatenated string of objects that have passed through the transform function</returns>
        public static string ToString<T>(this IEnumerable<T> e, Func<T, object> transform)
        {
            return new StringBuilder().Do(e, (sb, i) => sb.Append(transform(i))).ToString();
        }

        /// <summary>
        /// Convenience wrapper (for readability) that concatenates each object in an enumerable into a delimited string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e">the enumerable to operate on</param>
        /// <param name="separator">the string to use as a delimiter in the output</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        public static string Join<T>(this IEnumerable<T> e, string separator)
        {
            return string.Join(separator, e);
        }

        /// <summary>
        /// Convenience wrapper (for readability) that concatenates each object in an enumerable into a delimited string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e">the enumerable to operate on</param>
        /// <param name="separator">the character to use as a delimiter in the output</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"/>
        public static string Join<T>(this IEnumerable<T> e, char separator)
        {
            return string.Join(separator.ToString(), e);
        }

        /// <summary>
        /// Convenience wrapper for a negative Where clause for readability.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<T> Except<T>(this IEnumerable<T> e, Func<T, bool> predicate)
        {
            return e.Where(t => !predicate(t));
        }

        /// <summary>
        /// Returns all items that are not in the blacklist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="blackList"></param>
        /// <returns></returns>
        //public static IEnumerable<T> Except<T>(this IEnumerable<T> e, IEnumerable<T> blackList)
        //{
        //    return e.Where(t => !blackList.Contains(t));
        //}

        /// <summary>
        /// Returns all items aside from those specified in the method arguments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IEnumerable<T> Except<T>(this IEnumerable<T> e, params T[] parameters)
        {
            return e.Where(t => !parameters.Contains(t));
        }

        /// <summary>
        /// Returns a single value from the enumerable that (optionally) satisfies a condition. If there is no such value, or more than one, returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e">the enumerable to check</param>
        /// <param name="predicate">the condition to be satisfied</param>
        /// <returns></returns>
        public static T SingleOrNull<T>(this IQueryable<T> q)
            where T : class
        {
            return q._singleResultOperationOrNull(Queryable.Single);
        }

        /// <summary>
        /// Returns a single value from the enumerable that (optionally) satisfies a condition. If there is no such value, or more than one, returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e">the enumerable to check</param>
        /// <param name="predicate">the condition to be satisfied</param>
        /// <returns></returns>
        public static T SingleOrNull<T>(this IEnumerable<T> e, Func<T, bool> predicate = null)
            where T : class
        {
            return predicate == null ? e._singleResultOperationOrNull(Enumerable.Single) : e._singleResultOperationOrNull(Enumerable.Single, predicate);
        }

        /// <summary>
        /// Returns a value from the object unless the object is null, in which case null is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="t">the object to check</param>
        /// <param name="selector">the selection criteria for the member to return</param>
        /// <returns></returns>
        public static R? SelectStructOrNull<T, R>(this T t, Func<T, R> selector)
            where T : class
            where R : struct
        {
            return t == null ? (R?)null : selector(t);
        }

        [Obsolete("This method was srenamed to SelectStructOrNull to parallel the added SelectClassOrNull - please change references to reflect this so we can remove this redirection stub.")]
        public static R? SelectOrNull<T, R>(this T t, Func<T, R> selector)
            where T : class
            where R : struct
        {
            return t.SelectStructOrNull(selector);
        }

        /// <summary>
        /// Returns a value from the object unless the object is null, in which case null is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="t">the object to check</param>
        /// <param name="selector">the selection criteria for the member to return</param>
        /// <returns></returns>
        public static R SelectClassOrNull<T, R>(this T t, Func<T, R> selector)
            where T : class
            where R : class
        {
            return t == null ? (R)null : selector(t);
        }

        /// <summary>
        /// Returns the first value from the enumerable which (optionally) satisfies a condition. If there is no such value, or more than one, returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e">the enumerable to check</param>
        /// <param name="predicate">the condition to be satisfied</param>
        /// <returns></returns>
        public static T FirstOrNull<T>(this IEnumerable<T> e, Func<T, bool> predicate = null)
            where T : class
        {
            return predicate == null ? e._singleResultOperationOrNull(Enumerable.First) : e._singleResultOperationOrNull(Enumerable.First, predicate);
        }

        /// <summary>
        /// Returns the last value from the enumerable which (optionally) satisfies a condition. If there is no such value, or more than one, returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e">the enumerable to check</param>
        /// <param name="predicate">the condition to be satisfied</param>
        /// <returns></returns>
        public static T LastOrNull<T>(this IEnumerable<T> e, Func<T, bool> predicate = null)
            where T : class
        {
            return predicate == null ? e._singleResultOperationOrNull(Enumerable.Last) : e._singleResultOperationOrNull(Enumerable.Last, predicate);
        }

        /// <summary>
        /// Returns the single result of a function with a predicate. If there is no value, or more than one, returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="operation"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        internal static T _singleResultOperationOrNull<T>(this IEnumerable<T> e, Func<IEnumerable<T>, Func<T, bool>, T> operation, Func<T, bool> predicate)
            where T : class
        {
            try
            {
                return operation(e, predicate);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the single result of a function. If there is no value, or more than one, returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        internal static T _singleResultOperationOrNull<T>(this IEnumerable<T> e, Func<IEnumerable<T>, T> operation)
            where T : class
        {
            try
            {
                return operation(e);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the single result of a function. If there is no value, or more than one, returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        internal static T _singleResultOperationOrNull<T>(this IQueryable<T> q, Func<IQueryable<T>, T> operation)
            where T : class
        {
            try
            {
                return operation(q);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        #region Sort IComparable

        public static IOrderedEnumerable<T> Sort<T>(this IEnumerable<T> e)
            where T : IComparable
        {
            return e.OrderBy(x => x);
        }

        public static IOrderedEnumerable<T> SortDescending<T>(this IEnumerable<T> e)
            where T : IComparable
        {
            return e.OrderByDescending(x => x);
        }

        public static IOrderedQueryable<T> Sort<T>(this IQueryable<T> q)
            where T : IComparable
        {
            return q.OrderBy(x => x);
        }

        public static IOrderedQueryable<T> SortDescending<T>(this IQueryable<T> q)
            where T : IComparable
        {
            return q.OrderByDescending(x => x);
        }

        #endregion

        #region Sort IComparable Property

        public static IOrderedEnumerable<T> Sort<T, K>(this IEnumerable<T> e, Func<T, K> keySelector)
            where K : IComparable
        {
            return e.OrderBy(keySelector);
        }

        public static IOrderedEnumerable<T> SortDescending<T, K>(this IEnumerable<T> e, Func<T, K> keySelector)
            where K : IComparable
        {
            return e.OrderByDescending(keySelector);
        }

        public static IOrderedQueryable<T> Sort<T, K>(this IQueryable<T> q, Expression<Func<T, K>> keySelector)
            where K : IComparable
        {
            return q.OrderBy(keySelector);
        }

        public static IOrderedQueryable<T> SortDescending<T, K>(this IQueryable<T> q, Expression<Func<T, K>> keySelector)
            where K : IComparable
        {
            return q.OrderByDescending(keySelector);
        }

        #endregion
    }
}
