using BankHSE.Domain;
using System;

namespace BankHSE.DomainFactories
{
    public class DomainFactory : IDomainFactory
    {
        public BankAccount CreateBankAccount(string name, decimal balance)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Имя счета не может быть пустым.");

            if (balance < 0)
                throw new ArgumentException("Начальный баланс не может быть отрицательным.");

            // Фабрика создаёт объект
            return new BankAccount(Guid.NewGuid(), name, balance);
        }

        public Category CreateCategory(CategoryType type, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Имя категории не может быть пустым.");

            return new Category(Guid.NewGuid(), type, name);
        }

        public Operation CreateOperation(OperationType type, Guid bankAccountId, decimal amount, DateTime date, string description, Guid categoryId)
        {
            if (amount < 0)
                throw new ArgumentException("Сумма операции не может быть отрицательной.");

            return new Operation(Guid.NewGuid(), type, bankAccountId, amount, date, description, categoryId);
        }
    }
}