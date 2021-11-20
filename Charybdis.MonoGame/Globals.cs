using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Library.Core;
using Charybdis.MonoGame;
using System.Collections.Concurrent;
using System.Threading;

namespace Charybdis.MonoGame
{
    public class Globals
    {
        public static ReaderWriterLockSlim AllGameObjectsLock = new ReaderWriterLockSlim();// LockRecursionPolicy.SupportsRecursion);
        public static List<GameObject2> AllGameObjects = new List<GameObject2>();
        public static ReaderWriterLockSlim VisualsLock = new ReaderWriterLockSlim();
        public static List<Drawable2> Visuals = new List<Drawable2>();
        private static ThreadLocal<Random> _threadLocalRandom = new ThreadLocal<Random>(() => new Random());
        public static Random Random
        {
            get
            {
                return _threadLocalRandom.Value;
            }
        }
        public static GraphicsDevice GraphicsDevice;
        public static Font Font;
        public static Col3 GridColor = Col3.Black;
        
        public static int Width = 1920;
        public static int Height = 1080;
        
        public static int SimulationFramesPerSecond = 0;

        //public static int CleanupCalls = 0;
        //public static int CleanupCallThreshold = 1000; //5000; //Not based on anything! :D
        //public static int MegabytesUsedCleanUpThreshold = 3072; //3GB -> Clean
        public static int MemoryUsed = 0;

        public static bool DecoupleSimulationFromVisuals = true;
        public static bool ParallelUpdateLoop = true;
    }
}
