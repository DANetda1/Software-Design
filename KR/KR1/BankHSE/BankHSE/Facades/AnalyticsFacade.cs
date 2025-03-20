using BankHSE.Domain;
using System;
using System.Linq;

namespace BankHSE.Facades
{
    public class AnalyticsFacade
    {
        private readonly BankAccountFacade _accountFacade;
        private readonly OperationFacade _operationFacade;

        public AnalyticsFacade(BankAccountFacade accountFacade, OperationFacade operationFacade)
        {
            _accountFacade = accountFacade;
            _operationFacade = operationFacade;
        }

        // Разница доходов/расходов
        public decimal GetIncomeMinusExpenses()
        {
            var operations = _operationFacade.GetAllOperations();
            decimal income = operations.Where(o => o.Type == OperationType.Income).Sum(o => o.Amount);
            decimal expense = operations.Where(o => o.Type == OperationType.Expense).Sum(o => o.Amount);

            return income - expense;
        }

        // Группировка по категориям
        public void PrintOperationsGroupedByCategory()
        {
            var operations = _operationFacade.GetAllOperations();

            var grouped = operations
                .GroupBy(o => o.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count(), Total = g.Sum(op => op.Amount) });

            Console.WriteLine("\n--- Операции по категориям ---");
            foreach (var group in grouped)
            {
                Console.WriteLine($"КатегорияID: {group.CategoryId}, Кол-во операций: {group.Count}, Сумма: {group.Total}");
            }
        }
    }
}