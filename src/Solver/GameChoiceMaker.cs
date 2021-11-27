using OregonTrail.Shared;
using System;
using System.Diagnostics.Contracts;
using System.Text;

namespace OregonTrail.Solver
{
    public class GameChoiceMaker : IMessageChannel
    {
        private enum EChoiceType
        {
            ArbitraryNumber,
            WordFromMessage,
            WordFromFirstLine,
            WordFromLastLine,
            NumberFromMessage,
            NumberFromFirstLine,
            NumberFromLastLine,
            PreviousChoice,
            PreviousChoiceMessage,
            PreviousChoiceFirstLine,
            PreviousChoiceLastLine
        }

        private const int EChoiceTypeLength = 11;

        public const int DecisionInputSize = 17;
        public const int DecisionOutputSize = EChoiceTypeLength + 1;

        public DecisionEngine DecisionEngine { get; }

        private readonly MessageInterpreter _messageInterpreter;
        private readonly MessageIndexer _messageIndexer;
        private readonly GameChoiceMemory _gameChoiceMemory;
        private StringBuilder _sb;
        
        public GameChoiceMaker(DecisionEngine decisionEngine, MessageInterpreter messageInterpreter, MessageIndexer messageIndexer)
        {
            Contract.Requires(decisionEngine.InputSize == DecisionInputSize);
            Contract.Requires(decisionEngine.OutputSize == DecisionOutputSize);

            DecisionEngine = decisionEngine;
            _messageInterpreter = messageInterpreter;
            _messageIndexer = messageIndexer;

            _gameChoiceMemory = new GameChoiceMemory();
            _sb = new StringBuilder();
        }

        public void Write(string msg)
        {
            _sb.Append(msg);
        }

        public void WriteLine(string msg)
        {
            _sb.Append(msg);
            _sb.Append("\r\n");
        }

        public string Read()
        {
            var msg = _sb.ToString();
            _sb = new StringBuilder();

            var interpretedMessage = _messageInterpreter.Interpret(msg);

            var message = _messageIndexer.GetMessageIndex(interpretedMessage.Message.Value);
            var firstLine = _messageIndexer.GetMessageIndex(interpretedMessage.FirstLine.Value);
            var lastLine = _messageIndexer.GetMessageIndex(interpretedMessage.LastLine.Value);
            
            var input = CreateDecisionInput(interpretedMessage, message, firstLine, lastLine);
            var output = DecisionEngine.Decide(input);

            var choiceType = GetChoiceType(output, interpretedMessage, message, firstLine, lastLine);
            var choiceStr = GetChoiceMessage(choiceType, output, interpretedMessage, message, firstLine, lastLine);

            var choice = _messageIndexer.GetMessageIndex(choiceStr);

            _gameChoiceMemory.Update(message, firstLine, lastLine, choice, choiceStr);
            
            return choiceStr;
        }

        private double[] CreateDecisionInput(InterpretedMessage interpretedMessage, int message, int firstLine, int lastLine)
        {
            return new double[]
            {
                message,
                firstLine,
                lastLine,

                interpretedMessage.Message.Numbers.Length,
                interpretedMessage.FirstLine.Numbers.Length,
                interpretedMessage.LastLine.Numbers.Length,

                interpretedMessage.Message.Words.Length,
                interpretedMessage.FirstLine.Words.Length,
                interpretedMessage.LastLine.Words.Length,

                _gameChoiceMemory.PreviousChoice,

                _gameChoiceMemory.MessageMemory.GetLastChoice(message),
                _gameChoiceMemory.FirstLineMemory.GetLastChoice(firstLine),
                _gameChoiceMemory.LastLineMemory.GetLastChoice(lastLine),

                _gameChoiceMemory.CountEmptyMessage,

                _gameChoiceMemory.MessageMemory.CountSameMessage,
                _gameChoiceMemory.FirstLineMemory.CountSameMessage,
                _gameChoiceMemory.LastLineMemory.CountSameMessage
            };
        }

        private EChoiceType GetChoiceType(double[] output, InterpretedMessage interpretedMessage, int message, int firstLine, int lastLine)
        {
            var result = (EChoiceType)0;

            var previousChance = output[0];
            for (var i = 1; i < EChoiceTypeLength; i++)
            {
                var currentChance = output[i];
                if (currentChance < previousChance)
                    continue;

                var c = (EChoiceType)i;
                switch (c)
                {
                    case EChoiceType.WordFromMessage:
                        if (interpretedMessage.Message.Words.Length == 0)
                            continue;
                        break;
                    case EChoiceType.WordFromFirstLine:
                        if (interpretedMessage.FirstLine.Words.Length == 0)
                            continue;
                        break;
                    case EChoiceType.WordFromLastLine:
                        if (interpretedMessage.LastLine.Words.Length == 0)
                            continue;
                        break;
                    case EChoiceType.NumberFromMessage:
                        if (interpretedMessage.Message.Numbers.Length == 0)
                            continue;
                        break;
                    case EChoiceType.NumberFromFirstLine:
                        if (interpretedMessage.FirstLine.Numbers.Length == 0)
                            continue;
                        break;
                    case EChoiceType.NumberFromLastLine:
                        if (interpretedMessage.LastLine.Numbers.Length == 0)
                            continue;
                        break;
                    case EChoiceType.PreviousChoice:
                        if (_gameChoiceMemory.PreviousChoice < 0)
                            continue;
                        break;
                    case EChoiceType.PreviousChoiceMessage:
                        if (_gameChoiceMemory.MessageMemory.GetLastChoice(message) < 0)
                            continue;
                        break;
                    case EChoiceType.PreviousChoiceFirstLine:
                        if (_gameChoiceMemory.FirstLineMemory.GetLastChoice(firstLine) < 0)
                            continue;
                        break;
                    case EChoiceType.PreviousChoiceLastLine:
                        if (_gameChoiceMemory.LastLineMemory.GetLastChoice(lastLine) < 0)
                            continue;
                        break;
                }

                result = c;
                previousChance = currentChance;
            }

            return result;
        }

        private string GetChoiceMessage(EChoiceType choiceType, double[] output, InterpretedMessage interpretedMessage, int message, int firstLine, int lastLine)
        {
            double choiceOutput = output[EChoiceTypeLength];

            switch (choiceType)
            {
                case EChoiceType.ArbitraryNumber:
                    return Convert.ToInt32(Math.Floor(choiceOutput * 1000.0)).ToString();
                case EChoiceType.WordFromMessage:
                    return SelectFromArray(choiceOutput, interpretedMessage.Message.Words);
                case EChoiceType.WordFromFirstLine:
                    return SelectFromArray(choiceOutput, interpretedMessage.FirstLine.Words);
                case EChoiceType.WordFromLastLine:
                    return SelectFromArray(choiceOutput, interpretedMessage.LastLine.Words);
                case EChoiceType.NumberFromMessage:
                    return SelectFromArray(choiceOutput, interpretedMessage.Message.Numbers);
                case EChoiceType.NumberFromFirstLine:
                    return SelectFromArray(choiceOutput, interpretedMessage.FirstLine.Numbers);
                case EChoiceType.NumberFromLastLine:
                    return SelectFromArray(choiceOutput, interpretedMessage.LastLine.Numbers);
                case EChoiceType.PreviousChoice:
                    return _gameChoiceMemory.PreviousChoiceText;
                case EChoiceType.PreviousChoiceMessage:
                    return _gameChoiceMemory.MessageMemory.GetLastChoiceText(message);
                case EChoiceType.PreviousChoiceFirstLine:
                    return _gameChoiceMemory.FirstLineMemory.GetLastChoiceText(firstLine);
                case EChoiceType.PreviousChoiceLastLine:
                    return _gameChoiceMemory.LastLineMemory.GetLastChoiceText(lastLine);
                default:
                    return string.Empty;
            }
        }

        private string SelectFromArray(double output, string[] options)
        {
            if (options.Length == 0)
                return string.Empty;

            var i = Convert.ToInt32(Math.Floor(output * options.Length));
            return options[i < options.Length ? i : options.Length - 1];
        }
    }
}
