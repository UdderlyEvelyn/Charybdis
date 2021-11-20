using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Charybdis.Neural
{
    public static class Globals
    {
        private static ThreadLocal<Random> _threadLocalRandom = new ThreadLocal<Random>(() => new Random());
        public static Random Random
        {
            get
            {
                return _threadLocalRandom.Value;
            }
        }

        public static double SynapticConnectionMutationMultiplier = .0001; //More likely mutations by factor of 10, 1/25/19
        public static double MaximumNewSynapseMutationMultiplier = .01; //More likely mutations by factor of 10, 1/25/19

        public static double StartingBiasMutability = .03;
        public static double StartingWeightMutability = .1;

        public static bool ParallelNetworkUpdateMethod = false;
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             