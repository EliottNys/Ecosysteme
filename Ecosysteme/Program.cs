using System;

namespace Ecosysteme
{
    //idée pour plus tard : classe Habitat qui définit la taille du plan, et dans laquelle on "place" les organismes
    abstract class Organism
    {
        //attributes
        protected int life;
        protected int energy;
        protected int[] coordinates;
        //constructor
        public Organism(int[] coordinates)
        {
            life = 100;
            energy = 100;
            this.coordinates = coordinates;
        }
        //methods
        public void ConvertEnergy(int amount)   //when an organism does not have any energy left, it converts lifepoints into energypoints
        {
            life -= amount;
            energy += amount;
        }
        //accessors
        public int getLife()
        {
            return life;
        }
        public int getEnergy()
        {
            return energy;
        }
        public int[] getCoordinates()
        {
            return coordinates;
        }
    }
    class Plant : Organism
    {
        //attributes
        private int rootRadius; //how far a plant can consume organic waste
        private int sowingRadius;   //how far new plants can appear
        //constructor
        public Plant(int[] coordinates):
        base(coordinates)
        {
            rootRadius = 10;
            sowingRadius = 10;
        }
        //accessors
        public int getRootRadius()
        {
            return rootRadius;
        }
        public int getSowingRadius()
        {
            return sowingRadius;
        }
    }
    class Animal : Organism
    {
        //attributes
        private int visionRadius;
        private int contactRadius;  //how close an animal has to be with an object to interact with it (eat, mate...)
        private int[] direction;
        private int WalkSpeed;
        private int RunSpeed;   //used in case of hunting or fleeing
        //constructor
        public Animal(int[] coordinates, int WalkSpeed, int RunSpeed):
        base(coordinates)
        {
            visionRadius = 20;
            contactRadius = 20;
            Random rnd = new Random();
            direction = new[] { rnd.Next(-1,1), rnd.Next(-1,1)}; //random cardinal direction (examples: (-1, 1)=NW ; (1,0)=E ; (1,-1)=SE)
            while (direction[0]==0&&direction[1]==0)    //(0,0) is not a direction, so we generate a new one
            {
                direction = new[] { rnd.Next(-1,1), rnd.Next(-1,1)};
            }
            this.WalkSpeed = WalkSpeed;
            this.RunSpeed = RunSpeed;
        }
        //methods
        public void Walk()  //moves the animal in the habitat (distance=f(speed))
        {
            coordinates[0] += direction[0] * WalkSpeed;
            coordinates[1] += direction[1] * WalkSpeed;
        }
        public void Run()
        {
            coordinates[0] += direction[0] * RunSpeed;
            coordinates[1] += direction[1] * RunSpeed;
        }
        public void ChangeDirection(int[] direction)
        {
            this.direction = direction;
        }
        public void fatigue(int amount) //when an animal walks or runs, it loses energy
        {
            energy -= amount;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Début");
            Random rnd = new Random();
            Animal Vache = new Animal(new[] { rnd.Next(-100, 100), rnd.Next(-100, 100) }, 10,25);
            /*
            Console.WriteLine(string.Join(", ", Vache.getCoordinates()));
            Vache.Walk();
            Console.WriteLine(string.Join(", ", Vache.getCoordinates()));
            Vache.Run();
            Console.WriteLine(string.Join(", ", Vache.getCoordinates()));
            */
            /*
            Console.WriteLine(string.Join(", ", Vache.getLife(), Vache.getEnergy()));
            Vache.ConvertEnergy(5);
            Console.WriteLine(string.Join(", ", Vache.getLife(), Vache.getEnergy()));
            */
            Console.WriteLine("Fin");
        }
    }
}
