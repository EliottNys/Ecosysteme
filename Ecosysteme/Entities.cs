using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ecosysteme
{
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
            if (text.Length > 1)
            {
                return text.Remove(text.Length - 1, 1); //remove last new line
            }
            else
            {
                return "There are no entities";
            }
        }
        public string NumberString()
        {
            int[] array = this.Number();
            string text;
            text = string.Format("Number of entities:\n-Grass: {0}\n-Bush: {1}\n-Deer: {2}\n-Rabbit: {3}\n-Wolf: {4}\n-Fox: {5}\n-Meat: {6}\n-Organic waste: {7}", array[0], array[1], array[2], array[3], array[4], array[5], array[6], array[7]);
            return text;
        }
        public int[] Number()
        {
            int[] array = new int[8];
            foreach (Entity entity in entities)
            {
                if (entity is Grass) { array[0]++; }
                else if (entity is Bush) { array[1]++; }
                else if (entity is Deer) { array[2]++; }
                else if (entity is Rabbit) { array[3]++; }
                else if (entity is Wolf) { array[4]++; }
                else if (entity is Fox) { array[5]++; }
                else if (entity is Meat) { array[6]++; }
                else if (entity is OrganicWaste) { array[7]++; }
            }
            return array;
        }
        public void Add(Entity entity)
        {
            entities.Add(entity);
            IDGenerator.GetId(entity, out entity.IsFirstTime);
        }
        public void Remove(Entity entity) { entities.Remove(entity); }   //note: the object is removed from the object "entities", but does it still use resources / exist ?
        public void Iterate(int times = 1)
        {
            for (int iteration = 0; iteration < times; iteration++)
            {
                Array initialArray = entities.ToArray();
                foreach (Entity entity in initialArray)
                {
                    entity.Iterate(this);
                }
            } 
        }
        public bool NoPlant(int[] newCoordinates)  //checks if there is not yet a plant on at these coordinates
        {
            foreach (Entity entity in entities)
            {
                if (entity is Plant)
                {
                    Plant plant = (Plant)entity;
                    //if (plant.getOccupiedArea().Contains(newCoordinates)) { return false; }
                    foreach (int[] point in plant.getOccupiedArea())
                    {
                        if (Coordinates.Same(point, newCoordinates)) { return false; }
                    }
                }
            }
            return true;
        }
        public bool Chance(int chance) { return random.Next(0, chance) == 0; }
        public void OneOfEach(int times)
        {
            for (int i = 0; i < times; i++)
            {
                this.Add(new Grass(new int[] { random.Next(-50, 51), random.Next(-50, 51) }));
                this.Add(new Bush(new int[] { random.Next(-50, 51), random.Next(-50, 51) }));
                this.Add(new Deer(new int[] { random.Next(-50, 51), random.Next(-50, 51) }));
                this.Add(new Rabbit(new int[] { random.Next(-50, 51), random.Next(-50, 51) }));
                this.Add(new Wolf(new int[] { random.Next(-50, 51), random.Next(-50, 51) }));
                this.Add(new Fox(new int[] { random.Next(-50, 51), random.Next(-50, 51) }));
            }
        }
        public void GrassOrganicWaste() //to observe a plant eating
        {
            this.Add(new Grass(new int[] { -5, 0 }));
            this.Add(new OrganicWaste(new int[] { 5, 0 }, 25));
        }
        public void DeerGrass() //to observe a herbivore eating
        {
            this.Add(new Deer(new int[] { -5, 0 }));
            this.Add(new Grass(new int[] { 5, 0 }));
        }
        public void WolfMeat()  //to observe a carnivore eating
        {
            this.Add(new Wolf(new int[] { -5, 0 }));
            this.Add(new Meat(new int[] { 5, 0 }, 25));
        }
        public void WolfDeer()  //to observe hunting
        {
            this.Add(new Wolf(new int[] { -5, 0 }));
            this.Add(new Deer(new int[] { 5, 0 }));
        }
        public void OnlyGrass()     //to observe the propagation of a plant
        {
            this.Add(new Grass(new int[] { 0, 0 }));
        }
        public void DeerDeer()  //to observe mating (3 couples to increase chance of opposite sex)
        {
            Deer deer1 = new Deer(new int[] { -5, 0 });
            Deer deer2 = new Deer(new int[] { 5, 0 });
            while (deer1.getSex() == deer2.getSex()) { deer2 = new Deer(new int[] { 5, 0 }); }
            this.Add(deer1);
            this.Add(deer2);
        }
        public void Scenario1()
        {
            //bushes
            this.Add(new Bush(new int[] { 0, 0 }));
            this.Add(new Bush(new int[] { 0, 50 }));
            this.Add(new Bush(new int[] { -25, 0 }));
            this.Add(new Bush(new int[] { -25, -25 }));
            this.Add(new Bush(new int[] { -12, 50 }));
            this.Add(new Bush(new int[] { 13, 50 }));
            this.Add(new Bush(new int[] { 30, 20 }));
            this.Add(new Bush(new int[] { 40, -40 }));
            //grass
            this.Add(new Grass(new int[] { 0, 12 }));
            this.Add(new Grass(new int[] { -13, 0 }));
            this.Add(new Grass(new int[] { -25, 12 }));
            this.Add(new Grass(new int[] { -13, 25 }));
            this.Add(new Grass(new int[] { 0, 38 }));
            this.Add(new Grass(new int[] { 25, 12 }));
            this.Add(new Grass(new int[] { 25, -25 }));
            this.Add(new Grass(new int[] { 25, -50 }));
            this.Add(new Grass(new int[] { -20, -40 }));
            this.Add(new Grass(new int[] { -50, -20 }));
            this.Add(new Grass(new int[] { -35, 40 }));
            this.Add(new Grass(new int[] { 40, 35 }));
            //let the plants spread
            this.Iterate(500);
            //herbivores
            this.Add(new Deer(new int[] { 0, 0} ));
            this.Add(new Deer(new int[] { -16, 16 }));
            this.Add(new Deer(new int[] { -5, 16 }));
            this.Add(new Deer(new int[] { 10, 31 }));
            this.Add(new Deer(new int[] { 19, 17 }));
            this.Add(new Deer(new int[] { 0, 25 }));
            this.Add(new Rabbit(new int[] { 0, 30 }));
            this.Add(new Rabbit(new int[] { -2, 30 }));
            this.Add(new Rabbit(new int[] { -5, 27 }));
            this.Add(new Rabbit(new int[] { 0, 20 }));
            this.Add(new Rabbit(new int[] { 5, 15 }));
            //time to mate
            this.Iterate(35);
            //carnivores
            Wolf wolf1 = new Wolf(new int[] { -20, -10 });
            Wolf wolf2 = new Wolf(new int[] { 0, -20 });
            while (wolf1.getSex() == wolf2.getSex()) { wolf2 = new Wolf(new int[] { 0, -20 }); }    //not same sex or the species will die out
            this.Add(wolf1);
            this.Add(wolf2);
            Fox fox1 = new Fox(new int[] { 20, 40 });
            Fox fox2 = new Fox(new int[] { 25, 30 });
            while (fox1.getSex() == fox2.getSex()) { fox2 = new Fox(new int[] { 25, 30 }); }
            this.Add(fox1);
            this.Add(fox2);
        }
        //ACCESSORS
        public List<Entity> getList() { return entities; }
    }
}