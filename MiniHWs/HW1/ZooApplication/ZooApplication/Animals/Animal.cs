using ZooApplication.Interfaces;

namespace ZooApplication.Models.Animals
{
    public abstract class Animal : IAlive, IInventory
    {
        // id каждого животного
        public int Id { get; set; }

        public string Name { get; set; }

        public int Food { get; set; }

        public int Number
        {
            get => Id;
            set => Id = value;
        }

        // Проверка на доступ к контактному зоопарку
        public bool IsContactZoo { get; protected set; }

        protected Animal(int id, string name, int food)
        {
            Id = id;
            Name = name;
            Food = food;
        }
    }
}