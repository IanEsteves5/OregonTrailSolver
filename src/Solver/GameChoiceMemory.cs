using OregonTrail.Shared;
using System;
using System.Collections.Generic;

namespace OregonTrail.Solver
{
    public class GameChoiceMemory
    {
        public int PreviousChoice { get; private set; }
        public string PreviousChoiceText { get; private set; }
        public int CountEmptyMessage { get; private set; }

        public MessageChoiceMemory MessageMemory { get; }
        public MessageChoiceMemory FirstLineMemory { get; }
        public MessageChoiceMemory LastLineMemory { get; }

        public GameChoiceMemory()
        {
            PreviousChoice = -1;
            PreviousChoiceText = string.Empty;

            MessageMemory = new MessageChoiceMemory();
            FirstLineMemory = new MessageChoiceMemory();
            LastLineMemory = new MessageChoiceMemory();
        }

        public void Update(int message, int firstLine, int lastLine, int choice, string choiceText)
        {
            PreviousChoice = choice;
            PreviousChoiceText = choiceText;

            if (message < 0)
                CountEmptyMessage++;
            else
                CountEmptyMessage = 0;

            MessageMemory.Update(message, choice, choiceText);
            FirstLineMemory.Update(firstLine, choice, choiceText);
            LastLineMemory.Update(lastLine, choice, choiceText);
        }
    }

    public class MessageChoiceMemory
    {
        private int _previousMessage;
        private readonly Dictionary<int, Tuple<int, string>> _lastChoicesByMessage;

        public int CountSameMessage { get; private set; }

        public MessageChoiceMemory()
        {
            _previousMessage = -1;
            _lastChoicesByMessage = new Dictionary<int, Tuple<int, string>>();
        }

        public void Update(int message, int choice, string choiceText)
        {
            if (message < 0)
            {
                if (_previousMessage >= 0)
                    _lastChoicesByMessage[_previousMessage] = Tuple.Create(choice, choiceText);
                return;
            }

            if (_previousMessage == message)
                CountSameMessage++;
            else
            {
                _previousMessage = message;
                CountSameMessage = 0;
            }

            _lastChoicesByMessage[message] = Tuple.Create(choice, choiceText);
        }

        public int GetLastChoice(int message)
        {
            return _lastChoicesByMessage.GetValueOrDefault(message)?.Item1 ?? -1;
        }

        public string GetLastChoiceText(int message)
        {
            return _lastChoicesByMessage.GetValueOrDefault(message)?.Item2 ?? string.Empty;
        }
    }
}
