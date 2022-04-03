using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.Neural
{
    public class Neuron
    {
        private static ulong _nextID = 0;
        public ulong ID = _nextID++;
        
        public Network Network;
        public double[] Inputs;
        public Func<double, double> Function { get; private set; }
        public double Output = 0;
        public MutableDouble Bias;
        public int Layer;
        public int Depth;
        private ulong _lastUpdateNumber = 0;
        public ulong LastUpdate
        {
            get
            {
                return _lastUpdateNumber;
            }
        }

        public Neuron(Network network, int layer, int depth, Func<double, double> function, int inputCount, MutableDouble bias = null)
        {
            Layer = layer;
            Depth = depth;
            Network = network;
            Function = function;
            Inputs = new double[inputCount];
            Bias = bias ?? new MutableDouble(null, Globals.StartingBiasMutability);
            network.Neurons.Set(layer, depth, this);
        }

        public void Update(ulong updateNumber)
        {
            if (updateNumber > _lastUpdateNumber)
            {
                Output = Inputs.Sum();
                _lastUpdateNumber = updateNumber;
            }
        }

        public override string ToString()
        {
            return ("{" + ID + "-" + (Function != null ? Function.Method.Name : "I/O") + "}[" + Inputs.Where(i => i != 0).Select(i => i.Round().ToString("00.00")).Join('+') + "=" + Output.Round().ToString("00.00") + ']').Replace("+-", "-");
        }
    }
}