using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Collections;

namespace Charybdis.Library.Core
{
    /// <summary>
    /// Multithreading extension functions.
    /// </summary>
    public static class Multithreading
    {

    }

    /// <summary>
    /// Task extension and supporting functions.
    /// </summary>
    public static class Tasking
    {
        // Method Prefix I/O Table
        /* |Prefix |Input |Output|
         * |PipeDo |   y  |   n  |
         * |Pipe   |   y  |   y  |
         * |Run    |   n  |   y  |
         * |Do     |   y  |   n  |
         */ 

        #region WithCancellation

        /// <summary>
        /// Returns a task with cancellation support via the provided token.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task WithCancellation(this Task task, CancellationToken token)
        {
            return Task.Run(() =>
                {
                    while (true) //Forever, unless something stops it.
                    {
                        if (task.IsCompleted || token.IsCancellationRequested)
                            break; //If task finishes or is cancelled, stop the infinite loop.
                    }
                });
        }

        /// <summary>
        /// Returns a task with a return value with cancellation support via the provided token.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken token)
        {
            return Task.Run(() =>
            {
                while (true) //Forever, unless something stops it.
                {
                    if (task.IsCompleted || token.IsCancellationRequested)
                        break; //If task finishes or is cancelled, stop the infinite loop.
                }
                return task.Result;
            });
        }

        /// <summary>
        /// Returns an array of tasks with added cancellation support via the provided token.
        /// </summary>
        /// <param name="tasks"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task[] WithCancellation(this Task[] tasks, CancellationToken token)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = tasks[i].WithCancellation(token);
            }
            return tasks;
        }

        /// <summary>
        /// Returns an array of tasks with a return value with added cancellation support via the provided token.
        /// </summary>
        /// <param name="tasks"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<T>[] WithCancellation<T>(this Task<T>[] tasks, CancellationToken token)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = tasks[i].WithCancellation<T>(token);
            }
            return tasks;
        }

        /// <summary>
        /// Returns a typed IList of tasks with a return value with added cancellation support via the provided token.
        /// </summary>
        /// <param name="tasks"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IList<Task<T>> WithCancellation<T>(this IList<Task<T>> tasks, CancellationToken token)
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                tasks[i] = tasks[i].WithCancellation<T>(token);
            }
            return tasks;
        }

        #endregion

        #region Task Creation

        /// <summary>
        /// Returns a task that returns the result of the provided function operating on the provided value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="f"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Task<R> Pipe<T, R>(Func<T, R> f, T x)
        {
            return Task.Run(() => f(x));
        }

        /// <summary>
        /// Returns a task that returns the result of the provided function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Task<T> Run<T>(Func<T> f)
        {
            return Task.Run(f);
        }

        /// <summary>
        /// Returns a task that runs the provided action.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Task Do(Action a)
        {
            return Task.Run(a);
        }

        /// <summary>
        /// Returns a task that runs the provided action on the provided value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Task PipeDo<T>(Action<T> a, T x)
        {
            return Task.Run(() => a(x));
        }

        #endregion

        #region ContinueWith

        /// <summary>
        /// Chains a task onto an existing task.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Task ContinueWith(this Task t, Task c)
        {
            return t.ContinueWith(x => c);
        }

        /// <summary>
        /// Chains a task onto an existing task, passing the result of the first into the input of the second and returning the second's output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="t"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Task<R> ContinueWith<T, R>(this Task<T> t, Task<R> c)
        {
            return t.ContinueWith(c);
        }

        /// <summary>
        /// Chains a task onto an existing task, returning the result of the second.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Task<T> ContinueWith<T>(this Task t, Task<T> c)
        {
            return t.ContinueWith(c);
        }

        /// <summary>
        /// Chains a task onto an existing task, discarding the output of the first task.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Task ContinueWith<T>(this Task<T> t, Task c)
        {
            return t.ContinueWith(c);
        }

        #endregion

        #region Continuations

        /// <summary>
        /// After the task, perform an action.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Task ThenDo(this Task t, Action a)
        {
            return t.ContinueWith(x => a);
        }

        /// <summary>
        /// After the task, run a function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Task<T> ThenRun<T>(this Task t, Func<T> f)
        {
            return t.ContinueWith(Run(f));
        }

        /// <summary>
        /// Run the task, passing its result into a function that returns a different type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="t"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Task<R> ThenPipe<T, R>(this Task<T> t, Func<Task<T>, R> f)
        {
            return t.ContinueWith(x => Pipe(f, x)).Unwrap();
        }

        /// <summary>
        /// Run the task, passing its result into an action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Task ThenPipeDo<T>(this Task<T> t, Action<T> a)
        {
            return t.ContinueWith(x => PipeDo(a, x.Result)).Unwrap();
        }

        #endregion

        #region Continuations (With Progress)

        /// <summary>
        /// Run the task, passing its result into a function that returns a different type - with progress tracking.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="t"></param>
        /// <param name="f"></param>
        /// <param name="pb"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task<R> ThenPipeWithProgress<T, R>(this Task<T> t, Func<T, R> f, ProgressBase pb, string taskName = "Unspecified Pipe")
        {
            return t.ContinueWith(x => pb.Pipe(f, x.Result, taskName)).Unwrap();
        }

        /// <summary>
        /// After the task, run a function - with progress tracking.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="f"></param>
        /// <param name="pb"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task<T> ThenRunWithProgress<T>(this Task t, Func<T> f, ProgressBase pb, string taskName = "Unspecified Function")
        {
            return t.ContinueWith(pb.Run(f, taskName));
        }

        /// <summary>
        /// Run the task, passing its result into a function - with progress tracking.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="f"></param>
        /// <param name="pb"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task<T> ThenRunWithProgress<T>(this Task<T> t, Func<T> f, ProgressBase pb, string taskName = "Unspecified Function")
        {
            return t.ContinueWith(pb.Run(f, taskName));
        }

        /// <summary>
        /// After the task, run an action - with progress tracking.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="a"></param>
        /// <param name="pb"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task ThenDoWithProgress(this Task t, Action a, ProgressBase pb, string taskName = "Unspecified Action")
        {
            return t.ContinueWith(pb.Do(a, taskName));
        }

        /// <summary>
        /// After the task, discard the result and run an action - with progress tracking.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="a"></param>
        /// <param name="pb"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task ThenDoWithProgress<T>(this Task<T> t, Action a, ProgressBase pb, string taskName = "Unspecified Action")
        {
            return t.ContinueWith(pb.Do(a, taskName));
        }

        /// <summary>
        /// After the task, passes its result to an action - with progress tracking.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="a"></param>
        /// <param name="pb"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task ThenPipeDoWithProgress<T>(this Task<T> t, Action<T> a, ProgressBase pb, string taskName = "Unspecified Action")
        {
            return t.ContinueWith(x => pb.PipeDo(a, x.Result, taskName)).Unwrap();
        }

        #endregion

        #region Continuations (From Synchronization Context)

        /// <summary>
        /// Chains a new task created from an action onto an existing task, and runs that action in the synchronization context.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        internal static Task ThenDoFromCurrentSynchronizationContext(this Task t, Action a)
        {
            return t.ContinueWith(x => a, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Chains a new task created from a function onto an existing task, and runs that function in the synchronization context.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        internal static Task<T> ThenRunFromCurrentSynchronizationContext<T>(this Task t, Func<T> f)
        {
            return t.ContinueWith(x => f(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Chains a new task created from an action onto an existing task, passing the output from the original task into the input of the action, which is run in the synchronization context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        internal static Task ThenPipeDoFromCurrentSynchronizationContext<T>(this Task<T> t, Action<T> a)
        {
            return t.ContinueWith(x => a(x.Result), TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Chains a new task created from a function onto an existing task, passing the output from the first task into the input of the function (run in the synchronization context).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        internal static Task<T> ThenPipeFromCurrentSynchronizationContext<T>(this Task<T> t, Func<T, T> f)
        {
            return t.ContinueWith(x => f(x.Result), TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Chains a new task created from a function which returns a task onto an existing task, passing the output from the first task into the function, generating a task which will take the original task's output as input and be run in the synchronization context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        internal static Task<T> ThenPipeFromCurrentSynchronizationContext<T>(this Task<T> t, Func<Task<T>, T> f)
        {
            return t.ContinueWith(f, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Chains a new task created from a function onto an existing task, passing the output from the first task into the input of the function (run in the synchronization context) - this variant accepts a different end result type for the resulting task.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="t"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        internal static Task<R> ThenPipeFromCurrentSynchronizationContext<T, R>(this Task<T> t, Func<T, R> f)
        {
            return t.ContinueWith(x => f(x.Result), TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Chains a new task created from a function which returns a task onto an existing task, passing the output from the first task into the function, generating a task which will take the original task's output as input and be run in the synchronization context - this variant accepts a different end result type for the resulting task.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        internal static Task<R> ThenPipeFromCurrentSynchronizationContext<T, R>(this Task<T> t, Func<Task<T>, R> f)
        {
            return t.ContinueWith(f, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion

        #region Progress Task Wrappers

        public static Task<R> Pipe<T, R>(this ProgressBase pb, Func<T, R> f, T x, string taskName = "Unspecified Pipe")
        {
            if (pb is LoggedProgress)
            {
                var p = pb as LoggedProgress;
                return Run<R>(() =>
                {
                    p.Log(taskName + "...");
                    var result = f(x);
                    p.Next(taskName + " Done!");
                    return result;
                });
            }
            else if (pb is StagedProgress)
            {
                var p = pb as StagedProgress;
                p.Add(taskName);
                return Run<R>(() =>
                {
                    var result = f(x);
                    p.CompleteStage(taskName);
                    return result;
                });
            }
            else
            {
                throw new ArgumentException("Unsupported ProgressBase subclass.", "pb");
            }
        }

        public static Task<T> Run<T>(this ProgressBase pb, Func<T> f, string taskName = "Unspecified Function")
        {
            if (pb is LoggedProgress)
            {
                var p = pb as LoggedProgress;
                return Task.Run<T>(() =>
                    {
                        p.Log(taskName + "...");
                        var result = f();
                        p.Next(taskName + " Done!");
                        return result;
                    });
            }
            else if (pb is StagedProgress)
            {
                var p = pb as StagedProgress;
                p.Add(taskName);
                return Run<T>(() =>
                {
                    var result = f();
                    p.CompleteStage(taskName);
                    return result;
                });
            }
            else
            {
                throw new ArgumentException("Unsupported ProgressBase subclass.", "pb");
            }
        }

        public static Task Do(this ProgressBase pb, Action a, string taskName = "Unspecified Action")
        {
            if (pb is LoggedProgress)
            {
                var p = pb as LoggedProgress;
                return Task.Run(() =>
                {
                    p.Log(taskName + "...");
                    a();
                    p.Next(taskName + " Done!");
                });
            }
            else if (pb is StagedProgress)
            {
                var p = pb as StagedProgress;
                p.Add(taskName);
                return Task.Run(() =>
                {
                    a();
                    p.CompleteStage(taskName);
                });
            }
            else
            {
                throw new ArgumentException("Unsupported ProgressBase subclass.", "pb");
            }
        }

        public static Task PipeDo<T>(this ProgressBase pb, Action<T> a, T x, string taskName = "Unspecified Action")
        {
            if (pb is LoggedProgress)
            {
                var p = pb as LoggedProgress;
                return Task.Run(() =>
                {
                    p.Log(taskName + "...");
                    a(x);
                    p.Next(taskName + " Done!");
                });
            }
            else if (pb is StagedProgress)
            {
                var p = pb as StagedProgress;
                p.Add(taskName);
                return Task.Run(() =>
                {
                    a(x);
                    p.CompleteStage(taskName);
                });
            }
            else
            {
                throw new ArgumentException("Unsupported ProgressBase subclass.", "pb");
            }
        }

        #endregion

        #region Progress Task Wrappers With Cancellation

        public static Task<R> Pipe<T, R>(this ProgressBase pb, Func<T, R> f, CancellationToken token, T x, string taskName = "Unspecified Pipe")
        {
            if (pb is LoggedProgress)
            {
                var p = pb as LoggedProgress;
                return Run<R>(() =>
                {
                    if (token.IsCancellationRequested)
                    {
                        p.Log(taskName + " is cancelled, skipping...");
                        return default(R);
                    }
                    else
                    {
                        p.Log(taskName + "...");
                        var result = f(x);
                        p.Next(taskName + " Done!");
                        return result;
                    }
                });
            }
            else if (pb is StagedProgress)
            {
                var p = pb as StagedProgress;
                p.Add(taskName);
                return Run<R>(() =>
                {
                    if (token.IsCancellationRequested)
                    {
                        p.Remove(taskName);
                        return default(R);
                    }
                    else
                    {
                        var result = f(x);
                        p.CompleteStage(taskName);
                        return result;
                    }
                });
            }
            else
            {
                throw new ArgumentException("Unsupported ProgressBase subclass.", "pb");
            }
        }

        public static Task<T> Run<T>(this ProgressBase pb, Func<T> f, CancellationToken token, string taskName = "Unspecified Function")
        {
            if (pb is LoggedProgress)
            {
                var p = pb as LoggedProgress;
                return Task.Run<T>(() =>
                {
                    if (token.IsCancellationRequested)
                    {
                        p.Log(taskName + " is cancelled, skipping...");
                        return default(T);
                    }
                    else
                    {
                        if (taskName != null)
                            p.Log(taskName + "...");
                        var result = f();
                        p.Next(taskName != null ? (taskName + " Done!") : null);
                        return result;
                    }
                });
            }
            else if (pb is StagedProgress)
            {
                var p = pb as StagedProgress;
                p.Add(taskName);
                return Run<T>(() =>
                {
                    if (token.IsCancellationRequested)
                    {
                        p.Remove(taskName);
                        return default(T);
                    }
                    else
                    {
                        var result = f();
                        p.CompleteStage(taskName);
                        return result;
                    }
                });
            }
            else
            {
                throw new ArgumentException("Unsupported ProgressBase subclass.", "pb");
            }
        }

        public static Task Do(this ProgressBase pb, Action a, CancellationToken token, string taskName = "Unspecified Action")
        {
            if (pb is LoggedProgress)
            {
                var p = pb as LoggedProgress;
                return Task.Run(() =>
                {
                    if (token.IsCancellationRequested)
                    {
                        p.Next(taskName + " is cancelled, skipping...");
                        return;
                    }
                    else
                    {
                        p.Log(taskName + "...");
                        a();
                        p.Next(taskName + " Done!");
                    }
                });
            }
            else if (pb is StagedProgress)
            {
                var p = pb as StagedProgress;
                p.Add(taskName);
                return Task.Run(() =>
                {
                    if (token.IsCancellationRequested)
                    {
                        p.Remove(taskName);
                        return;
                    }
                    else
                    {
                        a();
                        p.CompleteStage(taskName);
                    }
                });
            }
            else
            {
                throw new ArgumentException("Unsupported ProgressBase subclass.", "pb");
            }
        }

        public static Task PipeDo<T>(this ProgressBase pb, Action<T> a, CancellationToken token, T x, string taskName = "Unspecified Action")
        {
            if (pb is LoggedProgress)
            {
                var p = pb as LoggedProgress;
                return Task.Run(() =>
                {
                    if (token.IsCancellationRequested)
                    {
                        p.Log(taskName + " is cancelled, skipping...");
                        return;
                    }
                    else
                    {
                        p.Log(taskName + "...");
                        a(x);
                        p.Next(taskName + " Done!");
                    }
                });
            }
            else if (pb is StagedProgress)
            {
                var p = pb as StagedProgress;
                p.Add(taskName);
                return Task.Run(() =>
                {
                    if (token.IsCancellationRequested)
                    {
                        p.Remove(taskName);
                        return;
                    }
                    else
                    {
                        a(x);
                        p.CompleteStage(taskName);
                    }
                });
            }
            else
            {
                throw new ArgumentException("Unsupported ProgressBase subclass.", "pb");
            }
        }

        #endregion

        #region AfterTasks

        public static Task DoAfterTasks(Action a, params Task[] tasks)
        {
            return Task.WhenAll(tasks).ThenDo(a);
        }
        public static Task PipeDoAfterTasks<T>(Action<T[]> a, params Task<T>[] tasks)
        {
            return Task.WhenAll(tasks).ThenPipeDo(a);
        }

        public static Task<T> RunAfterTasks<T>(Func<T> f, params Task[] tasks)
        {
            return Task.WhenAll(tasks).ThenRun(f);
        }

        #endregion

        #region AfterTasks (With Progress)

        public static Task<T> RunAfterTasks<T>(this ProgressBase pb, Func<T> f, IList<Task<T>> tasks, string taskName = "Unspecified Coalescing Function")
        {
            return Task.WhenAll(tasks).ThenRunWithProgress(f, pb, taskName);
        }

        public static Task PipeDoAfterTasks<T>(this ProgressBase pb, Action<T[]> a, IList<Task<T>> tasks, string taskName = "Unspecified Coalescing Action")
        {
            return AwaitAll(tasks).ThenPipeDoWithProgress(a, pb, taskName);
        }

        public static Task DoAfterTasks(this ProgressBase pb, Action a, params Task[] tasks)
        {
            return Task.WhenAll(tasks).ThenDoWithProgress(a, pb);
        }

        public static Task<R> PipeAfterTasks<T, R>(this ProgressBase pb, Func<T[], R> f, IList<Task<T>> tasks, string taskName = "Unspecified Coalescing Pipe")
        {
            return AwaitAll(tasks).ThenPipeWithProgress(f, pb, taskName);
        }

        #endregion

        #region AfterTasks (With Cancellation)

        public static Task DoAfterTasksWithCancellation(Action a, CancellationToken token, params Task[] tasks)
        {
            if (token.IsCancellationRequested)
                return null;
            return Task.WhenAll(tasks.WithCancellation(token)).ThenDo(a);
        }
        public static Task PipeDoAfterTasksWithCancellation<T>(Action<T[]> a, CancellationToken token, params Task<T>[] tasks)
        {
            if (token.IsCancellationRequested)
                return null;
            return Task.WhenAll(tasks.WithCancellation(token)).ThenPipeDo(a);
        }

        public static Task<T> RunAfterTasksWithCancellation<T>(Func<T> f, CancellationToken token, params Task[] tasks)
        {
            if (token.IsCancellationRequested)
                return null;
            return Task.WhenAll(tasks.WithCancellation(token)).ThenRun(f);
        }

        #endregion

        #region AfterTasks (With Progress & Cancellation)

        public static Task<T> RunAfterTasksWithCancellation<T>(this ProgressBase pb, Func<T> f, CancellationToken token, IList<Task<T>> tasks, string taskName = "Unspecified Coalescing Function")
        {
            if (token.IsCancellationRequested)
                return null;
            return Task.WhenAll(tasks.WithCancellation(token)).ThenRunWithProgress(f, pb, taskName);
        }

        public static Task PipeDoAfterTasksWithCancellation<T>(this ProgressBase pb, Action<T[]> a, CancellationToken token, IList<Task<T>> tasks, string taskName = "Unspecified Coalescing Action")
        {
            if (token.IsCancellationRequested)
                return null;
            return AwaitAll(tasks.WithCancellation(token)).ThenPipeDoWithProgress(a, pb, taskName);
        }

        public static Task DoAfterTasksWithCancellation(this ProgressBase pb, Action a, CancellationToken token, params Task[] tasks)
        {
            if (token.IsCancellationRequested)
                return null;
            return Task.WhenAll(tasks.WithCancellation(token)).ThenDoWithProgress(a, pb);
        }

        public static Task<R> PipeAfterTasksWithCancellation<T, R>(this ProgressBase pb, Func<T[], R> f, CancellationToken token, IList<Task<T>> tasks, string taskName = "Unspecified Coalescing Pipe")
        {
            if (token.IsCancellationRequested)
                return null;
            return AwaitAll(tasks.WithCancellation(token)).ThenPipeWithProgress(f, pb, taskName);
        }

        #endregion

        #region AfterTasks (From Synchronization Context)

        /// <summary>
        /// After a set of tasks with return values, perform an action from the synchronization context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="tasks"></param>
        /// <returns></returns>
        internal static Task DoFromSynchronizationContextAfterTasks<T>(Action a, params Task<T>[] tasks)
        {
            return AwaitAll<T>(tasks).ThenDoFromCurrentSynchronizationContext(a);
        }

        /// <summary>
        /// After a set of tasks with return values, run a function from the synchronization context that returns a different type of output from the tasks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="f"></param>
        /// <param name="tasks"></param>
        /// <returns></returns>
        internal static Task<R> RunFromSynchronizationContextAfterTasks<T, R>(Func<R> f, params Task<T>[] tasks)
        {
            return AwaitAll<T>(tasks).ThenRunFromCurrentSynchronizationContext(f);
        }

        /// <summary>
        /// After a set of tasks with return values, perform an action from the synchronization context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="tasks"></param>
        /// <returns></returns>
        internal static Task DoFromSynchronizationContextAfterTasks<T>(Action a, IList<Task<T>> tasks)
        {
            return AwaitAll(tasks).ThenDoFromCurrentSynchronizationContext(a);
        }

        /// <summary>
        /// After a set of tasks with return values, perform an action from the synchronization context that returns a different type of output from the tasks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="f"></param>
        /// <param name="tasks"></param>
        /// <returns></returns>
        internal static Task<R> RunFromSynchronizationContextAfterTasks<T, R>(Func<R> f, IList<Task<T>> tasks)
        {
            return AwaitAll(tasks).ThenRunFromCurrentSynchronizationContext(f);
        }

        /// <summary>
        /// After a set of tasks with return values, passes those return values into an action which operates on them from the synchronization context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="tasks"></param>
        /// <returns></returns>
        internal static Task PipeDoFromSynchronizationContextAfterTasks<T>(Action<T[]> a, params Task<T>[] tasks)
        {
            return AwaitAll<T>(tasks).ThenPipeDoFromCurrentSynchronizationContext(a);
        }

        /// <summary>
        /// After a set of tasks with return values, passes those return values into a function which operates on them from the synchronization context and returns a result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="f"></param>
        /// <param name="tasks"></param>
        /// <returns></returns>
        internal static Task<R> PipeFromSynchronizationContextAfterTasks<T, R>(Func<T[], R> f, params Task<T>[] tasks)
        {
            return AwaitAll<T>(tasks).ThenPipeFromCurrentSynchronizationContext(f);
        }

        /// <summary>
        /// After a set of tasks with return values, passes those return values into an action which operates on them from the synchronization context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="tasks"></param>
        /// <returns></returns>
        internal static Task PipeDoFromSynchronizationContextAfterTasks<T>(Action<T[]> a, IList<Task<T>> tasks)
        {
            return AwaitAll(tasks).ThenPipeDoFromCurrentSynchronizationContext(a);
        }

        /// <summary>
        /// After a set of tasks with return values, passes those return values into a function which operates on them from the synchronization context and returns a result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="f"></param>
        /// <param name="tasks"></param>
        /// <returns></returns>
        internal static Task<R> PipeFromSynchronizationContextAfterTasks<T, R>(Func<T[], R> f, IList<Task<T>> tasks)
        {
            return AwaitAll(tasks).ThenPipeFromCurrentSynchronizationContext(f);
        }

        #endregion

        #region AfterTasks (Named*Collection)

        /// <summary>
        /// After a set of tasks with results and progress tracking, passes the results into a function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="pb"></param>
        /// <param name="afterFunction"></param>
        /// <param name="taskFunctions"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task<R> PipeAfterTasks<T, R>(this ProgressBase pb, Func<T[], R> afterFunction, NamedFunctionCollection<T> taskFunctions, string taskName = "Unspecified Coalescing Pipe")
        {
            return taskFunctions.AwaitAllWithProgress(pb, taskName).ThenPipeFromCurrentSynchronizationContext(afterFunction);
        }

        /// <summary>
        /// After a set of tasks with progress tracking, performs an action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pb"></param>
        /// <param name="afterFunction"></param>
        /// <param name="taskActions"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task DoAfterTasks<T>(this ProgressBase pb, Action afterFunction, NamedActionCollection taskActions, string taskName = "Unspecified Coalescing Action")
        {
            return taskActions.AwaitAllWithProgress(pb, taskName).ThenDoFromCurrentSynchronizationContext(afterFunction);
        }

        /// <summary>
        /// After a set of tasks with results and progress tracking, passes the results into an action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pb"></param>
        /// <param name="afterFunction"></param>
        /// <param name="taskFunctions"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task PipeDoAfterTasks<T>(this ProgressBase pb, Action<T[]> afterFunction, NamedFunctionCollection<T> taskFunctions, string taskName = "Unspecified Coalescing Action")
        {
            return taskFunctions.AwaitAllWithProgress(pb, taskName).ThenPipeDoFromCurrentSynchronizationContext(afterFunction);
        }

        /// <summary>
        /// After a set of tasks with results and progress tracking, passes the results into a function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="pb"></param>
        /// <param name="afterFunction"></param>
        /// <param name="taskFunctions"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task<R> RunAfterTasks<T, R>(this ProgressBase pb, Func<R> afterFunction, NamedFunctionCollection<T> taskFunctions, string taskName = "Unspecified Coalescing Function")
        {
            return taskFunctions.AwaitAllWithProgress(pb, taskName).ThenRunFromCurrentSynchronizationContext(afterFunction);
        }

        #endregion

        #region AfterTasks (Named*Collection With Cancellation)

        /// <summary>
        /// After a set of tasks with results and progress tracking, passes the results into a function - supports cancellation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="pb"></param>
        /// <param name="afterFunction"></param>
        /// <param name="token"></param>
        /// <param name="taskFunctions"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task<R> PipeAfterTasksWithCancellation<T, R>(this ProgressBase pb, Func<T[], R> afterFunction, CancellationToken token, NamedFunctionCollection<T> taskFunctions, string taskName = "Unspecified Coalescing Pipe")
        {
            return taskFunctions.AwaitAllWithProgressAndCancellation(pb, taskName, token).ThenPipeFromCurrentSynchronizationContext(afterFunction);
        }

        /// <summary>
        /// After a set of tasks with results and progress tracking, passes the results into a action - supports cancellation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pb"></param>
        /// <param name="afterFunction"></param>
        /// <param name="token"></param>
        /// <param name="taskFunctions"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task DoAfterTasksWithCancellation<T>(this ProgressBase pb, Action afterAction, CancellationToken token, NamedActionCollection taskActions, string taskName = "Unspecified Coalescing Action")
        {
            return taskActions.AwaitAllWithProgressAndCancellation(pb, taskName, token).ThenDoFromCurrentSynchronizationContext(afterAction);
        }

        /// <summary>
        /// After a set of tasks with results and progress tracking, passes the results into a action - supports cancellation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pb"></param>
        /// <param name="afterFunction"></param>
        /// <param name="token"></param>
        /// <param name="taskFunctions"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task PipeDoAfterTasksWithCancellation<T>(this ProgressBase pb, Action<T[]> afterAction, CancellationToken token, NamedFunctionCollection<T> taskFunctions, string taskName = "Unspecified Coalescing Action")
        {
            return taskFunctions.AwaitAllWithProgressAndCancellation(pb, taskName, token).ThenPipeDoFromCurrentSynchronizationContext(afterAction);
        }

        /// <summary>
        /// After a set of tasks with results and progress tracking, passes the results into a function - supports cancellation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="pb"></param>
        /// <param name="afterFunction"></param>
        /// <param name="token"></param>
        /// <param name="taskFunctions"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task<R> RunAfterTasksWithCancellation<T, R>(this ProgressBase pb, Func<R> afterFunction, CancellationToken token, NamedFunctionCollection<T> taskFunctions, string taskName = "Unspecified Coalescing Function")
        {
            return taskFunctions.AwaitAllWithProgressAndCancellation(pb, taskName, token).ThenRunFromCurrentSynchronizationContext(afterFunction);
        }

        #endregion

        #region AwaitAll

        /// <summary>
        /// Returns a task that represents the completion of an IList of tasks with results.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static Task<T[]> AwaitAll<T>(this IList<Task<T>> tasks)
        {
            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Returns a task that represents the completion of an IList of tasks.
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static Task AwaitAll(this IList<Task> tasks)
        {
            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Returns a task that represents a progress-tracked set of actions being completed.
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="actions"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task AwaitAll(this ProgressBase pb, NamedActionCollection actions, string taskName = "Unspecified Action Block")
        {
            return Tasking.AwaitAll(actions.GetTasksWithProgress(pb));
        }

        /// <summary>
        /// Returns a task that represents a progress-tracked set of actions on a provided value being completed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pb"></param>
        /// <param name="actions"></param>
        /// <param name="x"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task AwaitAll<T>(this ProgressBase pb, NamedActionCollection<T> actions, T x, string taskName = "Unspecified Action Block")
        {
            return Tasking.AwaitAll(actions.GetTasksWithProgress(pb, x));
        }

        /// <summary>
        /// Returns a task that represents a progress-tracked set of functions operating on a value and returning their outputs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="pb"></param>
        /// <param name="functions"></param>
        /// <param name="x"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task<R[]> AwaitAll<T, R>(this ProgressBase pb, NamedFunctionCollection<T, R> functions, T x, string taskName = "Unspecified Pipe Block")
        {
            return Tasking.AwaitAll(functions.GetTasksWithProgress(pb, x));
        }

        /// <summary>
        /// Returns a task that represents a progress-tracked set of functions and returning their outputs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pb"></param>
        /// <param name="functions"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task<T[]> AwaitAll<T>(this ProgressBase pb, NamedFunctionCollection<T> functions, string taskName = "Unspecified Function Block")
        {
            return Tasking.AwaitAll(functions.GetTasksWithProgress(pb));
        }

        /// <summary>
        /// Returns a task that represents a progress-tracked set of actions with cancellation support via the provided token.
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="actions"></param>
        /// <param name="token"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task AwaitAll(this ProgressBase pb, NamedActionCollection actions, CancellationToken token, string taskName = "Unspecified Action Block")
        {
            return Tasking.AwaitAll(actions.GetTasksWithProgressAndCancellation(pb, token));
        }

        /// <summary>
        /// Returns a task that represents a progress-tracked set of actions being run on the provided value, with cancellation support via the provided token.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pb"></param>
        /// <param name="actions"></param>
        /// <param name="token"></param>
        /// <param name="x"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task AwaitAll<T>(this ProgressBase pb, NamedActionCollection<T> actions, CancellationToken token, T x, string taskName = "Unspecified Action Block")
        {
            return Tasking.AwaitAll(actions.GetTasksWithProgressAndCancellation(pb, token, x));
        }

        /// <summary>
        /// Returns a task that represents a progress-tracked set of functions being run on the provided value, with cancellation support via the provided token, and returning their outputs (of a different type than the inputs).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="pb"></param>
        /// <param name="functions"></param>
        /// <param name="token"></param>
        /// <param name="x"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task<R[]> AwaitAll<T, R>(this ProgressBase pb, NamedFunctionCollection<T, R> functions, CancellationToken token, T x, string taskName = "Unspecified Pipe Block")
        {
            return Tasking.AwaitAll(functions.GetTasksWithProgressAndCancellation(pb, token, x));
        }

        /// <summary>
        /// Returns a task that represents a progress-tracked set of functions being run on the provided value, with cancellation support via the provided token, and returning their outputs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pb"></param>
        /// <param name="functions"></param>
        /// <param name="token"></param>
        /// <param name="x"></param>
        /// <param name="taskName"></param>
        /// <returns></returns>
        public static Task<T[]> AwaitAll<T>(this ProgressBase pb, NamedFunctionCollection<T> functions, CancellationToken token, string taskName = "Unspecified Function Block")
        {
            return Tasking.AwaitAll(functions.GetTasksWithProgressAndCancellation(pb, token));
        }

        /// <summary>
        /// Returns a task that represents a progress-tracked set of other tasks.
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static Task AwaitAll(this ProgressBase pb, IList<Task> tasks)
        {
            return Task.WhenAll(tasks);
        }

        #endregion
    }

    #region Named Action/Function Collection Classes

    /// <summary>
    /// This type represents a set of actions with associated names - it is intended to be instantiated via the "new {}" syntax without type specified.
    /// </summary>
    public class NamedActionCollection : Dictionary<string, Action>, IDictionary<string, Action>
    {
        public NamedActionCollection() { }

        public List<Task> GetTasksWithProgressAndCancellation(ProgressBase pb, CancellationToken token)
        {
            List<Task> tasks = new List<Task>();
            foreach (var task in this)
            {
                tasks.Add(pb.Do(task.Value, token, task.Key));
            }
            return tasks;
        }

        public List<Task> GetTasksWithProgress(ProgressBase pb)
        {
            List<Task> tasks = new List<Task>();
            foreach (var task in this)
            {
                tasks.Add(pb.Do(task.Value, task.Key));
            }
            return tasks;
        }

        public List<Task> GetTasks()
        {
            List<Task> tasks = new List<Task>();
            foreach (var task in this)
            {
                tasks.Add(Tasking.Do(task.Value));
            }
            return tasks;
        }
        public Task GetTaskWithProgress(ProgressBase pb, string name)
        {
            return pb.Do(this[name], name);
        }

        public Task GetTask(string name)
        {
            return Tasking.Do(this[name]);
        }

        public Task AwaitAll()
        {
            return Task.WhenAll(GetTasks());
        }

        public Task AwaitAllWithProgress(ProgressBase pb, string name)
        {
            return pb.AwaitAll(this, name);
        }

        public Task AwaitAllWithProgressAndCancellation(ProgressBase pb, string name, CancellationToken token)
        {
            return pb.AwaitAll(this, token, name);
        }
    }

    /// <summary>
    /// This type represents a set of functions with associated names - it is intended to be instantiated via the "new {}" syntax without type specified.
    /// </summary>
    public class NamedFunctionCollection<T> : Dictionary<string, Func<T>>, IDictionary<string, Func<T>>
    {
        public NamedFunctionCollection() { }

        public List<Task<T>> GetTasksWithProgressAndCancellation(ProgressBase pb, CancellationToken token)
        {
            List<Task<T>> tasks = new List<Task<T>>();
            foreach (var task in this)
            {
                tasks.Add(pb.Run(task.Value, token, task.Key));
            }
            return tasks;
        }

        public List<Task<T>> GetTasksWithProgress(ProgressBase pb)
        {
            List<Task<T>> tasks = new List<Task<T>>();
            foreach (var task in this)
            {
                tasks.Add(pb.Run(task.Value, task.Key));
            }
            return tasks;
        }

        public List<Task<T>> GetTasks()
        {
            List<Task<T>> tasks = new List<Task<T>>();
            foreach (var task in this)
            {
                tasks.Add(Tasking.Run(task.Value));
            }
            return tasks;
        }

        public Task<T> GetTaskWithProgress(ProgressBase pb, string name)
        {
            return pb.Run(this[name], name);
        }

        public Task<T> GetTask(string name)
        {
            return Tasking.Run(this[name]);
        }

        public Task<T[]> AwaitAll()
        {
            return Tasking.AwaitAll(GetTasks());
        }

        public Task<T[]> AwaitAllWithProgress(ProgressBase pb, string name)
        {
            return pb.AwaitAll(this, name);
        }

        public Task<T[]> AwaitAllWithProgressAndCancellation(ProgressBase pb, string name, CancellationToken token)
        {
            return pb.AwaitAll(this, token, name);
        }
    }

    /// <summary>
    /// This type represents a set of actions that take input with associated names - it is intended to be instantiated via the "new {}" syntax without type specified.
    /// </summary>
    public class NamedActionCollection<T> : Dictionary<string, Action<T>>, IDictionary<string, Action<T>>
    {
        public NamedActionCollection() { }

        public List<Task> GetTasksWithProgressAndCancellation(ProgressBase pb, CancellationToken token, T x)
        {
            List<Task> tasks = new List<Task>();
            foreach (var task in this)
            {
                tasks.Add(pb.PipeDo(task.Value, token, x, task.Key));
            }
            return tasks;
        }

        public List<Task> GetTasksWithProgress(ProgressBase pb, T x)
        {
            List<Task> tasks = new List<Task>();
            foreach (var task in this)
            {
                tasks.Add(pb.PipeDo(task.Value, x, task.Key));
            }
            return tasks;
        }

        public List<Task> GetTasks(T x)
        {
            List<Task> tasks = new List<Task>();
            foreach (var task in this)
            {
                tasks.Add(Tasking.PipeDo(task.Value, x));
            }
            return tasks;
        }
        public Task GetTaskWithProgress(ProgressBase pb, string name, T x)
        {
            return pb.PipeDo(this[name], x, name);
        }

        public Task GetTask(string name, T x)
        {
            return Tasking.PipeDo(this[name], x);
        }

        public Task AwaitAll(T x)
        {
            return Task.WhenAll(GetTasks(x));
        }

        public Task AwaitAllWithProgress(ProgressBase pb, T x, string name)
        {
            return pb.AwaitAll(this, x, name);
        }
        public Task AwaitAllWithProgressAndCancellation(ProgressBase pb, CancellationToken token, T x, string name)
        {
            return pb.AwaitAll(this, token, x, name);
        }
    }

    /// <summary>
    /// This type represents a set of functions that take input with associated names - it is intended to be instantiated via the "new {}" syntax without type specified.
    /// </summary>
    public class NamedFunctionCollection<T, R> : Dictionary<string, Func<T, R>>, IDictionary<string, Func<T, R>>
    {
        public NamedFunctionCollection() { }

        public List<Task<R>> GetTasksWithProgressAndCancellation(ProgressBase pb, CancellationToken token, T x)
        {
            List<Task<R>> tasks = new List<Task<R>>();
            foreach (var task in this)
            {
                tasks.Add(pb.Pipe(task.Value, token, x, task.Key));
            }
            return tasks;
        }

        public List<Task<R>> GetTasksWithProgress(ProgressBase pb, T x)
        {
            List<Task<R>> tasks = new List<Task<R>>();
            foreach (var task in this)
            {
                tasks.Add(pb.Pipe(task.Value, x, task.Key));
            }
            return tasks;
        }

        public List<Task<R>> GetTasks(T x)
        {
            List<Task<R>> tasks = new List<Task<R>>();
            foreach (var task in this)
            {
                tasks.Add(Tasking.Pipe(task.Value, x));
            }
            return tasks;
        }

        public Task<R> GetTaskWithProgress(ProgressBase pb, string name, T x)
        {
            return pb.Pipe(this[name], x, name);
        }

        public Task<R> GetTask(string name, T x)
        {
            return Tasking.Pipe(this[name], x);
        }

        public Task<R[]> AwaitAll(T x)
        {
            return Tasking.AwaitAll(GetTasks(x));
        }

        public Task<R[]> AwaitAllWithProgress(ProgressBase pb, T x, string name)
        {
            return pb.AwaitAll(this, x, name);
        }
        public Task<R[]> AwaitAllWithProgressAndCancellation(ProgressBase pb, CancellationToken token, T x, string name)
        {
            return pb.AwaitAll(this, token, x, name);
        }
    }

    #endregion

    #region Progress Classes

    /// <summary>
    /// A progress tracker that operates on named stages.
    /// </summary>
    public class StagedProgress : ProgressBase
    {
        private Dictionary<string, bool> _stages = new Dictionary<string, bool>();

        public void Add(string stage)
        {
            _stages.Add(stage, false);
            _totalCount++;
        }

        public void AddRange(string[] stages)
        {
            foreach (string name in stages)
            {
                Add(name);
            }
        }

        public void Remove(string stage)
        {
            _stages.Remove(stage);
            _totalCount--;
        }

        public void CompleteStage(string stage)
        {
            _stages[stage] = true;
            _completeCount++;
            raiseOnProgress(this);
            if (_automaticDone)
            {
                if (_stages.All(s => s.Value)) //If all stages are done..
                {
                    _complete = true;
                    raiseOnDone(this);
                }
            }
        }

        public bool CheckStage(string stage)
        {
            return _stages[stage];
        }

        public IEnumerable<string> GetStages()
        {
            return _stages.Select(s => s.Key);
        }

        public StagedProgress(params string[] stages)
        {
            if (stages != null)
                AddRange(stages);
        }
    }

    /// <summary>
    /// A progress tracker that operates via logging output.
    /// </summary>
    public class LoggedProgress : ProgressBase
    {
        public event OnLogEventHandler OnLog;

        public delegate void OnLogEventHandler(LoggedProgress p, OnLogEventArgs e);
        public class OnLogEventArgs : EventArgs
        {
            public LoggedProgress Progress;
            public string Message;
            public bool NewLine = true;
        }

        public void Log(string message, bool newLine = true)
        {
            if (OnLog != null)
                OnLog(this, new OnLogEventArgs
                {
                    Progress = this,
                    Message = message,
                    NewLine = newLine,
                });
        }

        public LoggedProgress(int totalCount)
        {
            _totalCount = totalCount;
            _automaticDone = true;
        }

        public void Next(string message = null, bool newLine = true)
        {
            _completeCount++;
            if (_automaticDone)
            {
                if (_completeCount == _totalCount)
                {
                    _complete = true;
                    raiseOnDone(this);
                }
            }
            if (OnLog != null && message != null)
                Log(message, newLine);
            raiseOnProgress(this);
        }
    }

    /// <summary>
    /// The base class for progress trackers.
    /// </summary>
    public abstract class ProgressBase
    {
        public event Action<ProgressBase> OnDone;
        public event Action<ProgressBase> OnProgress;

        protected bool _automaticDone = false;

        public ProgressBase()
        {
            OnDone += OnDone;
            OnProgress += OnProgress;
        }

        public void Done()
        {
            _complete = true;
            raiseOnDone(this);
        }

        protected virtual void raiseOnProgress(ProgressBase p)
        {
            if (OnProgress != null)
                OnProgress(this);
        }

        protected virtual void raiseOnDone(ProgressBase p)
        {
            if (OnDone != null)
                OnDone(this);
        }

        protected bool _complete = false;
        bool IsDone
        {
            get
            {
                return _complete;
            }
        }
        protected double _completeCount = 0;
        protected double _totalCount = 1;

        public double Percentage
        {
            get
            {
                return FractionalPercentage * 100;
            }
        }

        public double FractionalPercentage
        {
            get
            {
                return _completeCount / _totalCount;
            }
        }
    }

    #endregion
}
