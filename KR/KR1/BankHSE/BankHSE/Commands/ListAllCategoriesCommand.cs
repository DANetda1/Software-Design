using System;
using BankHSE.Facades;

namespace BankHSE.Commands
{
    public class ListAllCategoriesCommand : BaseCommand
    {
        private readonly CategoryFacade _facade;

        public ListAllCategoriesCommand(CategoryFacade facade)
        {
            _facade = facade;
        }

        public override void Execute()
        {
            Console.WriteLine("\n--- Список всех категорий ---");
            var all = _facade.GetAllCategories();
            foreach (var cat in all)
            {
                Console.WriteLine($"Id={cat.Id}, Type={cat.Type}, Name={cat.Name}");
            }
        }
    }
}