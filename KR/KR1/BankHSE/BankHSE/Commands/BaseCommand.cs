namespace BankHSE.Commands
{
    public abstract class BaseCommand : ICommand
    {
        public abstract void Execute();
    }
}