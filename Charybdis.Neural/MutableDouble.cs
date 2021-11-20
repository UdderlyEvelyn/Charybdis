using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.Neural
{
    public class MutableDouble : IMutable<double>
    {
        public double MutabilityMutationMultiplier { get; set; }

        public double Mutability { get; set; }

        public double Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialValue">The starting double value. If null, it will be randomly assigned.</param>
        /// <param name="mutability">The starting mutability. If null, it will be set to .5 (50%).</param>
        public MutableDouble(double? initialValue = null, double? mutability = null)
        {
            Value = initialValue ?? Globals.Random.NextDouble() * 2 - 1;
            Mutability = mutability ?? .5;
            MutabilityMutationMultiplier = 1;
        }

        public MutableDouble GetCopy()
        {
            return new MutableDouble
            {
                MutabilityMutationMultiplier = MutabilityMutationMultiplier,
                Mutability = Mutability,
                Value = Value
            };
        }

        public MutableDouble GetMutatedCopy()
        {
            var md = new MutableDouble
            {
                MutabilityMutationMultiplier = MutabilityMutationMultiplier,
                Mutability = Mutability,
                Value = Value
            };
            md.Mutate();
            return md;
        }

        public void Mutate()
        {
            //Need to change this to make values closer to zero more likely.
            if (Globals.Random.Chance(Mutability))
                Value += Value * .01 * (Globals.Random.NextDouble() * 2 - 1);
            if (Globals.Random.Chance(Mutability))
                Mutability = Maths.Clamp(Mutability * ((Globals.Random.NextDouble() * 2 - 1) * MutabilityMutationMultiplier), .01, .99);
        }

        IMutable<double> IMutable<double>.GetCopy()
        {
            return GetCopy();
        }

        IMutable<double> IMutable<double>.GetMutatedCopy()
        {
            return GetMutatedCopy();
        }
    }
}
