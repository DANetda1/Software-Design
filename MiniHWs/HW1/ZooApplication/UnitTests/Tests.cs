using Xunit;
using ZooApplication.Services;
using ZooApplication.Models.Animals;

namespace ZooApplication.Tests
{
    public class ZooTests
    {
        [Fact]
        public void AddAnimal_ShouldIncreaseAnimalsCount()
        {
            var zoo = new Zoo();
            var animal = new Monkey(1, "Чичи", 5, 7);

            zoo.AddAnimal(animal);

            Assert.Single(zoo.Animals);
        }

        [Fact]
        public void AnimalWithKindnessAbove5_ShouldBeInContactZoo()
        {
            var rabbit = new Rabbit(2, "Ушастик", 3, 6);
            Assert.True(rabbit.IsContactZoo);
        }

        [Fact]
        public void Predator_ShouldNotBeInContactZoo()
        {
            var wolf = new Wolf(3, "Серый", 7);

            Assert.False(wolf.IsContactZoo);
        }

        [Fact]
        public void ShouldCorrectlyCountHerbivoresAndPredators()
        {
            var zoo = new Zoo();
            zoo.AddAnimal(new Monkey(1, "Обезьяна", 5, 7));
            zoo.AddAnimal(new Tiger(2, "Тигр", 10));
            zoo.AddAnimal(new Rabbit(3, "Кролик", 3, 6));
            zoo.AddAnimal(new Wolf(4, "Волк", 8));

            var herbivoresCount = zoo.Animals.Count(a => a is Herbo);
            var predatorsCount = zoo.Animals.Count(a => a is Predator);

            Assert.Equal(2, herbivoresCount);
            Assert.Equal(2, predatorsCount);
        }

        [Fact]
        public void ShouldCorrectlyCountAnimalsInContactZoo()
        {
            var zoo = new Zoo();
            var herbivore1 = new Monkey(1, "Обезьяна", 5, 7);
            var herbivore2 = new Rabbit(2, "Кролик", 3, 4);
            var predator = new Tiger(3, "Тигр", 10);

            zoo.AddAnimal(herbivore1);
            zoo.AddAnimal(herbivore2);
            zoo.AddAnimal(predator);

            var contactZooCount = zoo.Animals.Count(a => a is Animal animal && animal.IsContactZoo);

            Assert.Equal(1, contactZooCount);
        }

    }
}