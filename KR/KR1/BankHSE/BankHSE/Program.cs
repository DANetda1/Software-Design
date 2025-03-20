// Магомедов Абдул Омаргаджиевич. БПИ 234.
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using BankHSE.Commands;
using BankHSE.DomainFactories;
using BankHSE.Facades;
using BankHSE.Importers;
using BankHSE.Exporters;

namespace BankHSE
{
    internal class Program
    {
        static void Main()
        {
            // Настраиваем DI
            var services = new ServiceCollection();
            services.AddSingleton<IDomainFactory, DomainFactory>();
            services.AddSingleton<BankAccountFacade>();
            services.AddSingleton<CategoryFacade>();
            services.AddSingleton<OperationFacade>();
            services.AddSingleton<AnalyticsFacade>();
            services.AddTransient<CommandInvoker>();

            var sp = services.BuildServiceProvider();

            // Получаем объекты из контейнера
            var domainFactory = sp.GetRequiredService<IDomainFactory>();
            var accountFacade = sp.GetRequiredService<BankAccountFacade>();
            var categoryFacade = sp.GetRequiredService<CategoryFacade>();
            var operationFacade = sp.GetRequiredService<OperationFacade>();
            var analyticsFacade = sp.GetRequiredService<AnalyticsFacade>();
            var commandInvoker = sp.GetRequiredService<CommandInvoker>();

            while (true)
            {
                Console.WriteLine("\n========= ВШЭ-Банк: Учет финансов =========");
                Console.WriteLine("1) Создать счет");
                Console.WriteLine("2) Создать категорию");
                Console.WriteLine("3) Создать операцию (доход/расход)");
                Console.WriteLine("4) Список счетов");
                Console.WriteLine("5) Список категорий");
                Console.WriteLine("6) Список операций");
                Console.WriteLine("7) Аналитика");
                Console.WriteLine("8) Импорт (CSV / JSON / YAML)");
                Console.WriteLine("9) Экспорт (CSV / JSON / YAML)");
                Console.WriteLine("0) Выход");
                Console.Write("Выберите действие: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "0":
                        return;

                    case "1":
                        {
                            var cmd = new CreateBankAccountCommand(domainFactory, accountFacade);
                            commandInvoker.ExecuteCommand(new TimedCommandDecorator(cmd));
                            break;
                        }

                    case "2":
                        {
                            var cmd = new CreateCategoryCommand(domainFactory, categoryFacade);
                            commandInvoker.ExecuteCommand(new TimedCommandDecorator(cmd));
                            break;
                        }

                    case "3":
                        {
                            var cmd = new CreateOperationCommand(domainFactory, operationFacade, accountFacade, categoryFacade);
                            commandInvoker.ExecuteCommand(new TimedCommandDecorator(cmd));
                            break;
                        }

                    case "4":
                        {
                            var cmd = new ListAllAccountsCommand(accountFacade);
                            commandInvoker.ExecuteCommand(new TimedCommandDecorator(cmd));
                            break;
                        }

                    case "5":
                        {
                            var cmd = new ListAllCategoriesCommand(categoryFacade);
                            commandInvoker.ExecuteCommand(new TimedCommandDecorator(cmd));
                            break;
                        }

                    case "6":
                        {
                            var cmd = new ListAllOperationsCommand(operationFacade);
                            commandInvoker.ExecuteCommand(new TimedCommandDecorator(cmd));
                            break;
                        }

                    case "7":
                        {
                            Console.WriteLine("\n-- Аналитика --");
                            var diff = analyticsFacade.GetIncomeMinusExpenses();
                            Console.WriteLine($"Разница доходов и расходов: {diff}");
                            analyticsFacade.PrintOperationsGroupedByCategory();
                            break;
                        }

                    case "8":
                        {
                            // Импорт
                            Console.WriteLine("\nВыберите формат для импорта (csv/json/yaml): ");
                            var format = Console.ReadLine()?.ToLower();
                            DataImporter importer;

                            switch (format)
                            {
                                case "csv":
                                    importer = new CsvDataImporter(domainFactory, accountFacade, categoryFacade, operationFacade);
                                    Console.WriteLine("Пример пути для импорта: C:\\temp\\data.csv");
                                    break;
                                case "json":
                                    importer = new JsonDataImporter(domainFactory, accountFacade, categoryFacade, operationFacade);
                                    Console.WriteLine("Пример пути для импорта: C:\\temp\\data.json");
                                    break;
                                case "yaml":
                                    importer = new YamlDataImporter(domainFactory, accountFacade, categoryFacade, operationFacade);
                                    Console.WriteLine("Пример пути для импорта: C:\\temp\\data.yaml");
                                    break;
                                default:
                                    Console.WriteLine("Неверный формат!");
                                    continue;
                            }

                            Console.Write("Введите полный путь к файлу: ");
                            var filePath = Console.ReadLine();

                            // Проверка расширения
                            if (!CheckFileExtension(filePath, format))
                            {
                                Console.WriteLine($"Ошибка: расширение файла не соответствует формату {format}.");
                                break;
                            }

                            try
                            {
                                importer.Import(filePath);
                                Console.WriteLine("Импорт выполнен!");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Ошибка при импорте: " + ex.Message);
                            }
                            break;
                        }

                    case "9":
                        {
                            // Экспорт
                            Console.WriteLine("\nВыберите формат для экспорта (csv/json/yaml): ");
                            var format = Console.ReadLine()?.ToLower();
                            IExportVisitor visitor;

                            switch (format)
                            {
                                case "csv":
                                    visitor = new CsvExportVisitor();
                                    Console.WriteLine("Пример пути для экспорта: C:\\temp\\export.csv");
                                    break;
                                case "json":
                                    visitor = new JsonExportVisitor();
                                    Console.WriteLine("Пример пути для экспорта: C:\\temp\\export.json");
                                    break;
                                case "yaml":
                                    visitor = new YamlExportVisitor();
                                    Console.WriteLine("Пример пути для экспорта: C:\\temp\\export.yaml");
                                    break;
                                default:
                                    Console.WriteLine("Неверный формат!");
                                    continue;
                            }

                            Console.Write("Введите полный путь для сохранения файла: ");
                            var filePath = Console.ReadLine();

                            if (!CheckFileExtension(filePath, format))
                            {
                                Console.WriteLine($"Ошибка: расширение файла не соответствует формату {format}.");
                                break;
                            }

                            try
                            {
                                foreach (var acc in accountFacade.GetAllAccounts())
                                    acc.Accept(visitor);

                                foreach (var cat in categoryFacade.GetAllCategories())
                                    cat.Accept(visitor);

                                foreach (var op in operationFacade.GetAllOperations())
                                    op.Accept(visitor);

                                visitor.SaveToFile(filePath);
                                Console.WriteLine("Данные успешно экспортированы!");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Ошибка при сохранении файла: " + ex.Message);
                            }
                            break;
                        }

                    default:
                        Console.WriteLine("Неверный ввод!");
                        break;
                }
            }
        }

        // Проверка расширения
        private static bool CheckFileExtension(string path, string format)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            var lowerExt = Path.GetExtension(path).ToLower();
            return format switch
            {
                "csv" => lowerExt == ".csv",
                "json" => lowerExt == ".json",
                "yaml" => lowerExt == ".yaml" || lowerExt == ".yml",
                _ => false
            };
        }
    }
}