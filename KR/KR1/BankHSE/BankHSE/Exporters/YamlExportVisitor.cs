using BankHSE.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BankHSE.Exporters
{
    public class YamlExportVisitor : IExportVisitor
    {
        private readonly List<YamlAccountModel> _accounts = new List<YamlAccountModel>();
        private readonly List<YamlCategoryModel> _categories = new List<YamlCategoryModel>();
        private readonly List<YamlOperationModel> _operations = new List<YamlOperationModel>();

        public void Visit(BankAccount account)
        {
            _accounts.Add(new YamlAccountModel
            {
                Name = account.Name,
                Balance = account.Balance
            });
        }

        public void Visit(Category category)
        {
            _categories.Add(new YamlCategoryModel
            {
                Type = category.Type.ToString().ToLower(),
                Name = category.Name
            });
        }

        public void Visit(Operation operation)
        {
            _operations.Add(new YamlOperationModel
            {
                Type = operation.Type.ToString().ToLower(),
                AccountId = operation.BankAccountId.ToString(),
                Amount = operation.Amount,
                CategoryId = operation.CategoryId.ToString()
            });
        }

        public void SaveToFile(string filePath)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var data = new
            {
                Accounts = _accounts,
                Categories = _categories,
                Operations = _operations
            };

            var yaml = serializer.Serialize(data);
            File.WriteAllText(filePath, yaml, Encoding.UTF8);
        }

        private class YamlAccountModel
        {
            public string Name { get; set; }
            public decimal Balance { get; set; }
        }

        private class YamlCategoryModel
        {
            public string Type { get; set; }
            public string Name { get; set; }
        }

        private class YamlOperationModel
        {
            public string Type { get; set; }
            public string AccountId { get; set; }
            public decimal Amount { get; set; }
            public string CategoryId { get; set; }
        }
    }
}