using System;
using System.Collections.Generic;
using System.Linq;
using ZooApplication.Interfaces;
using ZooApplication.Models.Animals;

namespace ZooApplication.Services
{
    public class Zoo
    {
        // Список животных
        public List<IAlive> Animals { get; private set; } = new List<IAlive>();

        // Список инвентаря
        public List<IInventory> Inventory { get; private set; } = new List<IInventory>();

        // Добавляем животное
        public void AddAnimal(IAlive animal)
        {
            Animals.Add(animal);
            if (animal is IInventory inv)
            {
                Inventory.Add(inv);
            }
        }
    
        // Добавление предмета
        public void AddInventory(IInventory item)
        {
            Inventory.Add(item);
        }

        // Животные, подходящие для контактного зоопарка
        public void ShowContactZooAnimals()
        {
            var contactZooAnimals = Animals
                .Where(a =>
                    a is Animal animal && animal.IsContactZoo
                )
                .ToList();

            if (contactZooAnimals.Count == 0)
            {
                Console.WriteLine("На данный момент в контактном зоопарке нет животных");
            }
            else
            {
                int index = 1;
                foreach (var alive in contactZooAnimals)
                {
                    if (alive is Herbo herbo)
                    {
                        Console.WriteLine(
                            $"{index}) " +
                            $"Вид: {herbo.GetType().Name}; Имя: {herbo.Name}; " +
                            $"id: {herbo.Id}; кол-во потребляемой пищи: {herbo.Food}кг/день; " +
                            $"уровень доброты: {herbo.KindnessLevel}."
                        );
                        index++;
                    }
                    else if (alive is Predator predator)
                    {
                        Console.WriteLine(
                            $"{index}) " +
                            $"Вид: {predator.GetType().Name}; Имя: {predator.Name}; " +
                            $"id: {predator.Id}; кол-во потребляемой пищи: {predator.Food}кг/день."
                        );
                        index++;
                    }
                }
            }
        }
    }
}