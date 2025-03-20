using BankHSE.DomainFactories;
using BankHSE.Facades;
using System;
using System.IO;

namespace BankHSE.Importers
{
    public abstract class DataImporter
    {
        protected readonly IDomainFactory Factory;
        protected readonly BankAccountFacade AccountFacade;
        protected readonly CategoryFacade CategoryFacade;
        protected readonly OperationFacade OperationFacade;

        protected DataImporter(
            IDomainFactory factory,
            BankAccountFacade accountFacade,
            CategoryFacade categoryFacade,
            OperationFacade operationFacade)
        {
            Factory = factory;
            AccountFacade = accountFacade;
            CategoryFacade = categoryFacade;
            OperationFacade = operationFacade;
        }

        public void Import(string filePath)
        {
            // Импорт файла

            string fileContent;

            try
            {
                fileContent = ReadFile(filePath);
            }
            catch (Exception ex)
            {
                // Обработка ошибка
                throw new Exception($"Не удалось прочитать файл: {ex.Message}", ex);
            }

            // Распарсим содержимое
            ParseData(fileContent);

            // Вывод

            Console.WriteLine($"Импорт из файла '{filePath}' выполнен успешно!");
        }

        protected string ReadFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        protected abstract void ParseData(string fileContent);
    }
}