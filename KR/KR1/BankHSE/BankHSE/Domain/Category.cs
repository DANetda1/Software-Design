using BankHSE.Exporters;
using System;

namespace BankHSE.Domain
{
    public class Category : IExportable
    {
        public Guid Id { get; private set; }
        public CategoryType Type { get; private set; }
        public string Name { get; private set; }

        public Category(Guid id, CategoryType type, string name)
        {
            Id = id;
            Type = type;
            Name = name;
        }

        public void Accept(IExportVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}