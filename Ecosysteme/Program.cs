using System;

namespace Ecosysteme
{
    abstract class Organism
    {
        protected int life;
        protected int energy;
        protected int[] coordinates;
        public Organism()
        {
            this.life = 100;
            this.energy = 100;
            this.coordinates = new int[] {10,10};
        }
        public void ConvertEnergy()
        {
            this.life--;
            this.energy++;
        }
        public int getLife()
        {
            return this.life;
        }
        public int getEnergy()
        {
            return this.energy;
        }
        public int[] getCoordinates()
        {
            return this.coordinates;
        }
    }
    class Plant : Organism
    {
        private int rootRadius;
        private int sowingRadius;
        public Plant():
        base()
        {
            this.rootRadius = 10;
            this.sowingRadius = 10;
        }
        public int getRootRadius()
        {
            return this.rootRadius;
        }
        public int getSowingRadius()
        {
            return this.sowingRadius;
        }
    }
    class Animal : Organism
    {
        private int visionRadius;
        private int contactRadius;
        private int[] direction;
        private int speed;
        public Animal(int speed):
        base()
        {
            this.visionRadius = 20;
            this.contactRadius = 20;
            Random rnd = new Random();
            this.direction = new[] { rnd.Next(-1,1), rnd.Next(-1,1)};
            while (direction[0]==0&&direction[1]==0) //test 2
            {
                this.direction = new[] { rnd.Next(-1,1), rnd.Next(-1,1)};//test 3
            }
            this.speed = speed;
        }
        public void Move()
        {
            this.coordinates[0] += direction[0] * speed;
            this.coordinates[1] += direction[1] * speed;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Début");
            Animal Vache = new Animal(15);
            Console.WriteLine(string.Join(", ", Vache.getCoordinates()));
            Vache.Move();
            Console.WriteLine(string.Join(", ", Vache.getCoordinates()));
            Console.WriteLine("Fin");
        }
    }
}
