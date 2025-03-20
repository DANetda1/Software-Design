using BankHSE.Exporters;
using System;

namespace BankHSE.Domain
{
    public class BankAccount : IExportable
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal Balance { get; private set; }


        public BankAccount(Guid id, string name, decimal balance)
        {
            Id = id;
            Name = name;
            Balance = balance;
        }

        public void UpdateBalance(decimal newBalance)
        {
            Balance = newBalance;
        }

        public void Accept(IExportVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}