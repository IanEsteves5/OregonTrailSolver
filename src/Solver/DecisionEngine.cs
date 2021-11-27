using OregonTrail.Shared;
using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace OregonTrail.Solver
{
    public class DecisionEngine
    {
        public int InputSize { get; }
        public int OutputSize { get; }
        public DecisionLayer[] Layers { get; }

        public DecisionEngine(params DecisionLayer[] layers)
        {
            Contract.Requires(layers.Length > 0);
            for (var i = 1; i < layers.Length; i++)
                Contract.Requires(layers[i - 1].OutputSize == layers[i].InputSize);

            InputSize = layers[0].InputSize;
            OutputSize = layers[layers.Length - 1].OutputSize;

            Contract.Requires(InputSize > 0);
            Contract.Requires(OutputSize > 0);

            Layers = layers;
        }

        public double[] Decide(double[] input)
        {
            Contract.Requires(input.Length == InputSize);

            var m = new Matrix(InputSize, 1, input.Select(Normalize).ToArray());
            foreach (var n in Layers)
                m = (n.Weights * m + n.Offsets).Select(Normalize);

            return m.Values;
        }

        private double Normalize(double d)
        {
            return 1.0 / (1.0 + Math.Exp(-1.0 * d));
        }

        public override string ToString()
        {
            return string.Join(" => ", Layers.Select(n => n.InputSize).Concat(OutputSize));
        }
    }

    public class DecisionLayer
    {
        public int InputSize { get; }
        public int OutputSize { get; }
        public Matrix Weights { get; }
        public Matrix Offsets { get; }

        public DecisionLayer(Matrix weights, Matrix offsets)
        {
            Contract.Requires(offsets.Width == 1);
            Contract.Requires(weights.Height == offsets.Height);

            InputSize = weights.Width;
            OutputSize = weights.Height;

            Contract.Requires(InputSize > 0);
            Contract.Requires(OutputSize > 0);

            Weights = weights;
            Offsets = offsets;
        }

        public override string ToString()
        {
            return InputSize + " => " + OutputSize;
        }
    }
}
