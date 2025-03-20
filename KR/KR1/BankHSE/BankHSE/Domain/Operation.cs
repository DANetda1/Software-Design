using BankHSE.Exporters;
using System;

namespace BankHSE.Domain
{
    public class Operation : IExportable
    {
        public Guid Id { get; private set; }
        public OperationType Type { get; private set; }
        public Guid BankAccountId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime Date { get; private set; }
        public string Description { get; private set; }
        public Guid CategoryId { get; private set; }

        public Operation(
            Guid id,
            OperationType type,
            Guid bankAccountId,
            decimal amount,
            DateTime date,
            string description,
            Guid categoryId)
        {
            Id = id;
            Type = type;
            BankAccountId = bankAccountId;
            Amount = amount;
            Date = date;
            Description = description;
            CategoryId = categoryId;
        }

        public void Accept(IExportVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}