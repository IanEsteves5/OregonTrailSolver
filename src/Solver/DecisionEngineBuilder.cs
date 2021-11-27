using OregonTrail.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace OregonTrail.Solver
{
    public class DecisionEngineBuilder
    {
        public int InputSize { get; }
        public int OutputSize { get; }
        public int[] InnerLayers { get; }

        public double[] Values { get; }

        public DecisionEngineBuilder(int inputSize, int outputSize, int[] innerLayers)
        {
            Contract.Requires(inputSize > 0);
            Contract.Requires(outputSize > 0);
            foreach (var o in innerLayers)
                Contract.Requires(o > 0);

            InputSize = inputSize;
            OutputSize = outputSize;
            InnerLayers = innerLayers;

            var size = CalculateSize(inputSize, outputSize, innerLayers);
            Values = new double[size];
        }

        public DecisionEngineBuilder(DecisionEngine decisionEngine)
        {
            InputSize = decisionEngine.InputSize;
            OutputSize = decisionEngine.OutputSize;

            InnerLayers = decisionEngine.Layers.Skip(1).Select(l => l.InputSize).ToArray();

            Values = decisionEngine.Layers.SelectMany(l => l.Weights.Values.Concat(l.Offsets.Values)).ToArray();
        }

        public DecisionEngine Build()
        {
            var layers = CreateLayers().ToArray();
            return new DecisionEngine(layers);
        }

        private IEnumerable<DecisionLayer> CreateLayers()
        {
            var position = 0;

            var i = InputSize;
            foreach (var o in InnerLayers.Append(OutputSize))
            {
                var weightsSize = i * o;

                var weightsValues = new double[weightsSize];
                var offsetsValues = new double[o];

                Array.Copy(Values, position, weightsValues, 0, weightsSize);
                Array.Copy(Values, position + weightsSize, offsetsValues, 0, o);

                var weights = new Matrix(o, i, weightsValues);
                var offsets = new Matrix(i, o, offsetsValues);
                yield return new DecisionLayer(weights, offsets);

                i = o;
                position += weightsSize + o;
            }
        }

        private int CalculateSize(int inputSize, int outputSize, int[] innerLayers)
        {
            var result = 0;

            var i = inputSize;
            foreach (var o in innerLayers.Append(outputSize))
            {
                result += i * o + o;
                i = o;
            }

            return result;
        }
    }
}
