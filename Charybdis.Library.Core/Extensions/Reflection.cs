using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel;

namespace Charybdis.Library.Core
{
    /// <summary>
    /// This class encapsulates reflection extensions to LINQ that will operate on any data type.
    /// </summary>
    public static class Reflection
    {
        public static string GetShortName(this Assembly assembly)
        {
            return assembly.FullName.Split(",")[0];
        }

        /// <summary>
        /// Stores CachedType objects per-type to avoid having to gather that information more than once per type.
        /// </summary>
        private static Dictionary<Type, CachedType> _typeInfoCache = new Dictionary<Type, CachedType>();

        /// <summary>
        /// Retrieve a CachedType object for the provided type - tries the cache first, then if not found it constructs a new one (and adds it to the cache).
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static CachedType _getCachedOrNewCachedType(Type t)
        {
            if (_typeInfoCache.ContainsKey(t))
                return _typeInfoCache[t];
            else
                return _typeInfoCache[t] = new CachedType();
        }

        /// <summary>
        /// Clone the object via reflection (field and property values transferred to a new T).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T CloneViaReflection<T>(this T original)
            where T : class, new()
        {
            if (original == null) //If it's just null..
                return null; //No need to do the work, just return null.

            T clone = new T();
            CachedType ti = _getCachedOrNewCachedType(typeof(T));

            if (ti.Properties == null)
                ti.Properties = typeof(T).GetProperties();
            foreach (PropertyInfo pi in ti.Properties)
            {
                if (pi.CanWrite)
                    pi.SetValue(clone, pi.GetValue(original));
            }

            if (ti.Fields == null)
                ti.Fields = typeof(T).GetFields();
            foreach (FieldInfo fi in ti.Fields)
            {
                fi.SetValue(clone, fi.GetValue(original));
            }

            return clone;
        }

        /// <summary>
        /// Replace the values of the fields and properties of the target T with the values of the source T (but maintains the same reference and any other metadata).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public static void ReplaceViaReflection<T>(this T target, T source)
            where T : class, new()
        {
            if (source == null) //If it's just null..
                target = null; //No need to do the work, just make it null.

            CachedType ti = _getCachedOrNewCachedType(typeof(T));

            if (ti.Properties == null)
                ti.Properties = typeof(T).GetProperties();
            foreach (PropertyInfo pi in ti.Properties)
            {
                if (pi.CanWrite)
                    pi.SetValue(target, pi.GetValue(source));
            }

            if (ti.Fields == null)
                ti.Fields = typeof(T).GetFields();
            foreach (FieldInfo fi in ti.Fields)
            {
                fi.SetValue(target, fi.GetValue(source));
            }
        }

        /// <summary>
        /// Replace the values of the fields and properties of the target object with the values of the source T (but maintains the same reference and any other metadata). If the object is not a T, throws an exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public static void ReplaceViaReflection<T>(this object target, T source)
            where T : class, new()
        {
            if (!(target.GetType() is T))
                throw new ArgumentException("The target and source must be compatible types.");

            CachedType ti = _getCachedOrNewCachedType(typeof(T));

            if (ti.Properties == null)
                ti.Properties = typeof(T).GetProperties();
            foreach (PropertyInfo pi in ti.Properties)
            {
                if (pi.CanWrite)
                    pi.SetValue(target, pi.GetValue(source));
            }

            if (ti.Fields == null)
                ti.Fields = typeof(T).GetFields();
            foreach (FieldInfo fi in ti.Fields)
            {
                fi.SetValue(target, fi.GetValue(source));
            }
        }

        /// <summary>
        /// Invokes a method with optional parameters and returns its return value.
        /// </summary>
        /// <typeparam name="T">the type of the field/property/method return</typeparam>
        /// <param name="o">the object to operate on</param>
        /// <param name="memberName">the name of the member to access</param>
        /// <param name="parameters">parameters to pass to the method</param>
        /// <returns>return value of the method</returns>
        /// <exception cref="System.NullReferenceException"/>
        /// <exception cref="System.NotSupportedException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.Reflection.TargetException"/>
        /// <exception cref="System.Reflection.TargetParameterCountException"/>
        /// <exception cref="System.Reflection.TargetInvocationException"/>
        /// <exception cref="System.FieldAccessException"/>
        /// <exception cref="System.MethodAccessException"/>
        /// <exception cref="System.InvalidOperationException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.InvalidCastException"/>
        public static T InvokeViaReflection<T>(this object o, string memberName, params object[] parameters)
        {
            if (o is Type)
            {
                MethodInfo mi = ((Type)o).GetMethod(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                return (T)mi.Invoke(null, parameters);
            }
            else
            {
                MethodInfo mi = (o.GetType() as Type).GetMethod(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                return (T)mi.Invoke(o, parameters);
            }
        }

        /// <summary>
        /// Invokes a method with optional parameters.
        /// </summary>
        /// <typeparam name="T">the type of the field/property/method return</typeparam>
        /// <param name="o">the object to operate on</param>
        /// <param name="memberName">the name of the member to access</param>
        /// <param name="parameters">parameters to pass to the method</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException"/>
        /// <exception cref="System.NotSupportedException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.Reflection.TargetException"/>
        /// <exception cref="System.Reflection.TargetParameterCountException"/>
        /// <exception cref="System.Reflection.TargetInvocationException"/>
        /// <exception cref="System.MethodAccessException"/>
        /// <exception cref="System.InvalidOperationException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.InvalidCastException"/>
        public static void InvokeViaReflection(this object o, string memberName, params object[] parameters)
        {
            if (o is Type)
            {
                ((Type)o).GetMethod(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).Invoke(null, parameters);
            }
            else
            {
                (o.GetType() as Type).GetMethod(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).Invoke(o, parameters);
            }
        }

        /// <summary>
        /// Attempts to return the value of a member of an object known to be a derived type by member name (supports field, property, and invoked parameterless method return).
        /// </summary>
        /// <typeparam name="T">the type of the field/property/method return</typeparam>
        /// <param name="o">the object to operate on</param>
        /// <param name="memberName">the name of the member to access</param>
        /// <param name="defaultOverride">the value to return on failure, defaults to default(T)</param>
        /// <returns>value of the field, property, return value of the method, or defaultOverride if there was a failure</returns>
        public static T GetViaReflection<T>(this object o, string memberName, T defaultOverride = default(T))
        {
            try
            {
                if (o is Type)
                {
                    MemberInfo mi = ((Type)o).GetMember(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).Single();
                    switch (mi.MemberType)
                    {
                        case MemberTypes.Field:
                            return (T)(mi as FieldInfo).GetValue(null); //Return Field Value
                        case MemberTypes.Property:
                            return (T)(mi as PropertyInfo).GetValue(null); //Return Property Value
                        case MemberTypes.Method:
                            return (T)(mi as MethodInfo).Invoke(null, null); //Return Method Return Value
                        default:
                            return defaultOverride; //Return Default, Unsupported Member Type
                    }
                }
                else
                {
                    MemberInfo mi = (o.GetType() as Type).GetMember(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).Single();
                    switch (mi.MemberType)
                    {
                        case MemberTypes.Field:
                            return (T)(mi as FieldInfo).GetValue(o); //Return Field Value
                        case MemberTypes.Property:
                            return (T)(mi as PropertyInfo).GetValue(o); //Return Property Value
                        case MemberTypes.Method:
                            return (T)(mi as MethodInfo).Invoke(o, null); //Return Method Return Value
                        default:
                            return defaultOverride; //Return Default, Unsupported Member Type
                    }
                }
            }
            catch (Exception e)
            {
                e.ThrowIfNot<
                    NullReferenceException,
                    NotSupportedException,
                    ArgumentException,
                    TargetException,
                    TargetParameterCountException,
                    TargetInvocationException,
                    FieldAccessException,
                    MethodAccessException,
                    InvalidOperationException,
                    ArgumentNullException,
                    InvalidCastException>();
                return defaultOverride;
            }
        }

        /// <summary>
        /// Attempts to set the value of a field or property via reflection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <returns>whether the set was successful</returns>
        /// <exception cref="System.NullReferenceException"/>
        /// <exception cref="System.NotSupportedException"/>
        /// <exception cref="System.ArgumentException"/>
        /// <exception cref="System.Reflection.TargetException"/>
        /// <exception cref="System.Reflection.TargetParameterCountException"/>
        /// <exception cref="System.Reflection.TargetInvocationException"/>
        /// <exception cref="System.FieldAccessException"/>
        /// <exception cref="System.MethodAccessException"/>
        /// <exception cref="System.InvalidOperationException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.InvalidCastException"/>
        public static bool SetViaReflection<T>(this object o, string memberName, T value)
        {
            try
            {
                if (o is Type)
                {
                    MemberInfo mi = ((Type)o).GetMember(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).Single();
                    switch (mi.MemberType)
                    {
                        case MemberTypes.Field:
                            (mi as FieldInfo).SetValue(null, value); //Set Field Value
                            break;
                        case MemberTypes.Property:
                            (mi as PropertyInfo).SetValue(null, value); //Set Property Value
                            break;
                        case MemberTypes.Method:
                            throw new ArgumentException("You cannot \"set\" a method.");
                        default:
                            throw new ArgumentException("Unsupported member type.");
                    }
                }
                else
                {
                    MemberInfo mi = (o.GetType() as Type).GetMember(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).Single();
                    switch (mi.MemberType)
                    {
                        case MemberTypes.Field:
                            (mi as FieldInfo).SetValue(o, value); //Set Field Value
                            break;
                        case MemberTypes.Property:
                            (mi as PropertyInfo).SetValue(o, value); //Set Property Value
                            break;
                        case MemberTypes.Method:
                            throw new ArgumentException("You cannot \"set\" a method.");
                        default:
                            throw new ArgumentException("Unsupported member type.");
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                e.ThrowIfNot<
                    NullReferenceException,
                    NotSupportedException,
                    ArgumentException,
                    TargetException,
                    TargetParameterCountException,
                    TargetInvocationException,
                    FieldAccessException,
                    MethodAccessException,
                    InvalidOperationException,
                    ArgumentNullException,
                    InvalidCastException>();
                return false;
            }
        }

        /// <summary>
        /// Preload the assembly dependencies of the provided AppDomain.
        /// </summary>
        /// <param name="appDomain"></param>
        public static void PreloadDependencies(this AppDomain appDomain)
        {
            CachedAssembly.PreloadDependencies(appDomain);
        }
    }

    /// <summary>
    /// A ChangeProxy for a new T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NewProxy<T> : ChangeProxy<T>
        where T : class, new()
    {
        public NewProxy()
            : base()
        {
            Proxy = new T();
        }

        public NewProxy(T proxy)
            : base()
        {
            Proxy = proxy;
        }

        public override bool DirtyCheck()
        {
            return true;
        }

        public override void PropagateChanges()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// A ChangeProxy for a T to delete.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DeleteProxy<T> : ChangeProxy<T>
        where T : class, new()
    {
        public DeleteProxy(T actual)
            : base()
        {
            Actual = actual;
        }

        public override bool DirtyCheck()
        {
            return true;
        }

        public override void PropagateChanges()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// A ChangeProxy, used for updates of a T, and also the base class for other ChangeProxy types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChangeProxy<T> : IDisposable
        where T : class, new()
    {
        private PropertyInfo[] _properties;
        private FieldInfo[] _fields;

        public T Actual { get; internal set; }
        public T Proxy { get; internal set; }

        protected ChangeProxy()
        {

        }

        public ChangeProxy(T actual, T proxy) //For when you have a pre-created proxy.
        {
            _properties = typeof(T).GetProperties();
            _fields = typeof(T).GetFields();
            Actual = actual;
            Proxy = proxy;
        }

        public ChangeProxy(T actual)
        {
            _properties = typeof(T).GetProperties();
            Actual = actual;
            Proxy = new T();
            foreach (PropertyInfo pi in _properties)
            {
                if (pi.CanWrite)
                    pi.SetValue(Proxy, pi.GetValue(Actual));
            }
            _fields = typeof(T).GetFields();
            foreach (FieldInfo fi in _fields)
            {
                fi.SetValue(Proxy, fi.GetValue(Actual));
            }
        }

        public virtual bool DirtyCheck()
        {
            foreach (FieldInfo fi in _fields)
            {
                if (fi.GetValue(Proxy) != fi.GetValue(Actual))
                    return true;
            }
            foreach (PropertyInfo pi in _properties)
            {
                if (pi.GetValue(Proxy) != pi.GetValue(Actual))
                    return true;
            }
            return false;
        }

        public virtual void PropagateChanges()
        {
            foreach (PropertyInfo pi in _properties)
            {
                if (pi.CanWrite)
                    pi.SetValue(Actual, pi.GetValue(Proxy));
            }
            foreach (FieldInfo fi in _fields)
            {
                fi.SetValue(Actual, fi.GetValue(Proxy));
            }
        }

        public void Dispose()
        {
            Proxy = null;
            Actual = null;
        }
    }

    /// <summary>
    /// Tracks ChangeProxy objects of type T for an IQueryable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChangeTracker<T>
        where T : class, new()
    {
        public class EditList : List<T>, IList
        {
            bool IList.IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return false;
                }
            }

            int IList.Add(object value)
            {
                T t = value as T;
                if (t != null)
                {
                    Add(t);
                    return IndexOf(t);
                }
                else
                    throw new ArgumentException("Argument \"value\" must be of type T (\"" + typeof(T).FullName + "\").", "value");
            }

            void IList.Insert(int index, object value)
            {
                T t = value as T;
                if (t != null)
                    Insert(index, t);
                else
                    throw new ArgumentException("Argument \"value\" must be of type T (\"" + typeof(T).FullName + "\").", "value");
            }

            void IList.Remove(object value)
            {
                T t = value as T;
                if (t != null)
                    Remove(t);
                else
                    throw new ArgumentException("Argument \"value\" must be of type T (\"" + typeof(T).FullName + "\").", "value");
            }
        }

        private List<ChangeProxy<T>> _changeProxies = new List<ChangeProxy<T>>();
        
        public T this[T t]
        {
            get
            {
                return _changeProxies.Single(p => p.Actual == t).Proxy;
            }
            set
            {
                _changeProxies.Single(p => p.Actual == t).Proxy = value;
            }
        }

        private EditList _proxies = new EditList();
        public EditList Proxies
        {
            get
            {
                _proxies.Clear();
                _proxies.AddRange(_changeProxies.Select(p => p.Proxy));
                return _proxies;
            }
        }

        public IQueryable<T> Query { get; protected set; }

        public ChangeTracker(IQueryable<T> iq)
        {
            Query = iq;
            _changeProxies = new List<ChangeProxy<T>>();
            _changeProxies.AddRange(iq.ToList().Select(t => new ChangeProxy<T>(t)));
        }

        public void Revert()
        {
            _changeProxies.Clear();
            _changeProxies.AddRange(Query.Select(t => new ChangeProxy<T>(t)));
        }

        public NewProxy<T> New(T t)
        {
            var p = new NewProxy<T>(t);
            _changeProxies.Add(p);
            return p;
        }

        public DeleteProxy<T> Delete(T t)
        {
            _changeProxies.RemoveAll(cp => cp.Actual == t || cp.Proxy == t);
            var p = new DeleteProxy<T>(t);
            _changeProxies.Add(p);
            return p;
        }

        public ChangeProxy<T> Update(T actual, T newProxy)
        {
            var p = _changeProxies.SingleOrNull(cp => cp.Actual == actual);
            if (p != null)
                p.Proxy = newProxy;
            else
            {
                p = new ChangeProxy<T>(actual, newProxy);
                _changeProxies.Add(p);
            }
            return p;
        }
    }

    /// <summary>
    /// Represents reflection information about a Type.
    /// </summary>
    public class CachedType
    {
        //More of these exist to add, not needed at this time - UdderlyEvelyn 11/7/16
        public PropertyInfo[] Properties { get; set; }
        public FieldInfo[] Fields { get; set; }
        public MethodInfo[] Methods { get; set; }

        /// <summary>
        /// Initialize a CachedType for the provided type, populating all caches immediately.
        /// </summary>
        /// <param name="type"></param>
        public CachedType(Type type)
        {
            Properties = type.GetProperties();
            Fields = type.GetFields();
            Methods = type.GetMethods();
        }

        public CachedType()
        {

        }
    }

    /// <summary>
    /// Represents identification information about an Assembly.
    /// </summary>
    public class CachedAssembly
    {
        private static Assembly[] _initialAssemblies { get; set; }

        /// <summary>
        /// Preload the assembly dependencies of the provided AppDomain.
        /// </summary>
        /// <param name="appDomain"></param>
        internal static void PreloadDependencies(AppDomain appDomain)
        {
            _initialAssemblies = appDomain.GetAssemblies();
            foreach (var a in _initialAssemblies)
                CachedAssembly.Get(a);
        }    

        /// <summary>
        /// Generate a string describing the list of loaded assemblies in the assembly load cache.
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyLoadCacheSummary()
        {
            return "Assemblies Cached/Loaded: " + _assemblyLoadCache.Count;
        }

        public static CachedAssembly[] AssemblyLoadCacheHierarchy
        {
            get
            {
                return new [] { _assemblyLoadCache[Assembly.GetEntryAssembly().ToString()] };
            }
        }

        /// <summary>
        /// Assembly "display name" and info about the assembly and its loading/caching state.
        /// </summary>
        private static Dictionary<string, CachedAssembly> _assemblyLoadCache = new Dictionary<string, CachedAssembly>();

        public static CachedAssembly Get(AssemblyName assemblyName)
        {
            if (!_assemblyLoadCache.ContainsKey(assemblyName.ToString()))
            { 
                var ca = new CachedAssembly(); //Add a cache entry.
                _assemblyLoadCache.Add(assemblyName.ToString(), ca);
                ca.Build(assemblyName);
                return ca;
            }
            else //It's already in the cache..
                return _assemblyLoadCache[assemblyName.ToString()]; //Return it from the cache.
        }

        public static CachedAssembly Get(Assembly assembly)
        {
            if (!_assemblyLoadCache.ContainsKey(assembly.ToString()))
            { 
                var ca = new CachedAssembly(); //Add a cache entry.
                _assemblyLoadCache.Add(assembly.ToString(), ca);
                ca.Build(assembly);
                return ca;
            }
            else //It's already in the cache..
                return _assemblyLoadCache[assembly.ToString()]; //Return it from the cache.
        }

        private string _toStringCache = null;
        public override string ToString()
        {
            if (_toStringCache == null)
                _toStringCache = AssemblyName.Name + " (" + AssemblyName.Version + ")";
            return _toStringCache;
        }
        public AssemblyName AssemblyName { get; private set; }
        public Assembly Assembly { get; set; }
        public List<CachedAssembly> Parents { get; set; }
        public List<CachedAssembly> Dependencies { get; set; }
        public AssemblyName[] References { get; set; }

        /// <summary>
        /// Build the CachedAssembly for the specified assembly, caching it and optionally loading/caching its dependencies.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="loadDependencies"></param>
        private void Build(Assembly assembly)
        {
            Parents = new List<CachedAssembly>();
            AssemblyName = assembly.GetName();
            Assembly = assembly;
            References = Assembly.GetReferencedAssemblies();
            _loadDependencies();
        }

        /// <summary>
        /// Build the CachedAssembly for the specified assembly name, loading/caching it and optionally its dependencies.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="loadDependencies"></param>
        private void Build(AssemblyName assemblyName)
        {
            Parents = new List<CachedAssembly>();
            AssemblyName = assemblyName;
            try
            {
                Assembly = Assembly.Load(AssemblyName);
            }
            catch (Exception e)
            {
                e.ThrowIfNot<
                    ArgumentNullException,
                    System.IO.FileNotFoundException,
                    System.IO.FileLoadException,
                    BadImageFormatException
                    >("Unexpected failure attempting to preload assembly \"" + this + "\".");
            }
            References = Assembly.GetReferencedAssemblies();
            _loadDependencies();
        }
        
        private CachedAssembly()
        {

        }

        private void _loadDependencies()
        {
            try
            {
                Dependencies = References.Select(an => Get(an)).ToList();
            }
            catch (Exception e)
            {
                e.ThrowIfNot<InvalidOperationException, NullReferenceException>("Unexpected exception attempting to load dependencies for \"" + ToString() + "\".");
                Dependencies = new List<CachedAssembly>(); //Blank list, either no dependencies or we can't load them.
            }
            foreach (var ai in Dependencies)
                if (!ai.Parents.Contains(this))
                    ai.Parents.Add(this);
        }
    }
}