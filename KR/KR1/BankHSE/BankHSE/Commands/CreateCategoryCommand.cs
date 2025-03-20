using System;
using BankHSE.Domain;
using BankHSE.DomainFactories;
using BankHSE.Facades;

namespace BankHSE.Commands
{
    public class CreateCategoryCommand : BaseCommand
    {
        private readonly IDomainFactory _factory;
        private readonly CategoryFacade _facade;

        public CreateCategoryCommand(IDomainFactory factory, CategoryFacade facade)
        {
            _factory = factory;
            _facade = facade;
        }

        public override void Execute()
        {
            Console.WriteLine("\n--- Создание категории ---");
            Console.Write("Введите название категории: ");
            var name = Console.ReadLine();

            Console.Write("Тип категории (income/expense): ");
            var typeStr = Console.ReadLine()?.ToLower();

            CategoryType categoryType;
            if (typeStr == "income")
                categoryType = CategoryType.Income;
            else if (typeStr == "expense")
                categoryType = CategoryType.Expense;
            else
            {
                Console.WriteLine("Неверный тип категории.");
                return;
            }

            try
            {
                var category = _factory.CreateCategory(categoryType, name);
                _facade.CreateCategory(category);
                Console.WriteLine($"Категория создана: Id={category.Id}, Type={category.Type}, Name={category.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при создании категории: " + ex.Message);
            }
        }
    }
}