using System;
using BankHSE.Domain;
using BankHSE.DomainFactories;
using BankHSE.Facades;

namespace BankHSE.Commands
{
    public class CreateOperationCommand : BaseCommand
    {
        private readonly IDomainFactory _factory;
        private readonly OperationFacade _operationFacade;
        private readonly BankAccountFacade _accountFacade;
        private readonly CategoryFacade _categoryFacade;

        public CreateOperationCommand(
            IDomainFactory factory,
            OperationFacade operationFacade,
            BankAccountFacade accountFacade,
            CategoryFacade categoryFacade)
        {
            _factory = factory;
            _operationFacade = operationFacade;
            _accountFacade = accountFacade;
            _categoryFacade = categoryFacade;
        }

        public override void Execute()
        {
            Console.WriteLine("\n--- Создание операции ---");
            Console.Write("Тип операции (income/expense): ");
            var typeStr = Console.ReadLine()?.ToLower();

            OperationType opType;
            if (typeStr == "income")
                opType = OperationType.Income;
            else if (typeStr == "expense")
                opType = OperationType.Expense;
            else
            {
                Console.WriteLine("Неверный тип операции.");
                return;
            }

            Console.Write("ID счета: ");
            var accountIdStr = Console.ReadLine();
            if (!Guid.TryParse(accountIdStr, out Guid accountId))
            {
                Console.WriteLine("Неверный GUID для счёта.");
                return;
            }

            var account = _accountFacade.GetAccountById(accountId);
            if (account == null)
            {
                Console.WriteLine("Счет не найден.");
                return;
            }

            Console.Write("Сумма операции: ");
            var amountStr = Console.ReadLine();
            if (!decimal.TryParse(amountStr, out decimal amount))
            {
                Console.WriteLine("Неверный ввод суммы.");
                return;
            }

            Console.Write("Описание (необязательно): ");
            var description = Console.ReadLine();

            Console.Write("ID категории: ");
            var categoryIdStr = Console.ReadLine();
            if (!Guid.TryParse(categoryIdStr, out Guid categoryId))
            {
                Console.WriteLine("Неверный GUID категории.");
                return;
            }

            var category = _categoryFacade.GetCategoryById(categoryId);
            if (category == null)
            {
                Console.WriteLine("Категория не найдена.");
                return;
            }

            // Проверка на эквиваленстность типа операции и типа категории
            bool mismatch = (opType == OperationType.Income && category.Type == CategoryType.Expense)
                         || (opType == OperationType.Expense && category.Type == CategoryType.Income);
            if (mismatch)
            {
                Console.WriteLine($"Ошибка: тип операции ({opType}) не совпадает с типом категории ({category.Type}).");
                return;
            }

            // Дата операции
            var date = DateTime.Now;

            try
            {
                var operation = _factory.CreateOperation(opType, accountId, amount, date, description, categoryId);
                _operationFacade.CreateOperation(operation);

                // Обновляем баланс счета
                if (opType == OperationType.Income)
                {
                    account.UpdateBalance(account.Balance + amount);
                }
                else
                {
                    account.UpdateBalance(account.Balance - amount);
                }

                Console.WriteLine($"Операция создана: Id={operation.Id}, Type={operation.Type}, Amount={operation.Amount}, Category={category.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при создании операции: " + ex.Message);
            }
        }
    }
}