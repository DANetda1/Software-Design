using System;
using BankHSE.Facades;

namespace BankHSE.Commands
{
    public class ListAllAccountsCommand : BaseCommand
    {
        private readonly BankAccountFacade _facade;

        public ListAllAccountsCommand(BankAccountFacade facade)
        {
            _facade = facade;
        }

        public override void Execute()
        {
            Console.WriteLine("\n--- Список всех счетов ---");
            var all = _facade.GetAllAccounts();
            foreach (var acc in all)
            {
                Console.WriteLine($"Id={acc.Id}, Name={acc.Name}, Balance={acc.Balance}");
            }
        }
    }
}