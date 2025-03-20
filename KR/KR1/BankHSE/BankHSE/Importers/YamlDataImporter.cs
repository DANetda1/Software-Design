using BankHSE.Domain;
using BankHSE.DomainFactories;
using BankHSE.Facades;
using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BankHSE.Importers
{
    public class YamlDataImporter : DataImporter
    {
        public YamlDataImporter(
            IDomainFactory factory,
            BankAccountFacade accountFacade,
            CategoryFacade categoryFacade,
            OperationFacade operationFacade)
            : base(factory, accountFacade, categoryFacade, operationFacade)
        {
        }

        protected override void ParseData(string fileContent)
        {
            // Предположим, структура YAML такая же, как и в JSON:
            // Accounts:
            //  - Name: "Счет1"
            //    Balance: 1000
            // Categories:
            //  - Type: "income"
            //    Name: "Зарплата"
            // Operations:
            //  - Type: "income"
            //    AccountId: "..."
            //    Amount: 500
            //    CategoryId: "..."

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var data = deserializer.Deserialize<ImportYamlModel>(fileContent);
            if (data == null) return;

            // Accounts
            if (data.Accounts != null)
            {
                foreach (var accItem in data.Accounts)
                {
                    var acc = Factory.CreateBankAccount(accItem.Name, accItem.Balance);
                    AccountFacade.CreateAccount(acc);
                }
            }

            // Categories
            if (data.Categories != null)
            {
                foreach (var catItem in data.Categories)
                {
                    var ct = catItem.Type.ToLower() == "income" ? CategoryType.Income : CategoryType.Expense;
                    var cat = Factory.CreateCategory(ct, catItem.Name);
                    CategoryFacade.CreateCategory(cat);
                }
            }

            // Operations
            if (data.Operations != null)
            {
                foreach (var opItem in data.Operations)
                {
                    var opType = opItem.Type.ToLower() == "income" ? OperationType.Income : OperationType.Expense;
                    var opAccId = Guid.Parse(opItem.AccountId);
                    var opCatId = Guid.Parse(opItem.CategoryId);

                    var operation = Factory.CreateOperation(opType, opAccId, opItem.Amount, DateTime.Now, "", opCatId);
                    OperationFacade.CreateOperation(operation);

                    // Обновляем баланс
                    var acc = AccountFacade.GetAccountById(opAccId);
                    if (acc != null)
                    {
                        if (opType == OperationType.Income)
                            acc.UpdateBalance(acc.Balance + opItem.Amount);
                        else
                            acc.UpdateBalance(acc.Balance - opItem.Amount);
                    }
                }
            }
        }
    }

    // Модель для десериализации YAML
    public class ImportYamlModel
    {
        public List<YamlAccountModel> Accounts { get; set; }
        public List<YamlCategoryModel> Categories { get; set; }
        public List<YamlOperationModel> Operations { get; set; }
    }

    public class YamlAccountModel
    {
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }

    public class YamlCategoryModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }

    public class YamlOperationModel
    {
        public string Type { get; set; }
        public string AccountId { get; set; }
        public decimal Amount { get; set; }
        public string CategoryId { get; set; }
    }
}