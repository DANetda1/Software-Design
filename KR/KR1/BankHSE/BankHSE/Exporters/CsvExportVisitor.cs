using BankHSE.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BankHSE.Exporters
{
    public class CsvExportVisitor : IExportVisitor
    {
        private readonly List<string> _lines = new List<string>();

        public void Visit(BankAccount account)
        {
            // Сохраняем строку в _lines
            _lines.Add($"account;{account.Name};{account.Balance}");
        }

        public void Visit(Category category)
        {
            _lines.Add($"category;{category.Type.ToString().ToLower()};{category.Name}");
        }

        public void Visit(Operation operation)
        {
            _lines.Add($"operation;{operation.Type.ToString().ToLower()};{operation.BankAccountId};{operation.Amount};{operation.CategoryId}");
        }

        public void SaveToFile(string filePath)
        {
            File.WriteAllLines(filePath, _lines, Encoding.UTF8);
        }
    }
}