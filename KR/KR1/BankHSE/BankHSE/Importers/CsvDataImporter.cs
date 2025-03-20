using BankHSE.Domain;
using BankHSE.DomainFactories;
using BankHSE.Facades;
using System;

namespace BankHSE.Importers
{
    public class CsvDataImporter : DataImporter
    {
        public CsvDataImporter(
            IDomainFactory factory,
            BankAccountFacade accountFacade,
            CategoryFacade categoryFacade,
            OperationFacade operationFacade)
            : base(factory, accountFacade, categoryFacade, operationFacade)
        {
        }

        protected override void ParseData(string fileContent)
        {
            var lines = fileContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = line.Trim().Split(';');
                if (parts.Length == 0) continue;

                var recordType = parts[0].ToLower();
                switch (recordType)
                {
                    case "account":
                        // parts: [account, name, balance]
                        if (parts.Length < 3) continue;
                        var accName = parts[1];
                        var accBalance = decimal.Parse(parts[2]);
                        var accObj = Factory.CreateBankAccount(accName, accBalance);
                        AccountFacade.CreateAccount(accObj);
                        break;

                    case "category":
                        // parts: [category, income/expense, name]
                        if (parts.Length < 3) continue;
                        var catTypeStr = parts[1].ToLower();
                        var catName = parts[2];
                        var catType = catTypeStr == "income" ? CategoryType.Income : CategoryType.Expense;
                        var catObj = Factory.CreateCategory(catType, catName);
                        CategoryFacade.CreateCategory(catObj);
                        break;

                    case "operation":
                        // parts: [operation, income/expense, accountId, amount, categoryId]
                        if (parts.Length < 5) continue;
                        var opTypeStr = parts[1].ToLower();
                        var opType = opTypeStr == "income" ? OperationType.Income : OperationType.Expense;
                        var opAccId = Guid.Parse(parts[2]);
                        var opAmount = decimal.Parse(parts[3]);
                        var opCatId = Guid.Parse(parts[4]);
                        var opObj = Factory.CreateOperation(opType, opAccId, opAmount, DateTime.Now, "", opCatId);
                        OperationFacade.CreateOperation(opObj);

                        // Обновляем баланс
                        var acc = AccountFacade.GetAccountById(opAccId);
                        if (acc != null)
                        {
                            if (opType == OperationType.Income)
                            {
                                acc.UpdateBalance(acc.Balance + opAmount);
                            }
                            else
                            {
                                acc.UpdateBalance(acc.Balance - opAmount);
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}