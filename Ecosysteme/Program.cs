﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ecosysteme
{
    public static class Coordinates    //allows to handle coordinates
    {
        public static int Distance(int[] firstCoordinates, int[] secondCoordinates)
        {
            return (int)Math.Ceiling(Math.Sqrt(Math.Pow(firstCoordinates[0] - secondCoordinates[0],2) + Math.Pow(firstCoordinates[0] - secondCoordinates[0],2)));
            //N.B.: I use Math.Ceiling to always round UP
        }
    }
    //idée pour plus tard : classe Habitat qui définit la taille du plan, et dans laquelle on "place" les organismes
    abstract class Entity   //all objects that interact (organisms, meat & organic waste)
    {
        //attributes
        protected int[] coordinates;
        public bool IsFirstTime;
        //methods
        public override string ToString()
        {
            return base.ToString() + string.Format(": coordinates=[{0},{1}]", coordinates[0], coordinates[1]);
        }
        //accessors
        public int[] getCoordinates()
        {
            return coordinates;
        }
    }
    abstract class Organism : Entity    //everything that is alive
    {
        //attributes
        protected int life;
        protected int energy;
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
        public override string ToString()
        {
            return base.ToString() + string.Format(", life={0}, energy={1}", life, energy);
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
    }
    abstract class Plant : Organism //all plant species
    {
        //attributes
        protected int rootRadius; //how far a plant can consume organic waste
        protected int sowingRadius;   //how far new plants can appear
        protected int propagationSpeed; //how frequently a plant sows
        //constructor
        public Plant(int[] coordinates):
        base(coordinates)
        {
            
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
    abstract class Animal : Organism    //herbivores and carnivores
    {
        //attributes
        protected int sex;    //0=male, 1=female
        protected int visionRadius;
        protected int contactRadius;  //how close an animal has to be with an object to interact with it (eat, mate...)
        protected int[] direction;
        protected int walkSpeed;
        protected int runSpeed;   //used in case of hunting or fleeing
        //constructor
        public Animal(int[] coordinates) :
        base(coordinates)
        {
            Random rnd = new Random();
            sex = rnd.Next(1);
            direction = new[] { rnd.Next(-1, 1), rnd.Next(-1, 1) }; //random cardinal direction (examples: (-1, 1)=NW ; (1,0)=E ; (1,-1)=SE)
            while (direction[0] == 0 && direction[1] == 0)    //(0,0) is not a direction, so we generate a new one
            {
                direction = new[] { rnd.Next(-1, 1), rnd.Next(-1, 1) };
            }
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
    abstract class Herbivore : Animal   //all herbivore species
    {
        public Herbivore(int[] coordinates) :
        base(coordinates)
        {

        }
    }
    abstract class Carnivore : Animal   //all carnivore species
    {
        public Carnivore(int[] coordinates) :
        base(coordinates)
        {

        }
    }
    class Deer : Herbivore
    {
        public Deer(int[] coordinates) :
        base(coordinates)
        {
            visionRadius = 30;
            contactRadius = 5;
            walkSpeed = 10;
            runSpeed = 25;
        }
    }
    class Wolf : Carnivore
    {
        public Wolf(int[] coordinates) :
        base(coordinates)
        {
            visionRadius = 50;
            contactRadius = 5;
            walkSpeed = 10;
            runSpeed = 40;
        }
    }
    class Grass : Plant
    {
        //constructor
        public Grass(int[] coordinates) :
        base(coordinates)
        {
            rootRadius = 10;
            sowingRadius = 30;
            propagationSpeed = 20;
        }
    }
    class Meat : Entity  //created when an animal dies
    {
        //atributes
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
        public override string ToString()
        {
            return base.ToString() + string.Format(", time={0}, calories={1}", time, calories);
        }
        //accessors
        public int getTime()
        {
            return time;
        }
        public int getCalories()
        {
            return calories;
        }
    }
    class OrganicWaste : Entity  //created when a plant dies, meat rots or an animal poops
    {
        //atributes
        private int nutrients;   //equivalent to "calories" attribute for Meat class; defines how much energy it would provide to a plant
        //constructor
        public OrganicWaste(int[] coordinates, int nutrients)
        {
            this.coordinates=coordinates;
            this.nutrients = nutrients;
        }
        //methods
        public override string ToString()
        {
            return base.ToString() + string.Format(", nutrients={0}", nutrients);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Début");
            List<Entity> Entities = new List<Entity>(); //list of all entities in our biotope
            ObjectIDGenerator IDGenerator = new ObjectIDGenerator();    //allows to assign a unique ID to each object for easy recognizing
            Random rnd = new Random();
            Entities.Add(new Grass(new[] { rnd.Next(-100, 100), rnd.Next(-100, 100) }));
            Entities.Add(new OrganicWaste(new[] { rnd.Next(-100, 100), rnd.Next(-100, 100) }, 15));
            foreach (Entity entity in Entities)
            {
                Console.WriteLine(entity.ToString());
                Console.WriteLine(IDGenerator.GetId(entity, out entity.IsFirstTime));
            }
            while(true)
            {
                Console.WriteLine("How many iterations would you like to complete ?");
                string numberOfIterations = Console.ReadLine();
                if (int.TryParse(numberOfIterations, out int iterations))
                {
                    int iteration = 0;
                    while (iteration < iterations)
                    {
                        iteration++;
                    }
                }
                else
                {
                    Console.WriteLine($"\"{numberOfIterations}\" is not a number. Please type an integer.");
                }
            }
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
            /*
            int[] co1 = new int[] { 10, 10 };
            int[] co2 = new int[] { 20, 20 };
            Console.WriteLine(Coordinates.Distance(co1, co2));
            */
            Console.WriteLine("Fin");
        }
    }
}
