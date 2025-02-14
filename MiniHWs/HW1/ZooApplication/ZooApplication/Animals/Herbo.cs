namespace ZooApplication.Models.Animals
{
    // Класс для травоядных
    public class Herbo : Animal
    {
        // Уровень доброты (от 0 до 10)
        public int KindnessLevel { get; set; }

        public Herbo(int id, string name, int food, int kindnessLevel)
            : base(id, name, food)
        {
            KindnessLevel = kindnessLevel;
            // Если доброта > 5, то IsContactZoo = true
            IsContactZoo = KindnessLevel > 5;
        }
    }
}