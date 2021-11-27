using OregonTrail.Game;
using OregonTrail.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OregonTrail.Solver
{
    public enum EGameEvaluationResultType
    {
        Win,
        Lose,
        MaxRoundsExceeded,
        InfiniteLoop
    }

    public class GameEvaluator
    {
        private readonly Random _random;
        private readonly IGame _game;

        private readonly int _maxRounds;
        private readonly int _roundsInfiniteLoop;
        private readonly int _keepMessagesHistory;

        public GameEvaluator(Random random, IGame game, int maxRounds, int roundsInfiniteLoop, int keepMessagesHistory)
        {
            _random = random;
            _game = game;

            _maxRounds = maxRounds;
            _roundsInfiniteLoop = roundsInfiniteLoop;
            _keepMessagesHistory = keepMessagesHistory;
        }

        public GameEvaluationResult Evaluate(GameChoiceMaker choiceMaker)
        {
            var adapter = new MessageChannelAdapter(choiceMaker, _maxRounds, _roundsInfiniteLoop, _keepMessagesHistory);
            var replayRecorder = new MessageChannelReplayRecorder(adapter);

            try
            {
                var win = _game.Play(replayRecorder);

                if (win)
                    return new GameEvaluationResult(EGameEvaluationResultType.Win, adapter.Rounds,
                        CalculateWinScore(adapter), replayRecorder.GetReplay());

                return new GameEvaluationResult(EGameEvaluationResultType.Lose, adapter.Rounds,
                    CalculateLoseScore(adapter), replayRecorder.GetReplay());
            }
            catch (MaxRoundsExceededException)
            {
                return new GameEvaluationResult(EGameEvaluationResultType.MaxRoundsExceeded, _maxRounds,
                    CalculateMaxRoundsExceededScore(adapter), replayRecorder.GetReplay());
            }
            catch (InfiniteLoopRoundsException)
            {
                return new GameEvaluationResult(EGameEvaluationResultType.InfiniteLoop, _maxRounds, 
                    CalculateInfiniteLoopScore(adapter), replayRecorder.GetReplay());
            }
        }

        public GameChoiceMakerEvaluationResult Evaluate(GameChoiceMaker choiceMaker, int numberOfEvaluations)
        {
            var results = Enumerable
                .Range(0, numberOfEvaluations)
                .Select(i => Evaluate(choiceMaker))
                .ToArray();

            return new GameChoiceMakerEvaluationResult(choiceMaker, results);
        }

        private int CalculateWinScore(MessageChannelAdapter a)
        {
            return _maxRounds * 4 + 20 - a.DistinctMessages;
        }

        private int CalculateLoseScore(MessageChannelAdapter a)
        {
            return _maxRounds * 2 + 10 + a.DistinctMessages;
        }

        private int CalculateMaxRoundsExceededScore(MessageChannelAdapter a)
        {
            return _maxRounds + 5 + a.DistinctMessages;
        }

        private int CalculateInfiniteLoopScore(MessageChannelAdapter a)
        {
            return a.DistinctMessages;
        }

        private class MessageChannelAdapter : IMessageChannel
        {
            public int Rounds;
            public int ChangingMessages;
            public int DistinctMessages => _distinctMessages.Count;

            private readonly IMessageChannel _choiceMaker;
            private readonly int _maxRounds;
            private readonly int _roundsInfiniteLoop;
            private readonly int _keepMessagesHistory;

            private int _infiniteLoopCounter;
            private StringBuilder _sb;
            private readonly List<string> _messageHistory;
            private readonly HashSet<string> _distinctMessages;

            public MessageChannelAdapter(IMessageChannel choiceMaker, int maxRounds, int roundsInfiniteLoop, int keepMessagesHistory)
            {
                _choiceMaker = choiceMaker;
                _maxRounds = maxRounds;
                _roundsInfiniteLoop = roundsInfiniteLoop;
                _keepMessagesHistory = keepMessagesHistory;

                _sb = new StringBuilder();
                _messageHistory = new List<string>();
                _distinctMessages = new HashSet<string>();
            }

            public void Write(string msg)
            {
                _choiceMaker.Write(msg);

                _sb.Append(msg);
            }

            public void WriteLine(string msg)
            {
                _choiceMaker.WriteLine(msg);

                _sb.Append(msg);
                _sb.Append("\r\n");
            }

            public string Read()
            {
                Rounds++;
                if (Rounds > _maxRounds)
                    throw new MaxRoundsExceededException();

                var msg = _sb.ToString().Trim();
                if (_messageHistory.Contains(msg))
                {
                    _infiniteLoopCounter++;
                    if (_infiniteLoopCounter > _roundsInfiniteLoop)
                        throw new InfiniteLoopRoundsException();

                    _messageHistory.Remove(msg);
                    _messageHistory.Add(msg);
                }
                else
                {
                    ChangingMessages++;
                    _infiniteLoopCounter = 0;
                    
                    _messageHistory.Add(msg);
                    if (_messageHistory.Count > _keepMessagesHistory)
                        _messageHistory.RemoveAt(0);

                    _distinctMessages.Add(msg);
                }

                _sb = new StringBuilder();

                return _choiceMaker.Read();
            }
        }

        private class MessageChannelReplayRecorder : IMessageChannel
        {
            private readonly IMessageChannel _innerChannel;
            private readonly StringBuilder _sb;

            public MessageChannelReplayRecorder(IMessageChannel innerChannel)
            {
                _innerChannel = innerChannel;
                _sb = new StringBuilder();
            }

            public void Write(string msg)
            {
                _sb.Append(msg);
                _innerChannel.Write(msg);
            }

            public void WriteLine(string msg)
            {
                _sb.Append(msg);
                _sb.Append("\r\n");
                _innerChannel.WriteLine(msg);
            }

            public string Read()
            {
                var msg = _innerChannel.Read();
                _sb.Append(msg);
                _sb.Append("\r\n");
                return msg;
            }

            public Lazy<string> GetReplay()
            {
                return new Lazy<string>(_sb.ToString);
            }
        }

        private class MaxRoundsExceededException : Exception
        {
        }

        private class InfiniteLoopRoundsException : Exception
        {
        }
    }

    public class GameEvaluationResult
    {
        public EGameEvaluationResultType ResultType { get; }
        public int Rounds { get; }
        public int Score { get; }

        private readonly Lazy<string> _replay;
        public string Replay => _replay.Value;

        public GameEvaluationResult(EGameEvaluationResultType resultType, int rounds, int score, Lazy<string> replay)
        {
            ResultType = resultType;
            Rounds = rounds;
            Score = score;
            _replay = replay;
        }
    }

    public class GameChoiceMakerEvaluationResult
    {
        public GameChoiceMaker ChoiceMaker { get; }

        public Dictionary<EGameEvaluationResultType, int> ResultTypeTimes { get; }
        public double AvgRounds { get; }
        public double AvgScore { get; }

        public GameEvaluationResult BestResult { get; }

        public GameChoiceMakerEvaluationResult(GameChoiceMaker choiceMaker, GameEvaluationResult[] results)
        {
            ChoiceMaker = choiceMaker;

            ResultTypeTimes = results.GroupBy(r => r.ResultType).ToDictionary(g => g.Key, g => g.Count());
            AvgRounds = results.Average(r => r.Rounds);
            AvgScore = results.Average(r => r.Score);
            
            BestResult = results
                .OrderByDescending(r => r.Score)
                .FirstOrDefault()
                ?? new GameEvaluationResult(EGameEvaluationResultType.MaxRoundsExceeded, 0, 0, new Lazy<string>(() => string.Empty));
        }

        public double GetWinRate()
        {
            var wins = ResultTypeTimes.GetValueOrDefault(EGameEvaluationResultType.Win);
            var total = ResultTypeTimes.Sum(k => k.Value);

            return total != 0 ? Convert.ToDouble(wins) / total : 1.0;
        }
    }
}
