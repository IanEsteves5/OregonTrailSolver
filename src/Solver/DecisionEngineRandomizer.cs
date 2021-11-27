using OregonTrail.Shared;
using System;
using System.Diagnostics.Contracts;

namespace OregonTrail.Solver
{
    public class DecisionEngineRandomizer
    {
        private readonly Random _random;

        private readonly double _initialRange;
        private readonly double _initialOffset;
        private readonly double _mutationChance;
        private readonly double _mutationRange;
        private readonly double _mutationOffset;

        public DecisionEngineRandomizer(Random random, double initialMin, double initialMax, double mutationChance, double mutationMax)
        {
            _random = random;

            _initialRange = initialMax - initialMin;
            _initialOffset = initialMin;
            _mutationChance = mutationChance;
            _mutationRange = mutationMax * 2.0;
            _mutationOffset = mutationMax;
        }

        public DecisionEngine Create(int inputSize, int outputSize, int[] innerLayers)
        {
            var builder = new DecisionEngineBuilder(inputSize, outputSize, innerLayers);

            for (var i = 0; i < builder.Values.Length; i++)
                builder.Values[i] = _random.NextDouble() * _initialRange + _initialOffset;

            return builder.Build();
        }

        public DecisionEngine Combine(DecisionEngine d1, DecisionEngine d2)
        {
            Contract.Requires(d1.InputSize == d2.InputSize);
            Contract.Requires(d1.OutputSize == d2.OutputSize);
            Contract.Requires(d1.Layers.Length == d2.Layers.Length);

            var b1 = new DecisionEngineBuilder(d1);
            var b2 = new DecisionEngineBuilder(d2);

            Contract.Requires(b1.Values.Length == b2.Values.Length);
            for (var i = 0; i < b1.InnerLayers.Length; i++)
                Contract.Requires(b1.InnerLayers[i] == b2.InnerLayers[i]);

            for (var i = 0; i < b1.Values.Length; i++)
                if (_random.NextBool())
                    b1.Values[i] = b2.Values[i];

            return b1.Build();
        }

        public DecisionEngine Mutate(DecisionEngine d)
        {
            var b = new DecisionEngineBuilder(d);

            for (var i = 0; i < b.Values.Length; i++)
                if (_random.NextDouble() < _mutationChance)
                    b.Values[i] += _random.NextDouble() * _mutationRange - _mutationOffset;

            return b.Build();
        }
    }
}
