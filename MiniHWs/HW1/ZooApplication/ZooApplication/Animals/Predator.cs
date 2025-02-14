namespace ZooApplication.Models.Animals
{
    // Класс для хищников
    public class Predator : Animal
    {
        public Predator(int id, string name, int food)
            : base(id, name, food)
        {
            // У хищников IsContactZoo = false всегда.
            IsContactZoo = false;
        }
    }
}