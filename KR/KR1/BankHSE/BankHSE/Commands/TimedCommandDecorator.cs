using System;
using System.Diagnostics;

namespace BankHSE.Commands
{
    public class TimedCommandDecorator : ICommand
    {
        private readonly ICommand _innerCommand;

        public TimedCommandDecorator(ICommand innerCommand)
        {
            _innerCommand = innerCommand;
        }

        public void Execute()
        {
            var stopwatch = Stopwatch.StartNew();
            _innerCommand.Execute();
            stopwatch.Stop();
            Console.WriteLine($"[ВРЕМЯ ВЫПОЛНЕНИЯ]: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}