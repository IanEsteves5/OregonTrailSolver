using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace OregonTrail.Solver
{
    public class GenerationService
    {
        private readonly DecisionEngineRandomizer _decisionEngineRandomizer;
        private readonly Random _random;

        private readonly int _inputSize;
        private readonly int _outputSize;
        private readonly int[] _innerLayers;

        private readonly int _keepBest;
        private readonly int _keepWorst;
        private readonly int _mutate;
        private readonly int _combine;
        private readonly int _create;

        private readonly int _total;

        public GenerationService(DecisionEngineRandomizer decisionEngineRandomizer, Random random,
            int inputSize, int outputSize, int[] innerLayers, int keepBest, int keepWorst,
            int create, int mutate, int combine)
        {
            Contract.Requires(keepBest >= 0);
            Contract.Requires(keepWorst >= 0);
            Contract.Requires(mutate >= 0);
            Contract.Requires(combine >= 0);

            _decisionEngineRandomizer = decisionEngineRandomizer;
            _random = random;

            _inputSize = inputSize;
            _outputSize = outputSize;
            _innerLayers = innerLayers;

            _keepBest = keepBest;
            _keepWorst = keepWorst;
            _mutate = mutate;
            _combine = combine;
            _create = create;

            _total = keepBest + keepWorst + mutate + combine + create;
        }

        public DecisionEngine[] CreateInitialEngines()
        {
            var result = new DecisionEngine[_total];

            for (var i = 0; i < _total; i++)
                result[i] = _decisionEngineRandomizer.Create(_inputSize, _outputSize, _innerLayers);

            return result;
        }

        public DecisionEngine[] Advance(DecisionEngine[] previousGeneration)
        {
            Contract.Requires(previousGeneration.Length == _total);

            var nextGeneration = new DecisionEngine[_total];

            if (_keepBest > 0)
                Array.Copy(previousGeneration, nextGeneration, _keepBest);

            if (_keepWorst > 0)
                Array.Copy(previousGeneration, previousGeneration.Length - _keepWorst, nextGeneration, _keepBest, _keepWorst);

            if (_mutate > 0)
            {
                var startIndex = _keepBest + _keepWorst;
                var endIndex = startIndex + _mutate;
                for (var i = startIndex; i < endIndex; i++)
                    nextGeneration[i] = _decisionEngineRandomizer.Mutate(nextGeneration[_random.Next(startIndex)]);
            }

            if (_combine > 0)
            {
                var startIndex = _keepBest + _keepWorst + _mutate;
                var endIndex = startIndex + _combine;
                for (var i = startIndex; i < endIndex; i++)
                {
                    var indexD1 = _random.Next(startIndex);
                    var indexD2 = _random.Next(startIndex);
                    if (indexD1 == indexD2)
                    {
                        nextGeneration[i] = nextGeneration[indexD1];
                        continue;
                    }

                    var d1 = nextGeneration[indexD1];
                    var d2 = nextGeneration[indexD2];
                    nextGeneration[i] = _decisionEngineRandomizer.Combine(d1, d2);
                }
            }

            if (_create > 0)
            {
                var startIndex = _keepBest + _keepWorst + _mutate + _combine;
                var endIndex = startIndex + _create;
                for (var i = startIndex; i < endIndex; i++)
                    nextGeneration[i] = _decisionEngineRandomizer.Create(_inputSize, _outputSize, _innerLayers);
            }

            return nextGeneration;
        }
    }
}
