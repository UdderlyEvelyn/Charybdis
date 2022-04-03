using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Charybdis.Library.Core.Classes;

namespace Charybdis.Neural
{
    public class Network
    {
        //public class SimplifiedRepresentation
        //{
        //    public double SynapticConnectionChance;
        //    public Func<double, double> ActivationFunction;
        //    public int Width;
        //    public int Height;
        //    public int Inputs;
        //    public int Outputs;
        //    private List<SimplifiedNeuron> neuronList = new List<SimplifiedNeuron>();
        //    private List<SimplifiedSynapse> synapseList = new List<SimplifiedSynapse>();

        //    public SimplifiedRepresentation(Network n)
        //    {
        //        Width = n.Width - 1;
        //        Height = n.Height - 1;
        //        Inputs = n.InputCount;
        //        Outputs = n.OutputCount;
        //        SynapticConnectionChance = n.SynapticConnectionChance;
        //        ActivationFunction = n.ActivationFunction;
        //        var output = "Creating Simplified Representation Of Network\n";
        //        for (int x = 0; x < n.Width; x++)
        //        {
        //            for (int y = 0; y < n.Height; y++)
        //            {
        //                output += "Layer " + x + ", Depth " + y + ": ";
        //                var neuron = n.Neurons.Get(x, y);
        //                var synapses = n.Synapses.Get(x, y);
        //                if (neuron != null)
        //                {
        //                    neuronList.Add(new SimplifiedNeuron(neuron, x, y, synapses != null ? synapses.Count : 0));
        //                    output += "Neuron Found";
        //                }
        //                else
        //                    output += "No Neuron";
        //                if (synapses != null)
        //                {
        //                    foreach (var s in synapses)
        //                        synapseList.Add(new SimplifiedSynapse(s));
        //                    output += synapses.Count + " Synapses\n";
        //                }
        //                else
        //                    output += "No Synapses\n";
        //            }
        //        }
        //        Console.WriteLine(output);
        //    }

        //    public Network ToNetwork()
        //    {
        //        Network n = new Network(Width + 1, Height + 1, Inputs, Outputs, SynapticConnectionChance, ActivationFunction);
        //        Dictionary<ulong, ulong> neuronIDTranslationTable = new Dictionary<ulong, ulong>();
        //        foreach (var sn in neuronList)
        //        {
        //            var neuron = sn.ToNeuron(n, ActivationFunction);
        //            n.Neurons.Set(sn.layer, sn.depth, neuron);
        //            neuronIDTranslationTable.Add(sn.id, neuron.ID);
        //        }
        //        for (int x = 0; x < Width; x++)
        //        {
        //            var layerSynapses = synapseList.Where(ss => ss.layer == x);
        //            for (int y = 0; y < Height; y++)
        //            {
        //                var depthSynapses = layerSynapses.Where(ss => ss.depth == y);
        //                var synapses = new List<Synapse>();
        //                foreach (var ss in depthSynapses.OrderBy(ds => ds.index))
        //                {
        //                    var synapse = ss.ToSynapse(n);
        //                    n.SynapseBucket.Add(synapse);
        //                    synapses.Add(synapse);
        //                }
        //                n.Synapses.Set(x, y, synapses);
        //            }
        //        }
        //        return n;
        //    }

        //    private class SimplifiedNeuron
        //    {
        //        public ulong id;
        //        public int layer;
        //        public int depth;
        //        public double bias;
        //        public double biasMutability;
        //        public int inputCount;

        //        public SimplifiedNeuron(Neuron neuron, int x, int y, int inputs)
        //        {
        //            id = neuron.ID;
        //            bias = neuron.Bias.Value;
        //            biasMutability = neuron.Bias.Mutability;
        //            layer = x;
        //            depth = y;
        //            inputCount = inputs;
        //        }
                
        //        public Neuron ToNeuron(Network network, Func<double, double> activationFunction)
        //        {
        //            return new Neuron(network, layer, depth, activationFunction, inputCount, new MutableDouble(bias, biasMutability));
        //        }
        //    }

        //    private class SimplifiedSynapse
        //    {
        //        public ulong axonID;
        //        public ulong dendriteID;
        //        public int layer;
        //        public int depth;
        //        public int index;
        //        public double weight;
        //        public double weightMutability;
        //        public double mutability;

        //        public SimplifiedSynapse(Synapse synapse)
        //        {
        //            axonID = synapse.Axon.ID;
        //            dendriteID = synapse.Dendrite.ID;
        //            layer = synapse.Layer;
        //            depth = synapse.Depth;
        //            index = synapse.Index;
        //            weight = synapse.Weight.Value;
        //            weightMutability = synapse.Weight.Mutability;
        //            mutability = synapse.Mutability;
        //        }

        //        //Doesn't discard positional information but should probably put synapse in proper place (non-bucket) while using this.
        //        public Synapse ToSynapse(Network network)
        //        {
        //            return new Synapse(network, layer, index, depth, new MutableDouble(weight, weightMutability));
        //        }
        //    }
        //}

        private static ulong _nextNetworkID = 0;
        public ulong ID = ++_nextNetworkID;

        public Array2<Neuron> Neurons;
        public ClassArray2<List<Synapse>> Synapses;
        public List<Synapse> SynapseBucket = new List<Synapse>();
        public int Width;
        public int Height;
        public int InputCount;
        public int OutputCount;
        public Func<double, double> ActivationFunction;
        public double SynapticConnectionChance;

        public void Mutate(double chance)
        {
            if (chance < 0)
                chance = Math.Abs(chance);
            if (chance > 1)
                chance /= 100;
            int maximumNewSynapsesPerLayer = (int)((Globals.MaximumNewSynapseMutationMultiplier * Height).Round(places: 0));
            int maximumNewSynapsesForOutputLayer = (int)((Globals.MaximumNewSynapseMutationMultiplier * OutputCount).Round(places: 0));
            double synapticConnectionMutationChance = SynapticConnectionChance * Globals.SynapticConnectionMutationMultiplier;
            List<Synapse> synapsesToRemove = new List<Synapse>();
            for (int x = 1; x < Width - 1; x++) //Skip inputs by starting at 1, skip outputs by ending at 1 before the width.
            {
                for (int y = 0; y < Height; y++)
                {
                var depth = Synapses.Get(x, y);
                    foreach (var synapse in depth)
                        if (Globals.Random.Chance(synapticConnectionMutationChance)) //Chance to delete a synapse..
                            synapsesToRemove.Add(synapse);
                        else if (Globals.Random.Chance(chance)) //We're not deleting it, so it has a chance to mutate instead.
                        {
                            if (Globals.Random.Chance(.5))
                                synapse.Dendrite.Bias.Mutate();
                            else
                                synapse.Weight.Mutate();
                        }
                }
                for (int n = 0; n < (x == Width - 1 ? maximumNewSynapsesForOutputLayer : maximumNewSynapsesPerLayer); n++)
                    if (Globals.Random.Chance(synapticConnectionMutationChance) && Synapses.GetColumn(x).Count < (Neurons.GetColumn(x - 1).Count * Neurons.GetColumn(x).Count))
                    {
                        int yi = Globals.Random.Next(0, (x == Width - 1 ? OutputCount : Height));
                        int i = Globals.Random.Next(0, Height);
                        bool uniqueSynapseCombination = SynapseBucket.Any(s => s.Index == i && s.Depth == yi);
                        while (!uniqueSynapseCombination)
                        {
                            yi = Globals.Random.Next(0, (x == Width - 1 ? OutputCount : Height));
                            i = Globals.Random.Next(0, Height);
                            uniqueSynapseCombination = SynapseBucket.Any(s => s.Index == i && s.Depth == yi);
                        }
                        var newSynapse = new Synapse(this, x, i, yi);
                        Synapses.Get(x, yi).Add(newSynapse);
                        SynapseBucket.Add(newSynapse);
                    }
            }
        }

        public Synapse GetSynapse(int layer, int depth, int index)
        {
            Synapse s = null;
            var depthIndices = Synapses.Get(layer, depth);
            if (depthIndices == null)
                return null;
            try
            {
                s = depthIndices[index];
            }
            catch (IndexOutOfRangeException)
            {
                /*This is fine, we will be returning null.*/
            }
            catch (ArgumentOutOfRangeException)
            {
                /*This is fine, we will be returning null.*/
            }
            return s;
        }

        /// <summary>
        /// Creates a clone of this network.
        /// </summary>
        public Network Clone()
        {
            //return new SimplifiedRepresentation(this).ToNetwork();
            return new Network(this);
        }

        /// <summary>
        /// Creates a clone of the parent network.
        /// </summary>
        /// <param name="parent"></param>
        public Network(Network parent)
        {
            //string networkCreationLog = "\n";
            //DateTime start = DateTime.Now;
            //DateTime startInit = DateTime.Now;
            Neurons = new Array2<Neuron>(parent.Width, parent.Height);
            Synapses = new ClassArray2<List<Synapse>>(parent.Width, parent.Height);
            Width = parent.Width;
            Height = parent.Height;
            InputCount = parent.InputCount;
            OutputCount = parent.OutputCount;
            //networkCreationLog += "Init [" + (DateTime.Now - startInit).TotalMilliseconds + "ms]\n";
            for (int x = 0; x < Width; x++)
            {
                //DateTime startLayer = DateTime.Now;
                if (x == 0) //Input layer..
                    for (int y = 0; y < InputCount; y++)
                    {
                        //DateTime startDepth = DateTime.Now;
                        Neurons.Set(x, 0, new Neuron(this, x, y, null, 1));
                        //networkCreationLog += "Depth " + y + " [" + (DateTime.Now - startDepth).TotalMilliseconds + "ms]\n";
                    }
                else if (x == Width - 1) //Output layer..
                    for (int y = 0; y < OutputCount; y++)
                    {
                        List<Synapse> depthIndices = new List<Synapse>();
                        Synapses.Set(x, y, depthIndices);
                        //DateTime startDepth = DateTime.Now;
                        var parentNeuron = parent.Neurons.Get(x, y);
                        var thisNeuron = new Neuron(this, x, y, null, Height, parentNeuron.Bias.GetCopy());
                        Neurons.Set(x, y, thisNeuron);
                        for (int i = 0; i < Height; i++)
                        {

                            //DateTime startIndex = DateTime.Now;
                            //DateTime startGetSynapse = DateTime.Now;
                            var parentSynapse = parent.GetSynapse(x, y, i);
                            //networkCreationLog += "Get Synapse [" + (DateTime.Now - startGetSynapse).TotalMilliseconds + "ms]\n";
                            if (parentSynapse != null)
                            {
                                //DateTime startNewSynapse = DateTime.Now;
                                var newSynapse = new Synapse(this, x, i, y, parentSynapse.Weight.GetCopy());
                                depthIndices.Add(newSynapse);
                                SynapseBucket.Add(newSynapse);
                                //networkCreationLog += "New Synapse [" + (DateTime.Now - startNewSynapse).TotalMilliseconds + "ms]\n";
                            }
                            //else networkCreationLog += "Synapse Was Null\n";
                            //networkCreationLog += "Index " + i + " [" + (DateTime.Now - startIndex).TotalMilliseconds + "ms]\n";
                        }
                        //networkCreationLog += "Depth " + y + " [" + (DateTime.Now - startDepth).TotalMilliseconds + "ms]\n";
                    }
                else //Hidden layer(s)..
                    for (int y = 0; y < Height; y++)
                    {
                        List<Synapse> depthIndices = new List<Synapse>();
                        Synapses.Set(x, y, depthIndices);
                        //DateTime startDepth = DateTime.Now;
                        var parentNeuron = parent.Neurons.Get(x, y);
                        var thisNeuron = new Neuron(this, x, y, parent.ActivationFunction, Height, parentNeuron.Bias.GetCopy());
                        Neurons.Set(x, y, thisNeuron);
                        for (int i = 0; i < Height; i++)
                        {
                            //DateTime startIndex = DateTime.Now;
                            //DateTime startGetSynapse = DateTime.Now;
                            var parentSynapse = parent.GetSynapse(x, y, i);
                            //networkCreationLog += "Get Synapse [" + (DateTime.Now - startGetSynapse).TotalMilliseconds + "ms]\n";
                            if (parentSynapse != null)
                            {
                                //DateTime startNewSynapse = DateTime.Now;
                                var newSynapse = new Synapse(this, x, i, y, parentSynapse.Weight.GetCopy());
                                depthIndices.Add(newSynapse);
                                SynapseBucket.Add(newSynapse);
                                //networkCreationLog += "New Synapse [" + (DateTime.Now - startNewSynapse).TotalMilliseconds + "ms]\n";
                            }
                            //else networkCreationLog += "Synapse Was Null\n";
                            //networkCreationLog += "Index " + i + " [" + (DateTime.Now - startIndex).TotalMilliseconds + "ms]\n";
                        }
                        //networkCreationLog += "Depth " + y + " [" + (DateTime.Now - startDepth).TotalMilliseconds + "ms]\n";
                    }
                //networkCreationLog += "Layer " + x + " [" + (DateTime.Now - startLayer).TotalMilliseconds + "ms]\n";
            }
            //System.Diagnostics.Debug.WriteLine("Network Created From Parent [" + (DateTime.Now - start).TotalMilliseconds + "ms]" + networkCreationLog + "END");
        }

        public Network(int width, int height, int inputs, int outputs, double synapticConnectionChance, Func<double, double> activationFunction)
        {
            SynapticConnectionChance = synapticConnectionChance;
            ActivationFunction = activationFunction;
            Neurons = new Array2<Neuron>(width, height);
            Synapses = new ClassArray2<List<Synapse>>(width, height);
            Width = width;
            Height = height;
            InputCount = inputs;
            OutputCount = outputs;
            for (int x = 0; x < width; x++)
            {
                List<List<Synapse>> layerDepths = new List<List<Synapse>>();
                if (x == 0) //Input layer..
                    for (int y = 0; y < InputCount; y++)
                        Neurons.Set(x, y, new Neuron(this, x, y, null, 1));
                else if (x == width - 1) //Output layer..
                    for (int y = 0; y < OutputCount; y++)
                    {
                        List<Synapse> depthIndices = new List<Synapse>();
                        Synapses.Set(x, y, depthIndices);
                        Neurons.Set(x, y, new Neuron(this, x, y, null, height));
                        for (int i = 0; i < height; i++)
                            if (synapticConnectionChance < 1 ? Globals.Random.Chance(synapticConnectionChance) : true)
                            {
                                Synapse s = new Synapse(this, x, i, y);
                                depthIndices.Add(s);
                                SynapseBucket.Add(s);
                            }
                    }
                else //Hidden layer(s)..
                    for (int y = 0; y < height; y++)
                    {
                        List<Synapse> depthIndices = new List<Synapse>();
                        Synapses.Set(x, y, depthIndices);
                        Neurons.Set(x, y, new Neuron(this, x, y, activationFunction, height));
                        for (int i = 0; i < (x == 1 ? InputCount : height); i++)
                            if (synapticConnectionChance < 1 ? Globals.Random.Chance(synapticConnectionChance) : true)
                            {
                                Synapse s = new Synapse(this, x, i, y);
                                depthIndices.Add(s);
                                SynapseBucket.Add(s);
                            }
                    }
            }
        }

        public void Update(ulong updateNumber)
        {
            if (Globals.ParallelNetworkUpdateMethod)
            {
                var inputsResult = Parallel.ForEach(Neurons.GetColumn(0), neuron =>
                {
                    if (neuron != null)
                        neuron.Update(updateNumber);
                });
                while (!inputsResult.IsCompleted) { /*Wait..*/ }
                var outerResult = Parallel.For(0, Synapses.Height, x =>
                {
                    var layer = Synapses.GetColumn(x);
                    var neuronsToUpdate = new List<Neuron>();
                    foreach (var depth in layer)
                    {
                        if (depth != null)
                            foreach (var synapse in depth)
                            {
                                synapse.Fire(updateNumber);
                                if (synapse.Dendrite != null)
                                    neuronsToUpdate.Add(synapse.Dendrite);
                            }
                    }
                    var innerResult = Parallel.ForEach(neuronsToUpdate, neuron =>
                    {
                        if (neuron != null)
                            neuron.Update(updateNumber);
                    });
                    while (!innerResult.IsCompleted) { /*Wait..*/ }
                });
                while (!outerResult.IsCompleted) { /*Wait..*/ }
            }
            else
                foreach (Synapse s in SynapseBucket)
                {
                    s.Axon.Update(updateNumber);
                    s.Fire(updateNumber);
                    s.Dendrite.Update(updateNumber);
                }
        }

        public void SetInput(int index, double value)
        {
            if (Neurons != null)
            {
                var n = Neurons.Get(0, index);
                if (n != null)
                    n.Inputs[0] = value;
                //Else neuron isn't set (yet?), so do nothing.
            }
            else
                throw new Exception("The neural network's neuronal array has not been assigned yet.");
        }

        public List<List<double>> GetHidden()
        {
            var result = new List<List<double>>();
            for (int x = 1; x < Width - 1; x++) //x=1 to skip input row, x<Width-1 to skip output row
            {
                var column = new List<double>();
                for (int y = 0; y < Height; y++)
                {
                    var n = Neurons.Get(x, y);
                    column.AddRange(n.Inputs);
                    column.Add(n.Output);
                }
                result.Add(column);
            }
            return result;
        }

        public string GetLayerString(int x)
        {
            return Synapses.Array.Where(s => s != null).Select(s => s.ToString()).Join('\n');
            //return Synapses[x].Select(s => "[" + (s.Axon != null ? s.Axon.ID.ToString() : "?") + "->" + (s.Dendrite != null ? s.Dendrite.ID.ToString() : "?") + "]").Join('\n');
            //return Synapses[x].Select(s => s.Axon != null ? (s.Axon.ToString() + "@" + s.Weight) : "").Join('\n');
            //return Neurons.GetColumn(x).Select(n => n.ToString() + "\n" + Synapses[x].Where(s => s.Axon == n).Select(s => "@" + s.Weight + "->" + (s.Dendrite != null ? s.Dendrite.ID.ToString() : "OUT")).Join('\n')).Join('\n');
        }

        public string GetOutputPathString(int o)
        {
            Grid<string> stringGrid = new Grid<string>();
            for (int x = 0; x < Width; x++)
            {
                stringGrid.AddColumn();
                if (x == 0)
                    for (int y = 0; y < InputCount; y++)
                        stringGrid.AddRowToColumn(0, GetInput(y).Round().ToString());
                else if (x == Width - 1)
                    for (int y = 0; y < InputCount; y++)
                        stringGrid.AddRowToColumn(Width - 1, GetOutput(y).Round().ToString());
            }
            var n = Neurons.Get(Width - 1, o);
            if (n != null)
            {
                string result = n.Output.ToString();
                for (int x = 1; x < Width; x++)
                {
                    stringGrid.AddRowToColumn(x, GetContributorStringsForNeuron(n));
                }
                return stringGrid.ToString();
            }
            else return "NULL";
        }

        private string GetContributorStringsForNeuron(Neuron n)
        {
            List<string> contributorStrings = new List<string>();
            var addr = Neurons.GetPosition(n);
            var x = addr[0];
            var synapses = SynapseBucket.Where(s => s.Layer == x && s.Dendrite == n);
            var values = synapses.Select(s => s.Axon.Function(s.Axon.Output));
            var weights = synapses.Select(s => s.Weight);
            var biases = synapses.Select(s => s.Axon.Bias);
            foreach (var s in synapses)
            {
                var v = s.Axon.Function != null ? s.Axon.Function(s.Axon.Output) : s.Axon.Output;
                var w = s.Weight.Value;
                var b = s.Axon.Bias.Value;
                if ((v != 0 && w != 0) || b != 0)
                    contributorStrings.Add("(" + v.Round() + "*" + w.Round() + "+" + b.Round() + "=" + (v * w + b).Round() + ")");
            }
            return contributorStrings.Join("+").Replace("+-", "-");
        }

        public List<double> GetOutputs()
        {
            var result = new List<double>();
            for (int y = 0; y < Height; y++)
                result.Add(GetOutput(y));
            return result;
        }

        public double GetOutput(int y)
        {
            var n = Neurons.Get(Width - 1, y);
            if (n != null)
            {
                if (double.IsNaN(n.Output))
                    return 0; //No output? Return zero output.
                    //throw new Exception("Output \"" + y + "\" for neuron #" + n.ID + " of network #" + ID + " was NaN. Last update on this neuron was \"" + n.LastUpdate + "\".");
                else
                    return n.Output;
            }
            else //Neuron doesn't exist..
                return 0; //Returns zero output.
        }

        public List<double> GetInputs()
        {
            var result = new List<double>();
            for (int y = 0; y < Height; y++)
                result.Add(GetInput(y));
            return result;
        }

        public double GetInput(int y)
        {
            var n = Neurons.Get(0, y);
            if (n != null)
                return n.Inputs[0];
            else //Neuron doesn't exist..
                return 0; //Return zero input.
        }

        public override string ToString()
        {
            string header = Width + "x" + Height + " Neural Network\n";
            List<string> lines = new List<string>();
            for (int x = 0; x < Width; x++)
            {
                lines.Add("");
                lines[x] += GetLayerString(x);
            }
            lines.Insert(0, header);
            return lines.Join("\n\n");
        }
    }
}
