using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using ZooApplication.Models.Animals;
using ZooApplication.Models.Things;
using ZooApplication.Services;

namespace ZooApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            // Настраиваем DI
            var serviceProvider = new ServiceCollection()
                .AddSingleton<Zoo>()                // Zoo в виде Singleton
                .AddSingleton<VeterinaryClinic>()   // VetClinic тоже Singleton
                .BuildServiceProvider();

            var zoo = serviceProvider.GetService<Zoo>();
            var clinic = serviceProvider.GetService<VeterinaryClinic>();

            bool exit = false;
            // Основное меню
            while (!exit)
            {
                Console.WriteLine("====================================");
                Console.WriteLine("Меню:");
                Console.WriteLine("1. Добавить животное");
                Console.WriteLine("2. Добавить предмет");
                Console.WriteLine("3. Показать инвентарь");
                Console.WriteLine("4. Показать список животных в контактном зоопарке");
                Console.WriteLine("5. Показать количество травоядных и хищников");
                Console.WriteLine("6. Показать количество животных в контактном зоопарке");
                Console.WriteLine("7. Выход");
                Console.Write("Выберите опцию: ");
                var choice = Console.ReadLine();
                Console.WriteLine("====================================");

                switch (choice)
                {
                    case "1":
                        AddAnimal(zoo, clinic);
                        break;
                    case "2":
                        AddInventory(zoo);
                        break;
                    case "3":
                        ShowInventory(zoo);
                        break;
                    case "4":
                        zoo.ShowContactZooAnimals();
                        break;
                    case "5":
                        ShowHerbivoresAndPredatorsCount(zoo);
                        break;
                    case "6":
                        ShowContactZooCount(zoo);
                        break;
                    case "7":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте ещё раз.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                    Console.ReadKey();
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Программа завершена.");
        }

        // Добавление животного
        static void AddAnimal(Zoo zoo, VeterinaryClinic clinic)
        {
            Console.WriteLine("Выберите тип животного:");
            Console.WriteLine("1. Обезьяна");
            Console.WriteLine("2. Кролик");
            Console.WriteLine("3. Тигр");
            Console.WriteLine("4. Волк");
            Console.WriteLine("5. Другое животное");
            Console.Write("Ваш выбор: ");
            string choice = Console.ReadLine();
            Animal animal = null;

            // Вводим наводящие поля
            string name = ReadNonEmptyString("Введите имя (кличку) животного: ");
            int id = ReadUniqueId("Введите идентификатор (id) животного: ", zoo);
            int food = ReadInt("Введите количество потребляемой пищи (кг/день): ");

            // Выбираем тип
            switch (choice)
            {
                case "1":
                    // Обезьяна -> травоядное. Запрашивается уровень доброты
                    int monkeyKindness = ReadIntInRange("Введите уровень доброты (0-10): ", 0, 10);
                    animal = new Monkey(id, name, food, monkeyKindness);
                    break;

                case "2":
                    // Кролик -> тоже травоядное
                    int rabbitKindness = ReadIntInRange("Введите уровень доброты (0-10): ", 0, 10);
                    animal = new Rabbit(id, name, food, rabbitKindness);
                    break;

                case "3":
                    // Тигр -> хищник
                    animal = new Tiger(id, name, food);
                    break;

                case "4":
                    // Волк -> хищник
                    animal = new Wolf(id, name, food);
                    break;

                case "5":
                    // Другое животное -> Хищник/Травоядное
                    string otherType = ReadNonEmptyString("Введите вид животного: ");
                    bool isPredator = AskYesNo("Данное животное хищник? (Да/Нет): ");

                    if (isPredator)
                    {
                        // Не рассматриваем уровень доброты в данном случае
                        animal = new Predator(id, name, food);
                    }
                    else
                    {
                        // Запрашиваем уровень доброты
                        int kindness = ReadIntInRange("Введите уровень доброты (0-10): ", 0, 10);
                        animal = new Herbo(id, name, food, kindness);
                    }
                    break;

                default:
                    Console.WriteLine("Некорректный выбор типа животного!");
                    return;
            }

            // Проверяем здоровье в ветклинике
            bool isHealthy = clinic.CheckHealth(animal.Name);
            if (isHealthy)
            {
                zoo.AddAnimal(animal);
                Console.WriteLine($"Животное \"{animal.Name}\" (ID: {animal.Id}) успешно добавлено в зоопарк.");
            }
            else
            {
                Console.WriteLine($"Животное \"{animal.Name}\" не прошло проверку здоровья и не будет добавлено.");
            }
        }

        // Добавляем предмет
        static void AddInventory(Zoo zoo)
        {
            Console.WriteLine("Выберите тип предмета:");
            Console.WriteLine("1. Компьютер");
            Console.WriteLine("2. Стол");
            Console.WriteLine("3. Другой предмет");
            Console.Write("Ваш выбор: ");
            var choice = Console.ReadLine();

            Thing item = null;
            int number;
            string name;

            switch (choice)
            {
                case "1":
                    number = ReadUniqueId("Введите инвентарный номер для компьютера: ", zoo);
                    name = ReadNonEmptyString("Введите название/описание компьютера: ");
                    item = new Computer(number, name);
                    break;

                case "2":
                    number = ReadUniqueId("Введите инвентарный номер для стола: ", zoo);
                    name = ReadNonEmptyString("Введите название стола: ");
                    item = new Table(number, name);
                    break;

                case "3":
                    name = ReadNonEmptyString("Введите название предмета: ");
                    number = ReadUniqueId("Введите инвентаризационный номер: ", zoo);
                    item = new Thing(number, name);
                    break;

                default:
                    Console.WriteLine("Некорректный выбор предмета!");
                    return;
            }

            zoo.AddInventory(item);
            Console.WriteLine($"Предмет \"{item.Name}\" (номер: {item.Number}) успешно добавлен в инвентарь.");
        }

        // Показываем инвентарь
        static void ShowInventory(Zoo zoo)
        {
            if (zoo.Inventory.Count == 0)
            {
                Console.WriteLine("Инвентарь пуст.");
            }
            else
            {
                Console.WriteLine("Список предметов (и животных) в инвентаре:");
                int index = 1;
                foreach (var item in zoo.Inventory)
                {
                    // Если это предмет
                    if (item is Thing thing)
                    {
                        Console.WriteLine($"{index}) {thing.GetType().Name}; " +
                                          $"Инв. номер: {thing.Number}; Название: {thing.Name}");
                    }
                    // Если это животное
                    else if (item is Animal animal)
                    {
                        Console.WriteLine($"{index}) {animal.GetType().Name}; " +
                                          $"Id: {animal.Id}; Имя: {animal.Name}; " +
                                          $"Кг еды/день: {animal.Food}");
                    }
                    index++;
                }
            }
        }

        // Показываем кол-во хищников и травоядных
        static void ShowHerbivoresAndPredatorsCount(Zoo zoo)
        {
            // Травоядные
            int herboCount = zoo.Animals.Count(a => a is Herbo);
            // Хищники
            int predatorCount = zoo.Animals.Count(a => a is Predator);

            Console.WriteLine($"Травоядных: {herboCount}, хищников: {predatorCount}");
        }

        // Показываем кол-во животных в контактном зоопарке
        static void ShowContactZooCount(Zoo zoo)
        {
            int contactCount = zoo.Animals
                .Count(a => a is Animal an && an.IsContactZoo);

            Console.WriteLine($"Количество животных в контактном зоопарке: {contactCount}");
        }

        #region Вспомогательные методы для валидации ввода

        // Проверка на пустую строку
        static string ReadNonEmptyString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }
                Console.WriteLine("Ошибка: строка не может быть пустой. Повторите ввод.");
            }
        }

        // Проверка на существующий идентификатор
        static int ReadUniqueId(string prompt, Zoo zoo)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();

                if (int.TryParse(input, out int id))
                {
                    // Проверяем, нет ли уже такого id в инвентаре
                    bool exists = zoo.Inventory.Any(i => i.Number == id);
                    if (!exists)
                    {
                        return id;
                    }
                    Console.WriteLine("Ошибка: такой идентификатор уже существует. Повторите ввод.");
                }
                else
                {
                    Console.WriteLine("Ошибка: введите корректное целое число.");
                }
            }
        }

        // Проверка на целое число
        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (int.TryParse(input, out int number))
                {
                    return number;
                }
                Console.WriteLine("Ошибка: введите целое число.");
            }
        }

        // Проверка на корректное число
        static int ReadIntInRange(string prompt, int min, int max)
        {
            while (true)
            {
                int val = ReadInt(prompt);
                if (val >= min && val <= max)
                {
                    return val;
                }
                Console.WriteLine($"Ошибка: число вне диапазона [{min}..{max}]. Повторите ввод.");
            }
        }

        // Проверка на Да/Нет
        static bool AskYesNo(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var answer = Console.ReadLine();
                if (answer.Equals("Да", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (answer.Equals("Нет", StringComparison.OrdinalIgnoreCase))
                    return false;
                Console.WriteLine("Пожалуйста, введите \"Да\" или \"Нет\".");
            }
        }
    }
    #endregion
}