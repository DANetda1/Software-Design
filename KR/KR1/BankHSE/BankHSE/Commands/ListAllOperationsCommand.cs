using System;
using BankHSE.Facades;

namespace BankHSE.Commands
{
    public class ListAllOperationsCommand : BaseCommand
    {
        private readonly OperationFacade _facade;

        public ListAllOperationsCommand(OperationFacade facade)
        {
            _facade = facade;
        }

        public override void Execute()
        {
            Console.WriteLine("\n--- Список всех операций ---");
            var all = _facade.GetAllOperations();
            foreach (var op in all)
            {
                Console.WriteLine($"Id={op.Id}, Type={op.Type}, AccountId={op.BankAccountId}, Amount={op.Amount}, CategoryId={op.CategoryId}, Date={op.Date}, Desc={op.Description}");
            }
        }
    }
}