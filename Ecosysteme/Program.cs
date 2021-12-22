﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace Ecosysteme
{
    public static class Terminal    //for information that needs to be displayed regularly
    {
        public static void Separate() { Console.WriteLine("______________________________________________________________"); }  //separation for more clarity in the console
        public static void Entities(Entities entities)  //Displays all the entities with their properties (ID, class, etc.)
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
        private static void WrongIteration(string input) { Console.WriteLine($"\"{input}\" is not a number. Please type an integer."); }    //message displayed if the input is invalid
    }
    public static class Coordinates    //allows to handle coordinates, probabilities ...
    {
        //METHODS
        public static int Distance(int[] firstCoordinates, int[] secondCoordinates)     //calculates the distance between two points (rounded up)
        {
            return (int)Math.Ceiling(Math.Sqrt(Math.Pow(firstCoordinates[0] - secondCoordinates[0], 2) + Math.Pow(firstCoordinates[1] - secondCoordinates[1], 2)));
            //N.B.: I use Math.Ceiling to always round UP
        }
        public static int[] CloseBy(int[] coordinates, int radius, Random random)   //returns coordinates that are within the radius of the original coordinates
        {
            while (true)
            {
                int[] shift = new[] { random.Next(-radius, radius), random.Next(-radius, radius) };
                int[] newPoint = coordinates.Zip(shift, (x, y) => x + y).ToArray();
                if (Coordinates.Distance(coordinates, newPoint) <= radius)   //if the horizontal and vertical shift are both smaller than the radius, the point could still be outside the radius
                {
                    return newPoint;
                }
            }
        }
        public static int[] Direction(int[] departure, int[] destination)   //gives the direction you need to take to go from departure-point to destination-point
        {
            int[] vector = destination.Zip(departure, (x, y) => x - y).ToArray();
            int sinus = (int)Math.Round((double)(1000 * (vector[1] / vector[0])));
            if (vector[0] > 0 && vector[1] > 0) //1st quadrant
            {
                if (sinus > 924) { return new int[] { 0, 1 }; } //N
                else if (sinus < 383) { return new int[] { 1, 0 }; }    //E
                else { return new int[] { 1, 1 }; } //NE
            }
            else if (vector[0] > 0 && vector[1] < 0) //2nd quadrant
            {
                if (sinus > -383) { return new int[] { 1, 0 }; }    //E
                else if (sinus < -924) { return new int[] { 0, -1 }; }  //S
                else { return new int[] { 1, -1 }; }    //SE
            }
            else if (vector[0] < 0 && vector[1] < 0) //3rd quadrant
            {
                if (sinus < -924) { return new int[] { 0, -1 }; }  //S
                else if (sinus > -383) { return new int[] { -1, 0 }; } //W
                else { return new int[] { -1, -1 }; }   //SW
            }
            else  //4th quadrant
            {
                if (sinus < 383) { return new int[] { -1, 0 }; }    //W
                else if (sinus > 924) { return new int[] { 0, 1 }; }    //N
                else { return new int[] { -1, 1 }; }    //NW
            }
        }
    }
    public class Entities
    {
        //ATTRIBUTES
        protected List<Entity> entities;
        protected ObjectIDGenerator IDGenerator;
        public Random random;
        //CONSTRUCTOR
        public Entities()
        {
            entities = new List<Entity>(); //list of all entities in our biotope
            IDGenerator = new ObjectIDGenerator();
            random = new Random();
            //allows to assign a unique ID to each object for easy recognizing (not necessary for the code, but practical when tracking a certain entity)
        }
        //METHODS
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
        public void Remove(Entity entity) { entities.Remove(entity); }   //note: the object is removed from the object "entities", but does it still use resources / exist ?
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
        public bool Chance(int chance) { return random.Next(1, chance) == 1; }
        //ACCESSORS
        public List<Entity> getList() { return entities; }
    }
    //idée pour plus tard : classe Habitat qui définit la taille du plan, et dans laquelle on "place" les organismes
    public abstract class Entity   //all objects that interact (organisms, meat & organic waste)
    {
        //ATTRIBUTES
        protected int[] coordinates;    //position in the plane
        public bool IsFirstTime;    //needed for the ID generator (public so the generator can modify it)
        //METHODS
        abstract public void Iterate(Entities entities, Array initialArray); //what happens to the entity or what the entity does at each iteration
        //eventually, this has to take as input the list of all entities and will send as ouput an enumerate of actions that must be executed by the program / an updated list of entities
        public override string ToString() { return string.Format("coordinates=[{0},{1}]", coordinates[0], coordinates[1]); }
        abstract public void Transform(Entities entities);
        //ACCESSORS
        public int[] getCoordinates() { return coordinates; }
    }
    abstract class Organism : Entity    //everything that is alive
    {
        //ATTRIBUTES
        protected int life;
        protected int energy;
        //CONSTRUCTOR
        public Organism(int[] coordinates)
        {
            life = 100;
            energy = 100;
            this.coordinates = coordinates;
        }
        //METHODS
        public override void Iterate(Entities entities, Array initialArray)
        {
            if (this.IterateEnergyAndLife(1))
            {
                this.Transform(entities);
            }
        }
        public bool IterateEnergyAndLife(int amount)
        {
            if (energy > amount)
            {
                this.Fatigue(amount);
                if (life < 100 && energy >= 80) //if an organism has over 80% energy, his health regenerates over time
                {
                    life += amount;
                }
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
        public override void Transform(Entities entities)
        {
            if (this.GetType().IsSubclassOf(typeof(Plant))) //organism is plant
            {
                entities.Add(new OrganicWaste(coordinates, 20));    //turn into organic waste
            }
            else  //organism is animal
            {
                entities.Add(new Meat(coordinates, 20));
            }
            entities.Remove(this);
        }
        public void Fatigue(int amount) { energy -= amount; }   //an organism loses energy over time ; when an animal walks or runs, it loses energy
        public void ConvertEnergy(int amount)   //when an organism does not have any energy left, it converts lifepoints into energypoints
        {
            life -= amount;
            energy += amount;
        }
        abstract protected Organism Reproduce(int[] coordinates);
        public override string ToString() { return base.ToString() + string.Format(", life={0}, energy={1}", life, energy); }
        //ACCESSORS
        public int getLife() { return life; }
        public int getEnergy() { return energy; }
    }
    abstract class Plant : Organism //all plant species
    {
        //ATTRIBUTES
        protected int rootRadius; //how far a plant can consume organic waste
        protected int sowingRadius;   //how far new plants can appear
        protected int propagationSpeed; //how frequently a plant sows
        //CONSTRUCTOR
        public Plant(int[] coordinates):
        base(coordinates)
        {
            ;
        }
        //METHODS
        public override void Iterate(Entities entities, Array initialArray)
        {
            //losing/regenerating of energy/life
            base.Iterate(entities, initialArray);
            //feeding
            OrganicWaste food = this.FindOrganicWaste(entities);
            this.Consume(food, entities);
            //reproduction
            int[] reproduce = this.DetermineReproduction(entities);
            if (reproduce != null)
            {
                entities.Add(this.Reproduce(reproduce));
            }
            //behavior unique to plants
        }
        private OrganicWaste FindOrganicWaste(Entities entities)
        {
            OrganicWaste response = null;
            int distance = 10000;
            foreach (Entity entity in entities.getList())
            {
                if (entity.GetType() == typeof(OrganicWaste) && Coordinates.Distance(coordinates, entity.getCoordinates()) < rootRadius && Coordinates.Distance(coordinates, entity.getCoordinates()) < distance)
                {
                    response = (OrganicWaste)entity;
                    distance = Coordinates.Distance(coordinates,entity.getCoordinates());
                }
            }
            return response;
        }
        private void Consume(OrganicWaste food, Entities entities)
        {
            if (food != null)
            {
                int emptyEnergy = 100 - energy;
                int nutrients = food.getNutrients();
                while (emptyEnergy > 0 && nutrients > 0)
                {
                    emptyEnergy--;
                    energy++;
                    nutrients--;
                }
                if (nutrients == 0)
                {
                    entities.Remove(food);
                }
                else
                {
                    food.Leave(nutrients);
                }
            }
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
        //ACCESSORS
        public int getRootRadius() { return rootRadius; }
        public int getSowingRadius() { return sowingRadius; }
        public int getPropagationSpeed() { return propagationSpeed; }
    }
    abstract class Animal : Organism    //herbivores and carnivores
    {
        //ATTRIBUTES
        protected int sex;    //0=male, 1=female
        protected int visionRadius;
        protected int contactRadius;  //how close an animal has to be with an object to interact with it (eat, mate...)
        protected int[] direction;
        protected int walkSpeed;
        protected int runSpeed;   //used in case of hunting or fleeing
        protected bool pregnant;
        protected int pregnantTime;
        protected int gestationPeriod;
        //CONSTRUCTOR
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
        //METHODS
        protected void RandomDirection(Entities entities)
        {
            direction = new[] { entities.random.Next(-1, 1), entities.random.Next(-1, 1) }; //random cardinal direction (examples: (-1, 1)=NW ; (1,0)=E ; (1,-1)=SE)
            while (direction[0] == 0 && direction[1] == 0)    //(0,0) is not a direction, so we generate a new one
            {
                direction = new[] { entities.random.Next(-1, 1), entities.random.Next(-1, 1) };
            }
        }
        public override void Iterate(Entities entities, Array initialArray)
        {
            //losing/regenerating of energy/life
            base.Iterate(entities, initialArray);
            //pooping
            if (entities.Chance(3)) { this.Poop(entities); }
            //pregnancy
            if (pregnant)
            {
                if (pregnantTime == gestationPeriod)
                {
                    pregnantTime = 0;
                    entities.Add(this.Reproduce(coordinates));
                }
                else { pregnantTime++; }
            }
            //actions (feeding / mating)
            Entity food = FindFood(entities);
            if ((food == null || energy > 80 || (energy > 10 && life > 50)) && !pregnant)   //food is not a priority / there is no food
            {
                Animal mate = FindMate(entities);
                if (mate == null)   //there is nothing of interest (no food or mate)
                {
                    if (entities.Chance(10)) { this.RandomDirection(entities); }    //animal changes direction regularly so it doesn't wander off too far away from the other entities
                    this.Walk();
                }
                else if (Coordinates.Distance(coordinates, mate.getCoordinates()) <= contactRadius)
                {
                    this.Mate();
                    mate.Mate();
                }
                else
                {
                    this.ChangeDirection(Coordinates.Direction(coordinates, mate.getCoordinates()));
                    this.Walk();
                }
            }
            else    //food is a priority / already pregnant
            {
                //eat / go to food
            }
        }
        public void Move(int speed)  //moves the animal in the habitat (distance=f(speed))
        {
            if (direction.Sum() == 1 || direction.Sum() == -1)
            {
                coordinates[0] += direction[0] * speed;
                coordinates[1] += direction[1] * speed;
            }
            else
            {
                coordinates[0] -= (int)Math.Ceiling(direction[0] * speed * 0.7);
                coordinates[1] -= (int)Math.Ceiling(direction[1] * speed * 0.7);
            }
        }
        public void Walk() { this.Move(walkSpeed); }
        public void Run() { this.Move(runSpeed); }
        public void ChangeDirection(int[] direction) { this.direction = direction; }
        public void Poop(Entities entities) { entities.Add(new OrganicWaste(coordinates, 20)); }    //when an animal poops, it leaves organic waste behind (which can be consumed by plants)
        protected Animal FindMate(Entities entities)   //finds the closest eligible mate within the vision radius
        {
            Animal response = null;
            int distance = 10000;
            foreach (Entity entity in entities.getList())
            {
                if (entity.GetType() == this.GetType() && Coordinates.Distance(coordinates, entity.getCoordinates()) < visionRadius && Coordinates.Distance(coordinates, entity.getCoordinates()) < distance)
                {
                    if (sex != ((Animal)entity).getSex() && !((Animal)entity).getPregnant())  //not same sex and not yet pregnant
                    {
                        response = (Animal)entity;
                        distance = Coordinates.Distance(coordinates, entity.getCoordinates());
                    }
                }
            }
            return response;
        }
        protected void Mate()
        {
            if (this.getSex() == 1) { pregnant = true; }    //female -> pregnancy starts
        }
        protected abstract Entity FindFood(Entities entities);
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
        //ACCESSORS
        public int getSex() { return sex; }
        public int getVisionRadius() { return visionRadius; }
        public int getContactRadius() { return contactRadius; }
        public int[] getDirection() { return direction; }
        public int getWalkSpeed() { return walkSpeed; }
        public int getRunSpeed() { return runSpeed; }
        public bool getPregnant() { return pregnant; }
    }
    abstract class Herbivore : Animal   //all herbivore species
    {
        //CONSTRUCTOR
        public Herbivore(int[] coordinates) :
        base(coordinates)
        {

        }
        //METHODS
        public override void Iterate(Entities entities, Array initialArray)
        {
            base.Iterate(entities, initialArray);
        }
        protected override Entity FindFood(Entities entities)
        {
            Entity response = null;
            int distance = 10000;
            foreach (Entity entity in entities.getList())
            {
                if (entity.GetType() == typeof(Plant) && Coordinates.Distance(coordinates, entity.getCoordinates()) < visionRadius && Coordinates.Distance(coordinates, entity.getCoordinates()) < distance)
                {
                    response = entity;
                    distance = Coordinates.Distance(coordinates, entity.getCoordinates());
                }
            }
            return response;
        }
    }
    abstract class Carnivore : Animal   //all carnivore species
    {
        //CONSTRUCTOR
        public Carnivore(int[] coordinates) :
        base(coordinates)
        {

        }
        //METHODS
        public override void Iterate(Entities entities, Array initialArray)
        {
            base.Iterate(entities, initialArray);
            //behavior unique to carnivores
        }
        protected override Entity FindFood(Entities entities)
        {
            Entity response = null;
            int distance = 10000;
            foreach (Entity entity in entities.getList())
            {
                if (entity.GetType() == typeof(Herbivore) && Coordinates.Distance(coordinates, entity.getCoordinates()) < visionRadius && Coordinates.Distance(coordinates, entity.getCoordinates()) < distance)
                {
                    response = entity;
                    distance = Coordinates.Distance(coordinates, entity.getCoordinates());
                }
            }
            return response;
        }
    }
    class Deer : Herbivore
    {
        //CONSTRUCTOR
        public Deer(int[] coordinates) :
        base(coordinates)
        {
            visionRadius = 30;
            contactRadius = 5;
            walkSpeed = 10;
            runSpeed = 25;
            gestationPeriod = 50;
        }
        //METHODS
        protected override Organism Reproduce(int[] coordinates) { return new Deer(coordinates); }
    }
    class Wolf : Carnivore
    {
        //CONSTRUCTOR
        public Wolf(int[] coordinates) :
        base(coordinates)
        {
            visionRadius = 50;
            contactRadius = 5;
            walkSpeed = 10;
            runSpeed = 40;
            gestationPeriod = 90;
        }
        //METHODS
        protected override Organism Reproduce(int[] coordinates) { return new Wolf(coordinates); }
    }
    class Grass : Plant
    {
        //CONSTRUCTOR
        public Grass(int[] coordinates) :
        base(coordinates)
        {
            rootRadius = 100;
            sowingRadius = 25;
            propagationSpeed = 3;
        }
        //METHODS
        protected override Organism Reproduce(int[] newCoordinates) { return new Grass(newCoordinates); }
    }
    class Meat : Entity  //created when an animal dies
    {
        //ATTRIBUTES
        private int time;   //after some time, the meat rots and becomes organic waste
        private int calories;   //defines how much energy it would provide to a carnivore or how much organic waste it would produce
        //CONSTRUCTOR
        public Meat(int[] coordinates, int calories)
        {
            this.coordinates = coordinates;
            this.calories = calories;
            time = 0;
        }
        //METHODS
        public override void Iterate(Entities entities, Array initialArray)
        {
            if (time < 20) { this.Rot(); }
            else { this.Transform(entities); }
        }
        public void Rot() { time += 1; }
        public override string ToString() { return base.ToString() + string.Format(", time={0}, calories={1}", time, calories); }
        public override void Transform(Entities entities)
        {
            entities.Add(new OrganicWaste(coordinates, 20));    //turn into organic waste
            entities.Remove(this);
        }
        //ACCESSORS
        public int getTime() { return time; }
        public int getCalories() { return calories; }
    }
    class OrganicWaste : Entity  //created when a plant dies, meat rots or an animal poops
    {
        //ATTRIBUTES
        private int nutrients;   //equivalent to "calories" attribute for Meat class; defines how much energy it would provide to a plant
        //CONSTRUCTOR
        public OrganicWaste(int[] coordinates, int nutrients)
        {
            this.coordinates=coordinates;
            this.nutrients = nutrients;
        }
        //METHODS
        public override void Iterate(Entities entities, Array initialArray) { }
        public override string ToString() { return base.ToString() + string.Format(", nutrients={0}", nutrients); }
        public void Leave(int leftover) { nutrients = leftover; }   //if only a part is consumed
        public override void Transform(Entities entities) { }   //Organic waste does not transform
        //ACCESSORS
        public int getNutrients() { return nutrients; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Entities entities = new Entities();
            Random rnd = new Random();
            //entities.Add(new Grass(new[] { rnd.Next(-100, 100), rnd.Next(-100, 100) }));
            //entities.Add(new Meat(new[] { rnd.Next(-100, 100), rnd.Next(-100, 100) }, 50));
            //entities.Add(new OrganicWaste(new[] { rnd.Next(-100, 100), rnd.Next(-100, 100) }, 15));
            //entities.Add(new Deer(new[] { rnd.Next(-100, 100), rnd.Next(-100, 100) }));
            //entities.Add(new Wolf(new[] { rnd.Next(-100, 100), rnd.Next(-100, 100) }));
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
