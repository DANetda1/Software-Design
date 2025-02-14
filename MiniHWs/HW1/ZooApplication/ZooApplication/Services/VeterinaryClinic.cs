using System;

namespace ZooApplication.Services
{
    public class VeterinaryClinic
    {
        public bool CheckHealth(string animalName)
        {
            Console.WriteLine($"Проверка здоровья животного \"{animalName}\"...");
            // Проверка 50 на 50
            return new Random().Next(2) == 1;
        }
    }
}