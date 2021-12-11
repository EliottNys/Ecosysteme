﻿using System;

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
        private int sex;    //0=male, 1=female
        private int visionRadius;
        private int contactRadius;  //how close an animal has to be with an object to interact with it (eat, mate...)
        private int[] direction;
        private int walkSpeed;
        private int runSpeed;   //used in case of hunting or fleeing
        //constructor
        public Animal(int[] coordinates, int walkSpeed, int runSpeed) :
        base(coordinates)
        {
            Random rnd = new Random();
            sex = rnd.Next(1);
            visionRadius = 20;
            contactRadius = 20;
            direction = new[] { rnd.Next(-1, 1), rnd.Next(-1, 1) }; //random cardinal direction (examples: (-1, 1)=NW ; (1,0)=E ; (1,-1)=SE)
            while (direction[0] == 0 && direction[1] == 0)    //(0,0) is not a direction, so we generate a new one
            {
                direction = new[] { rnd.Next(-1, 1), rnd.Next(-1, 1) };
            }
            this.walkSpeed = walkSpeed;
            this.runSpeed = runSpeed;
        }
        //methods
        public void Walk()  //moves the animal in the habitat (distance=f(speed))
        {
            coordinates[0] += direction[0] * walkSpeed;
            coordinates[1] += direction[1] * walkSpeed;
        }
        public void Run()
        {
            coordinates[0] += direction[0] * runSpeed;
            coordinates[1] += direction[1] * runSpeed;
        }
        public void ChangeDirection(int[] direction)
        {
            this.direction = direction;
        }
        public void Fatigue(int amount) //when an animal walks or runs, it loses energy
        {
            energy -= amount;
        }
        public void Poop(int amount)    //when an animal poops, it leaves organic waste behind (which can be consumed by plants); the amount it poops is defined by how much he ate
        {
            new OrganicWaste(this.coordinates, amount);
        }
        //accessors
        public int getSex()
        {
            return sex;
        }
        public int getVisionRadius()
        {
            return visionRadius;
        }
        public int getContactRadius()
        {
            return contactRadius;
        }
        public int[] getDirection()
        {
            return direction;
        }
        public int getWalkSpeed()
        {
            return walkSpeed;
        }
        public int getRunSpeed()
        {
            return runSpeed;
        }
    }
    class Meat  //created when an animal dies
    {
        //atributes
        private int[] coordinates;
        private int time;   //after some time, the meat rots and becomes organic waste
        private int calories;   //defines how much energy it would provide to a carnivore or how much organic waste it would produce
        //constructor
        public Meat(int[] coordinates, int calories)
        {
            this.coordinates = coordinates;
            this.calories = calories;
            time = 0;
        }
        //methods
        public void Rot()
        {
            time += 1;
        }
        //accessors
        public int[] getCoordinates()
        {
            return coordinates;
        }
        public int getTime()
        {
            return time;
        }
        public int getCalories()
        {
            return calories;
        }
    }
    class OrganicWaste  //created when a plant dies, meat rots or an animal poops
    {
        //atributes
        private int[] coordinates;
        private int nutrients;   //equivalent to "calories" attribute for Meat class; defines how much energy it would provide to a plant
        //constructor
        public OrganicWaste(int[] coordinates, int nutrients)
        {
            this.coordinates=coordinates;
            this.nutrients = nutrients;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Début");
            Random rnd = new Random();
            //Animal Vache = new Animal(new[] { rnd.Next(-100, 100), rnd.Next(-100, 100) }, 10,25);
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
