﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace Ecosysteme
{
    public static class Terminal    //for information that needs to be displayed regularly
    {
        public static void Separate()
        {
            Console.WriteLine("______________________________________________________________");
        }
        public static void Entities(Entities entities)
        {
            Console.WriteLine(entities.Display());
            Terminal.Separate();
        }
        public static int AskIterations()   //asks the user to input how many iterations they would like the program to complete. If the user does not type an int, (s)he is asked again until (s)he does.
        {

            Console.WriteLine("How many iterations would you like to complete ?");
            string numberOfIterations = Console.ReadLine();
            if (int.TryParse(numberOfIterations, out int iterations))
            {
                return iterations;
            }
            Terminal.WrongIteration(numberOfIterations);
            return AskIterations();
        }
        private static void WrongIteration(string input)
        {
            Console.WriteLine($"\"{input}\" is not a number. Please type an integer.");
        }
    }
    public static class Coordinates    //allows to handle coordinates, probabilities ...
    {
        //methods
        public static int Distance(int[] firstCoordinates, int[] secondCoordinates)     //calculates the distance between two points (rounded up)
        {
            return (int)Math.Ceiling(Math.Sqrt(Math.Pow(firstCoordinates[0] - secondCoordinates[0], 2) + Math.Pow(firstCoordinates[1] - secondCoordinates[1], 2)));
            //N.B.: I use Math.Ceiling to always round UP
        }
        public static int[] CloseBy(int[] coordinates, int radius, Random random)   //returns coordinates that are within the radius of the original coordinates
        {
            while(true)
            {
                int[] shift = new[] { random.Next(-radius, radius), random.Next(-radius, radius) };
                int[] newPoint = coordinates.Zip(shift, (x, y) => x + y).ToArray();
                if (Coordinates.Distance(coordinates, newPoint) <= radius)   //if the horizontal and vertical shift are both smaller than the radius, the point could still be outside the radius
                {
                    return newPoint;
                }
            }
        }
    }
    public class Entities
    {
        //attributes
        protected List<Entity> entities;
        protected ObjectIDGenerator IDGenerator;
        public Random random;
        //constructor
        public Entities()
        {
            entities = new List<Entity>(); //list of all entities in our biotope
            IDGenerator = new ObjectIDGenerator();
            random = new Random();
            //allows to assign a unique ID to each object for easy recognizing (not necessary for the code, but practical when tracking a certain entity)
        }
        //methods
        public string Display()
        {
            string text = new string("");
            foreach (Entity entity in entities)
            {
                text += string.Format("id={0}, type={1}, {2}\n", IDGenerator.GetId(entity, out entity.IsFirstTime), entity.GetType().Name, entity.ToString());
            }
            if (text.Length > 3)
            {
                return text.Remove(text.Length - 1, 1); //remove last new line
            }
            else
            {
                return "There are no entities";
            }
        }
        public void Add(Entity entity)
        {
            entities.Add(entity);
            IDGenerator.GetId(entity, out entity.IsFirstTime);
        }
        public void Remove(Entity entity)   //note: the object is removed from the object "entities", but does it still use resources / exist ?
        {
            entities.Remove(entity);
        }
        public void Iterate()
        {
            Array initialArray = entities.ToArray();
            foreach (Entity entity in initialArray)
            {
                entity.Iterate(this, initialArray);
            }
        }
        public bool NoPlant(int[] newCoordinates)  //checks if there is not yet a plant on at these coordinates
        {
            foreach (Entity entity in entities)
            {
                if (entity.getCoordinates().SequenceEqual(newCoordinates) && entity.GetType().IsSubclassOf(typeof(Plant)))
                {
                    return false;
                }
            }
            return true;
        }
    }
    //idée pour plus tard : classe Habitat qui définit la taille du plan, et dans laquelle on "place" les organismes
    public abstract class Entity   //all objects that interact (organisms, meat & organic waste)
    {
        //attributes
        protected int[] coordinates;    //position in the plane
        public bool IsFirstTime;    //needed for the ID generator (public so the generator can modify it)
        //methods
        abstract public void Iterate(Entities entities, Array initialArray); //what happens to the entity or what the entity does at each iteration
        //eventually, this has to take as input the list of all entities and will send as ouput an enumerate of actions that must be executed by the program / an updated list of entities
        public override string ToString()
        {
            return string.Format("coordinates=[{0},{1}]", coordinates[0], coordinates[1]);
        }
        abstract public Entity Reproduce(int[] coordinates);
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
        public override void Iterate(Entities entities, Array initialArray)
        {
            if (this.IterateEnergy(1))
            {
                entities.Remove(this);
            }
        }
        public bool IterateEnergy(int amount)
        {
            if (energy > amount)
            {
                this.Fatigue(amount);
                return false;
            }
            else if (life > amount-energy)
            {
                this.ConvertEnergy(amount-energy);
                this.Fatigue(amount);
                return false;
            }
            else
            {
                return true;
            }
        }
        public void Fatigue(int amount) //an organism loses energy over time ; when an animal walks or runs, it loses energy
        {
            energy -= amount;
        }
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
        //methods
        public override void Iterate(Entities entities, Array initialArray)
        {
            base.Iterate(entities, initialArray);
            //reproduction
            int[] reproduce = this.DetermineReproduction(entities);
            if (reproduce != null)
            {
                entities.Add(this.Reproduce(reproduce));
            }
            //behavior unique to plants
        }
        private int[] DetermineReproduction(Entities entities) //checks 2 things : probability and whether or not the point is free for a new plant
        {
            if (entities.random.Next(1, 101) < propagationSpeed)
            {
                int[] newCoordinates = Coordinates.CloseBy(coordinates, sowingRadius, entities.random);
                if (entities.NoPlant(newCoordinates))
                {
                    return newCoordinates;
                }
            }
            return null;
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
        public int getPropagationSpeed()
        {
            return propagationSpeed;
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
        protected bool pregnant;
        protected int pregnantTime;
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
            pregnant = false;
            pregnantTime = 0;
        }
        //methods
        public override void Iterate(Entities entities, Array initialArray)
        {
            base.Iterate(entities, initialArray);
            // behavior unique to animals
        }
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
        public void Poop(int amount)    //when an animal poops, it leaves organic waste behind (which can be consumed by plants); the amount it poops is defined by how much he ate
        {
            //informs the program to create organic waste at the animal's position
        }
        public void PregnancyIteration()
        {
            if (pregnantTime < 90)
            {
                pregnantTime++;
            }
            else
            {
                pregnant = false;
                pregnantTime = 0;
                //inform program to creat a new animal of the same type at the animal's position
            }
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
        //constructor
        public Herbivore(int[] coordinates) :
        base(coordinates)
        {

        }
        //methods
        public override void Iterate(Entities entities, Array initialArray)
        {
            base.Iterate(entities, initialArray);
            //behavior unique to herbivores
        }
    }
    abstract class Carnivore : Animal   //all carnivore species
    {
        //constructor
        public Carnivore(int[] coordinates) :
        base(coordinates)
        {

        }
        //methods
        public override void Iterate(Entities entities, Array initialArray)
        {
            base.Iterate(entities, initialArray);
            //behavior unique to carnivores
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
        //methods
        public override Entity Reproduce(int[] coordinates)
        {
            return new Deer(coordinates);
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
        //methods
        public override Entity Reproduce(int[] coordinates)
        {
            return new Wolf(coordinates);
        }
    }
    class Grass : Plant
    {
        //constructor
        public Grass(int[] coordinates) :
        base(coordinates)
        {
            rootRadius = 10;
            sowingRadius = 25;
            propagationSpeed = 3;
        }
        //methods
        public override Entity Reproduce(int[] newCoordinates)
        {
            return new Grass(newCoordinates);
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
        public override void Iterate(Entities entities, Array initialArray)
        {
            if (time<20)
            {
                this.Rot();
            }
        }
        public override Entity Reproduce(int[] coordinates)
        {
            return new Meat(coordinates, calories);
        }
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
        public override void Iterate(Entities entities, Array initialArray)
        {
            ;
        }
        public override string ToString()
        {
            return base.ToString() + string.Format(", nutrients={0}", nutrients);
        }
        public override Entity Reproduce(int[] coordinates)
        {
            return new OrganicWaste(coordinates, nutrients);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Entities entities = new Entities();
            Random rnd = new Random();
            entities.Add(new Grass(new[] { rnd.Next(-100, 100), rnd.Next(-100, 100) }));
            entities.Add(new Meat(new[] { rnd.Next(-100, 100), rnd.Next(-100, 100) }, 50));
            entities.Add(new OrganicWaste(new[] { rnd.Next(-100, 100), rnd.Next(-100, 100) }, 15));
            Terminal.Entities(entities);
            while (true)
            {
                int numberOfIterations = Terminal.AskIterations();
                for (int iteration=0; iteration < numberOfIterations; iteration++)
                {
                    entities.Iterate();
                }
                Terminal.Separate();
                Terminal.Entities(entities);
            }
        }
    }
}
