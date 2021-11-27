using System;

namespace OregonTrail.Shared
{
    public interface IMessageChannel
    {
        void Write(string msg);
        void WriteLine(string msg);
        string Read();
    }

    public class FakeMessageChannel : IMessageChannel
    {
        public void Write(string msg)
        {
        }

        public void WriteLine(string msg)
        {
        }

        public string Read()
        {
            return string.Empty;
        }
    }

    public class ConsoleMessageChannel : IMessageChannel
    {
        public void Write(string msg)
        {
            Console.Write(msg);
        }

        public void WriteLine(string msg)
        {
            Console.WriteLine(msg);
        }

        public string Read()
        {
            return Console.ReadLine();
        }
    }

    public class MessageChannelListener : IMessageChannel
    {
        private readonly IMessageChannel _messageChannel;

        public MessageChannelListener(IMessageChannel messageChannel)
        {
            _messageChannel = messageChannel;
        }

        public void Write(string msg)
        {
            Console.Write(msg);
            _messageChannel.Write(msg);
        }

        public void WriteLine(string msg)
        {
            Console.WriteLine(msg);
            _messageChannel.WriteLine(msg);
        }

        public string Read()
        {
            var msg = _messageChannel.Read();
            Console.WriteLine(msg);

            var c = Console.ReadKey();
            if (c.Key == ConsoleKey.Q)
                throw new Exception();

            return msg;
        }
    }

    public static class MessageChannelExtensions
    {
        public static void Write(this IMessageChannel messageChannel, string msg, params object[] args)
        {
            messageChannel.Write(string.Format(msg, args));
        }

        public static void WriteLine(this IMessageChannel messageChannel, string msg, params object[] args)
        {
            messageChannel.WriteLine(string.Format(msg, args));
        }

        public static void WriteLine(this IMessageChannel messageChannel)
        {
            messageChannel.WriteLine(string.Empty);
        }

        public static bool TryReadInt(this IMessageChannel messageChannel, out int i)
        {
            var input = messageChannel.Read();
            return int.TryParse(input, out i);
        }
    }
}
