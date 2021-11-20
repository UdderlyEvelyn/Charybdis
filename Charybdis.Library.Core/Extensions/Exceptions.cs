using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace Charybdis.Library.Core
{
    [ExcludeFromCodeCoverage] //Exception handling methods are almost all identical overloads, don't need to test. Could test Summarize() but it's based on a .NET type so nothing should break it.
    public static class Exceptions
    {
        /// <summary>
        /// Flattens an aggregate exception into a FlattenedAggregateException wrapped in a SummaryAggregateException so it can be displayed without revealing information and then logged in an easy-to-view way.
        /// </summary>
        /// <param name="ae"></param>
        /// <param name="returnSingleExceptionAsIs">If true, when there is only one exception in the AggregateException, it is returned unmodified instead of a SummaryAggregateException.</param>
        /// <returns></returns>
        public static Exception Summarize(this AggregateException ae, bool returnSingleExceptionAsIs = false)
        {
            var exceptions = ae.Flatten().InnerExceptions;
            if (exceptions.Count == 1 && returnSingleExceptionAsIs)
                return exceptions[0];
            StringBuilder sb = new StringBuilder();
            int fatalCount = 0;
            for (int i = 0; i < exceptions.Count; i++)
            {
                Exception e = exceptions[i];
                CustomException ce = e as CustomException;
                if (ce != null)
                {
                    if (ce.Fatal)
                        fatalCount++;
                    sb.Append("\n\n" + (ce.Fatal ? "Fatal " : "") + "Exception #" + i + ":\n" + e.ToString());
                }
                else
                {
                    //We assume all non-custom exceptions are fatal.
                    sb.Append("\n\nFatal Exception #" + i + ":\n" + e.ToString());
                    fatalCount++;
                }
            }
            string summary = "An aggregate exception was thrown, consisting of " + Processing.GetCountString(exceptions.Count, "exception") + ", " + fatalCount + " of which were fatal.";
            return new SummaryAggregateException(summary, new FlattenedAggregateException(sb.ToString(), ae), fatalCount > 0);
        }

        //Throws the exception if it isn't one of the listed ones. Used in catch statements to check exception type without coding complex control structures all over the place.
        //The variant with a second argument of "string message" wraps the original exception in a new exception with the given message (for adding detail to exceptions when thrown).
        //The variant with a second argument of "Exception exception" throws the specified exception (allowing you to construct it yourself of any type).
        #region public static void ThrowIfNot<A..Z>(this Exception e[, string message, string caller (autopopulated) | , Exception exception])

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        public static void ThrowIfNot<A, B>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W
               || e is X)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W
               || e is X
               || e is Y)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with a message that includes the caller name (or the message provided if one was provided) if not, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z>(this Exception e, string message = null, [CallerMemberName]string caller = null)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W
               || e is X
               || e is Y
               || e is Z)) throw new Exception(message ?? "Unexpected failure in " + caller + ".", e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A>(this Exception e, Exception exception) //Single-argument is redundant with regular catch blocks, but allows for different control flow (more readable).
        {
            if (!(e is A)) throw exception;
        }

        public static void ThrowIfNot<A, B>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W
               || e is X)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W
               || e is X
               || e is Y)) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if not (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIfNot<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z>(this Exception e, Exception exception)
        {
            if (!(e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W
               || e is X
               || e is Y
               || e is Z)) throw exception;
        }

        #endregion

        //Throws the exception if it is one of the listed ones. Used in catch statements to check exception type without coding complex control structures all over the place.
        //The variant with a second argument of "string message" wraps the original exception in a new exception with the given message (for adding detail to exceptions when thrown).
        //The variant with a second argument of "Exception exception" throws the provided exception instead (allowing you to construct it yourself of any type).
        #region public static void ThrowIf<A..Z>(this Exception e[, string message | , Exception exception])

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A>(this Exception e)
        {
            if (e is A) throw e;
        }

        public static void ThrowIf<A, B>(this Exception e)
        {
            if (e is A
               || e is B) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W
               || e is X) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W
               || e is X
               || e is Y) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws it if so. (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z>(this Exception e)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W
               || e is X
               || e is Y
               || e is Z) throw e;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A>(this Exception e, string message)
        {
            if (e is A) throw new Exception(message, e);
        }

        public static void ThrowIf<A, B>(this Exception e, string message)
        {
            if (e is A
               || e is B) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W
               || e is X) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W
               || e is X
               || e is Y) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws a new exception with the provided message if so, with the original exception as the inner exception (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z>(this Exception e, string message)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W
               || e is X
               || e is Y
               || e is Z) throw new Exception(message, e);
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A>(this Exception e, Exception exception)
        {
            if (e is A) throw exception;
        }

        public static void ThrowIf<A, B>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W
               || e is X) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W
               || e is X
               || e is Y) throw exception;
        }

        /// <summary>
        /// Tests whether the exception is (or is derived from) one of the passed-in types and throws the provided exception if so (note that these are checked in order and further checks are skipped, so order them most to least likely if possible).
        /// </summary>
        public static void ThrowIf<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z>(this Exception e, Exception exception)
        {
            if (e is A
               || e is B
               || e is C
               || e is D
               || e is E
               || e is F
               || e is G
               || e is H
               || e is I
               || e is J
               || e is K
               || e is L
               || e is M
               || e is N
               || e is O
               || e is P
               || e is Q
               || e is R
               || e is S
               || e is T
               || e is U
               || e is V
               || e is W
               || e is X
               || e is Y
               || e is Z) throw exception;
        }

        #endregion
    }
}
