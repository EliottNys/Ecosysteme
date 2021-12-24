using System;
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
            int[] vector = new int[] { destination[0] - departure[0], destination[1] - departure[1] };
            if (vector[0] == 0)
            {
                if (vector[1] > 0) { return new int[] { 0, 1 }; }
                else { return new int[] { 0, -1 };}
            }
            if (vector[1] == 0)
            {
                if (vector[0] > 0) { return new int[] { 1, 0 }; }
                else { return new int[] { -1, 0 }; }
            }
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
        public static List<int[]> Area(int[] coordinates, int radius)   //returns all coordinates within range
        {
            List<int[]> answer = new List<int[]>();
            for (int i = coordinates[0] - radius; i <= coordinates[0] + radius; i++)
            {
                for (int j = coordinates[1] - radius; j <= coordinates[1] + radius; j++)
                {
                    if (Coordinates.Distance(new int[] { i, j }, coordinates) <= radius) { answer.Add(new int[] { i, j }); }    //square => (pseudo-)circle
                }
            }
            return answer;
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
                entity.Iterate(this);
            }
        }
        public bool NoPlant(int[] newCoordinates)  //checks if there is not yet a plant on at these coordinates
        {
            foreach (Entity entity in entities)
            {
                if (entity.GetType().IsSubclassOf(typeof(Plant)))
                {
                    Plant plant = (Plant)entity;
                    if (plant.getOccupiedArea().Contains(newCoordinates)) { return false; }
                }
                /*
                if (entity.getCoordinates().SequenceEqual(newCoordinates) && entity.GetType().IsSubclassOf(typeof(Plant)))
                {
                    return false;
                }
                */
            }
            return true;
        }
        public bool Chance(int chance) { return random.Next(0, chance) == 0; }
        public void OneOfEach(int times)
        {
            for (int i = 0; i < times; i++)
            {
                this.Add(new Grass(new int[] {random.Next(-100,101), random.Next(-100, 101)}));
                this.Add(new Bush(new int[] { random.Next(-100, 101), random.Next(-100, 101) }));
                this.Add(new Deer(new int[] { random.Next(-100, 101), random.Next(-100, 101) }));
                this.Add(new Rabbit(new int[] { random.Next(-100, 101), random.Next(-100, 101) }));
                this.Add(new Wolf(new int[] { random.Next(-100, 101), random.Next(-100, 101) }));
                this.Add(new Fox(new int[] { random.Next(-100, 101), random.Next(-100, 101) }));
            }
        }
        public void MatingTest()
        {
            this.Add(new Deer(new int[] { -5, 0 }));
            this.Add(new Deer(new int[] { 5, 0 }));
        }
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
        abstract public void Iterate(Entities entities); //what happens to the entity or what the entity does at each iteration
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
        public override void Iterate(Entities entities)
        {
            if (this.IterateEnergyAndLife(1))
            {
                this.Transform(entities);
            }
        }
        private bool IterateEnergyAndLife(int amount)
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
        private void Fatigue(int amount) { energy -= amount; }   //an organism loses energy over time ; when an animal walks or runs, it loses energy
        private void ConvertEnergy(int amount)   //when an organism does not have any energy left, it converts lifepoints into energypoints
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
        protected List<int[]> occupiedArea; //area in which no other plant can grow,
        protected int propagationSpeed; //how frequently a plant sows (%)
        protected int calorieDensity; //! calories per life point (==> calories = calorieDensity * life)
        //CONSTRUCTOR
        public Plant(int[] coordinates):
        base(coordinates)
        {
            this.occupiedArea = Coordinates.Area(coordinates, 1);   //if you want to override this in the species class: 1 occupies 5 points, 2 occupies 13 points, 3 occupies 29 points ...
        }
        //METHODS
        public override void Iterate(Entities entities)
        {
            //losing/regenerating of energy/life
            base.Iterate(entities);
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
        public void Leave(int amount) { life = amount; }
        //ACCESSORS
        public int getRootRadius() { return rootRadius; }
        public int getSowingRadius() { return sowingRadius; }
        public int getPropagationSpeed() { return propagationSpeed; }
        public int getCalorieDensity() { return calorieDensity; }
        public List<int[]> getOccupiedArea() { return occupiedArea; }
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
            sex = rnd.Next(2);
            direction = new[] { rnd.Next(-1, 2), rnd.Next(-1, 2) }; //random cardinal direction (examples: (-1, 1)=NW ; (1,0)=E ; (1,-1)=SE)
            while (direction[0] == 0 && direction[1] == 0)    //(0,0) is not a direction, so we generate a new one
            {
                direction[0] = rnd.Next(-1, 2);
                direction[1] = rnd.Next(-1, 2);
            }
            pregnant = false;
            pregnantTime = 0;
        }
        //METHODS
        private void RandomDirection(Entities entities)
        {
            direction = new[] { entities.random.Next(-1, 2), entities.random.Next(-1, 2) }; //random cardinal direction (examples: (-1, 1)=NW ; (1,0)=E ; (1,-1)=SE)
            while (direction[0] == 0 && direction[1] == 0)    //(0,0) is not a direction, so we generate a new one
            {
                direction[0] = entities.random.Next(-1, 2);
                direction[1] = entities.random.Next(-1, 2);
            }
        }
        public override void Iterate(Entities entities)
        {
            //losing/regenerating of energy/life
            base.Iterate(entities);
            //pooping
            if (entities.Chance(10)) { this.Poop(entities); }
            //pregnancy
            if (pregnant) { this.PregnancyIteration(entities); }
            //actions (feeding / mating)
            this.Action(entities);
        }
        private void Move(int speed)  //moves the animal in the habitat (distance=f(speed))
        {
            if (direction.Sum() == 1 || direction.Sum() == -1)
            {
                coordinates[0] += direction[0] * speed;
                coordinates[1] += direction[1] * speed;
            }
            else
            {
                coordinates[0] += (int)Math.Round(direction[0] * speed * 0.7);
                coordinates[1] += (int)Math.Ceiling(direction[1] * speed * 0.7);
            }
        }
        public void Walk() { this.Move(walkSpeed); }
        public void Run() { this.Move(runSpeed); }
        private void ChangeDirection(int[] direction) { this.direction = direction; }
        private void Poop(Entities entities) { entities.Add(new OrganicWaste(coordinates, 20)); }    //when an animal poops, it leaves organic waste behind (which can be consumed by plants)
        private Animal FindMate(Entities entities)   //finds the closest eligible mate within the vision radius
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
        private void Mate() { if (this.getSex() == 1) { pregnant = true; } }  //female -> pregnancy starts
        protected abstract Entity FindFood(Entities entities);
        private void Action(Entities entities)
        {
            Entity food = FindFood(entities);
            if ((food == null || energy > 80 || (energy > 10 && life > 50)) && !pregnant)   //food is not a priority / there is no food
            {
                this.NotFoodIteration(entities, food);
            }
            else    //food is a priority / already pregnant
            {
                this.FoodIteration(entities, food);
            }
        }
        private void PregnancyIteration(Entities entities)
        {
            if (pregnantTime == gestationPeriod)
            {
                pregnant = false;
                pregnantTime = 0;
                entities.Add(this.Reproduce(coordinates));
            }
            else { pregnantTime++; }
        }
        private void NotFoodIteration(Entities entities, Entity food)
        {
            Animal mate = FindMate(entities);
            if (mate == null)   //no mate in sight
            {
                if (food != null) { this.FoodIteration(entities, food); }   //food is not a priority, but there is nothing else to do
                else  //there is nothing of interest (no food or mate)
                {
                    if (entities.Chance(10)) { this.RandomDirection(entities); }    //animal changes direction regularly so it doesn't wander off too far away from the other entities
                    this.Walk();
                }
            }
            else if (Coordinates.Distance(coordinates, mate.getCoordinates()) <= contactRadius) //mate
            {
                this.Mate();
                mate.Mate();
            }
            else  //go towards mate
            {
                this.ChangeDirection(Coordinates.Direction(coordinates, mate.getCoordinates()));
                this.Walk();
            }
        }
        private void FoodIteration(Entities entities, Entity food)
        {
            if (food == null)
            {
                if (entities.Chance(10)) { this.RandomDirection(entities); }    //animal changes direction regularly so it doesn't wander off too far away from the other entities
                this.Walk();
            }
            else if (Coordinates.Distance(coordinates, food.getCoordinates()) > contactRadius)   //walk or run towards food
            {
                this.ChangeDirection(Coordinates.Direction(coordinates, food.getCoordinates()));
                if (food.GetType() == typeof(Herbivore) && Coordinates.Distance(coordinates, food.getCoordinates()) < 100) { this.Run(); }    //if the animal is hunting, it runs
                else { this.Walk(); }
            }
            else if (food.GetType() != typeof(Animal))  //eat food
            {
                if (food.GetType() == typeof(Meat))
                {
                    Meat meat = (Meat)food;
                    this.EatMeat(entities, meat);
                }
                else
                {
                    Plant plant = (Plant)food;
                    this.EatPlant(entities, plant);
                }
            }
            else  //kill food
            {
                Animal prey = (Animal)food;
                prey.Damage(25);
            }
        }
        private void EatPlant(Entities entities, Plant plant)
        {
            int emptyEnergy = 100 - energy;
            int life = plant.getLife();
            int calorieDensity = plant.getCalorieDensity();
            while (emptyEnergy > 0 && life > 0)
            {
                emptyEnergy-=calorieDensity;
                energy+= calorieDensity;
                life--;
            }
            if (life == 0)
            {
                entities.Remove(plant);
            }
            else
            {
                plant.Leave(life);
            }
        }
        public void Damage(int amount)
        {
            if (life < amount) { life = 0; }
            else { life -= amount; }
        }
        private void EatMeat(Entities entities, Meat meat)
        {
            int emptyEnergy = 100 - energy;
            int calories = meat.getCalories();
            while (emptyEnergy > 0 && calories > 0)
            {
                emptyEnergy--;
                energy++;
                calories--;
            }
            if (calories == 0)
            {
                entities.Remove(meat);
            }
            else
            {
                meat.Leave(calories);
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
        public override void Iterate(Entities entities)
        {
            base.Iterate(entities);
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
        public override void Iterate(Entities entities)
        {
            base.Iterate(entities);
            //behavior unique to carnivores
        }
        protected override Entity FindFood(Entities entities)
        {
            Entity response = null;
            bool foundMeat = false; //needed to prioritize eating meat as opposed to killing another animal
            int distance = 10000;
            foreach (Entity entity in entities.getList())
            {
                if ((entity.GetType() == typeof(Meat) || (entity.GetType() == typeof(Herbivore) && foundMeat == false)) //meat or animal if no meat was found yet
                    && Coordinates.Distance(coordinates, entity.getCoordinates()) < visionRadius    //within vision radius
                    && Coordinates.Distance(coordinates, entity.getCoordinates()) < distance)   //closer than previous finding
                {
                    if (entity.GetType() == typeof(Meat)) { foundMeat = true; }
                    response = entity;
                    distance = Coordinates.Distance(coordinates, entity.getCoordinates());
                }
            }
            return response;
        }
    }
    //SPECIES
    class Deer : Herbivore
    {
        //CONSTRUCTOR
        public Deer(int[] coordinates) :
        base(coordinates)
        {
            visionRadius = 20;
            contactRadius = 3;
            walkSpeed = 5;
            runSpeed = 8;
            gestationPeriod = 30;
        }
        //METHODS
        protected override Organism Reproduce(int[] coordinates) { return new Deer((int[])coordinates.Clone()); }
    }
    class Rabbit : Herbivore
    {
        //CONSTRUCTOR
        public Rabbit(int[] coordinates) :
        base(coordinates)
        {
            visionRadius = 8;
            contactRadius = 2;
            walkSpeed = 7;
            runSpeed = 8;
            gestationPeriod = 20;
        }
        //METHODS
        protected override Organism Reproduce(int[] coordinates) { return new Rabbit((int[])coordinates.Clone()); }
    }
    class Wolf : Carnivore
    {
        //CONSTRUCTOR
        public Wolf(int[] coordinates) :
        base(coordinates)
        {
            visionRadius = 30;
            contactRadius = 3;
            walkSpeed = 5;
            runSpeed = 10;
            gestationPeriod = 50;
        }
        //METHODS
        protected override Organism Reproduce(int[] coordinates) { return new Wolf((int[])coordinates.Clone()); }
    }
    class Fox : Carnivore
    {
        //CONSTRUCTOR
        public Fox(int[] coordinates) :
        base(coordinates)
        {
            visionRadius = 25;
            contactRadius = 3;
            walkSpeed = 5;
            runSpeed = 8;
            gestationPeriod = 70;
        }
        //METHODS
        protected override Organism Reproduce(int[] coordinates) { return new Wolf((int[])coordinates.Clone()); }
    }
    class Grass : Plant
    {
        //CONSTRUCTOR
        public Grass(int[] coordinates) :
        base(coordinates)
        {
            rootRadius = 30;
            sowingRadius = 5;
            propagationSpeed = 5;
            calorieDensity = 1;   //calories par life point
        }
        //METHODS
        protected override Organism Reproduce(int[] newCoordinates) { return new Grass((int[])coordinates.Clone()); }
    }
    class Bush : Plant
    {
        //CONSTRUCTOR
        public Bush(int[] coordinates) :
        base(coordinates)
        {
            rootRadius = 40;
            sowingRadius = 15;
            propagationSpeed = 3;
            calorieDensity = 3;   //calories par life point
        }
        //METHODS
        protected override Organism Reproduce(int[] newCoordinates) { return new Bush((int[])coordinates.Clone()); }
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
        public override void Iterate(Entities entities)
        {
            if (time < 20) { this.Rot(); }
            else { this.Transform(entities); }
        }
        private void Rot() { time += 1; }
        public override string ToString() { return base.ToString() + string.Format(", time={0}, calories={1}", time, calories); }
        public override void Transform(Entities entities)
        {
            entities.Add(new OrganicWaste(coordinates, 20));    //turn into organic waste
            entities.Remove(this);
        }
        public void Leave(int amount) { calories = amount; }
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
        public override void Iterate(Entities entities) { }
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
            entities.MatingTest();
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
