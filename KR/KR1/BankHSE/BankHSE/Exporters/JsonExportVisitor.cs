using BankHSE.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BankHSE.Exporters
{
    public class JsonExportVisitor : IExportVisitor
    {
        // Храним временные списки
        private readonly List<JsonAccountModel> _accounts = new List<JsonAccountModel>();
        private readonly List<JsonCategoryModel> _categories = new List<JsonCategoryModel>();
        private readonly List<JsonOperationModel> _operations = new List<JsonOperationModel>();

        public void Visit(BankAccount account)
        {
            _accounts.Add(new JsonAccountModel
            {
                Name = account.Name,
                Balance = account.Balance
            });
        }

        public void Visit(Category category)
        {
            _categories.Add(new JsonCategoryModel
            {
                Type = category.Type.ToString().ToLower(),
                Name = category.Name
            });
        }

        public void Visit(Operation operation)
        {
            _operations.Add(new JsonOperationModel
            {
                Type = operation.Type.ToString().ToLower(),
                AccountId = operation.BankAccountId.ToString(),
                Amount = operation.Amount,
                CategoryId = operation.CategoryId.ToString()
            });
        }

        public void SaveToFile(string filePath)
        {
            var data = new
            {
                Accounts = _accounts,
                Categories = _categories,
                Operations = _operations
            };
            var json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        private class JsonAccountModel
        {
            public string Name { get; set; }
            public decimal Balance { get; set; }
        }

        private class JsonCategoryModel
        {
            public string Type { get; set; }
            public string Name { get; set; }
        }

        private class JsonOperationModel
        {
            public string Type { get; set; }
            public string AccountId { get; set; }
            public decimal Amount { get; set; }
            public string CategoryId { get; set; }
        }
    }
}