using BankHSE.Domain;
using System;

namespace BankHSE.DomainFactories
{
    public interface IDomainFactory
    {
        BankAccount CreateBankAccount(string name, decimal balance);
        Category CreateCategory(CategoryType type, string name);
        Operation CreateOperation(OperationType type, Guid bankAccountId, decimal amount, DateTime date, string description, Guid categoryId);
    }
}