using OregonTrail.Game;
using OregonTrail.Solver;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace OregonTrail
{
    public static class Program
    {
        static void Main()
        {
            var random = new Random();
            
            var decisionEngineRandomizer = new DecisionEngineRandomizer(random,
                initialMin: -10.0,
                initialMax: 10.0,
                mutationChance: 0.5,
                mutationMax: 2.0);

            var generationService = new GenerationService(decisionEngineRandomizer, random,
                inputSize: GameChoiceMaker.DecisionInputSize,
                outputSize: GameChoiceMaker.DecisionOutputSize,
                innerLayers: new int[] { 80, 40, 20 },
                keepBest: 10,
                keepWorst: 2,
                create: 5,
                mutate: 30,
                combine: 30);

            var messageInterpreter = new MessageInterpreter();
            var messageIndexer = new MessageIndexer();
            var game = new GameLogic(random);
            var gameEvaluator = new GameEvaluator(random, game,
                maxRounds: 50,
                roundsInfiniteLoop: 3,
                keepMessagesHistory: 3);
            
            var engines = generationService.CreateInitialEngines();
            int generation = 1;
            double winRate = 0.0;

            var replaysFile = string.Format("replays_{0}.txt", DateTime.Now.ToString("yyyyMMddhhmmss"));
            
            var sw = new Stopwatch();

            for (;;)
            {
                sw.Restart();

                var numberOfEvaluations = Convert.ToInt32(10.0 + winRate * 90.0);

                var evaluatedEngines = engines
                    .AsParallel()
                    .Select(e => EvaluateDecisionEngine(e, messageInterpreter, messageIndexer, gameEvaluator, numberOfEvaluations))
                    .OrderByDescending(r => r.AvgScore)
                    .ToArray();

                sw.Stop();
                
                var decisionEngineResult = evaluatedEngines[0];
                var bestGameResult = decisionEngineResult.BestResult;

                winRate = decisionEngineResult.GetWinRate();

                var resultMsg = string.Format("Gen={0}; T={1}s; WinRate={2}%; AvgScore={3}; MaxScore={4}",
                    generation,
                    sw.Elapsed.TotalSeconds.ToString("N2", CultureInfo.InvariantCulture),
                    (winRate * 100.0).ToString("N2", CultureInfo.InvariantCulture),
                    decisionEngineResult.AvgScore.ToString("N2", CultureInfo.InvariantCulture),
                    bestGameResult.Score);

                Console.WriteLine(resultMsg);

                using (var writer = new StreamWriter(replaysFile, append: true))
                {
                    writer.WriteLine("----------");
                    writer.WriteLine(resultMsg);
                    writer.WriteLine("----------");
                    writer.WriteLine(bestGameResult.Replay);
                    writer.WriteLine(string.Format("Result={0}", bestGameResult.ResultType.ToString()));
                }

                engines = generationService.Advance(evaluatedEngines.Select(r => r.ChoiceMaker.DecisionEngine).ToArray());
                generation++;
            }
        }

        private static GameChoiceMakerEvaluationResult EvaluateDecisionEngine(DecisionEngine decisionEngine,
            MessageInterpreter messageInterpreter, MessageIndexer messageIndexer, GameEvaluator gameEvaluator, int nummberOfEvaluations)
        {
            var choiceMaker = new GameChoiceMaker(decisionEngine, messageInterpreter, messageIndexer);
            return gameEvaluator.Evaluate(choiceMaker, nummberOfEvaluations);
        }
    }
}
