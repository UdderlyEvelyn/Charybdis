using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.Neural
{
    public class Synapse : IMutable
    {
        public Network Network;
        public int Layer;
        public int Index;
        public int Depth;
        public Neuron Axon;
        public Neuron Dendrite;
        public MutableDouble Weight;
        private ulong _lastUpdateNumber = 0;

        public double Mutability
        {
            get
            {
                return ((IMutable)Weight).Mutability;
            }

            set
            {
                ((IMutable)Weight).Mutability = value;
            }
        }

        //Axon -> Dendrite
        public Synapse(Network network, int layer, int index, int depth, MutableDouble weight = null)
        {
            Network = network;
            Layer = layer;
            Index = index;
            Depth = depth;
            Dendrite = network.Neurons.Get(layer, depth);
            Axon = network.Neurons.Get(layer - 1, index);
            Weight = weight ?? new MutableDouble(Globals.StartingWeightMutability/*Globals.Random.NextDouble() * 2 - 1*/);
            network.SynapseBucket.Add(this);
            network.Synapses.Get(layer, depth).Add(this);
        }

        public void Fire(ulong updateNumber)
        {
            if (updateNumber > _lastUpdateNumber)
            {
                if (Dendrite != null && Axon != null)
                    Dendrite.Inputs[Index] = (Axon.Function != null ? Axon.Function(Axon.Output + Axon.Bias.Value) : (Axon.Output + Axon.Bias.Value)) * Weight.Value;
                _lastUpdateNumber = updateNumber;
            }
        }

        public override string ToString()
        {
            return "<" + (Axon != null ? Axon.ID.ToString() : "?") + "@" + (Weight.Value.Round(places: 0) * 100) + "%->" + (Dendrite != null ? Dendrite.ID.ToString() : "?") + ">";
        }

        public void Mutate()
        {
            ((IMutable)Weight).Mutate();
        }
    }
}
