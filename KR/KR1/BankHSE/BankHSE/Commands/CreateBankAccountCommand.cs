using System;
using BankHSE.Domain;
using BankHSE.DomainFactories;
using BankHSE.Facades;

namespace BankHSE.Commands
{
    public class CreateBankAccountCommand : BaseCommand
    {
        private readonly IDomainFactory _factory;
        private readonly BankAccountFacade _facade;

        public CreateBankAccountCommand(IDomainFactory factory, BankAccountFacade facade)
        {
            _factory = factory;
            _facade = facade;
        }

        public override void Execute()
        {
            Console.WriteLine("\n--- Создание счета ---");
            Console.Write("Введите название счета: ");
            var name = Console.ReadLine();

            Console.Write("Введите начальный баланс: ");
            var balanceStr = Console.ReadLine();

            if (!decimal.TryParse(balanceStr, out decimal balance))
            {
                Console.WriteLine("Ошибка! Неверный ввод баланса.");
                return;
            }

            try
            {
                var account = _factory.CreateBankAccount(name, balance);
                _facade.CreateAccount(account);
                Console.WriteLine($"Счет создан: Id={account.Id}, Name={account.Name}, Balance={account.Balance}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при создании счета: " + ex.Message);
            }
        }
    }
}