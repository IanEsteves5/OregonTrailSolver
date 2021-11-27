using System.Collections.Generic;

namespace OregonTrail.Solver
{
    public class MessageIndexer
    {
        private readonly Dictionary<int, int> _knownMessages;
        private int _nextIndex;

        public MessageIndexer()
        {
            _knownMessages = new Dictionary<int, int>();
            _nextIndex = 0;
        }

        public int GetMessageIndex(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
                return -1;

            var hash = msg.Trim().ToUpper().GetHashCode();

            int index;
            if (!_knownMessages.TryGetValue(hash, out index))
                index = _knownMessages[hash] = _nextIndex++;

            return index;
        }
    }
}
