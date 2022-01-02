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
        public string Number()
        {
            int grass = 0;
            int bush = 0;
            int deer = 0;
            int rabbit = 0;
            int wolf = 0;
            int fox = 0;
            foreach (Entity entity in entities)
            {
                if (entity is Grass) { grass ++; }
                else if (entity is Bush) { bush++; }
                else if (entity is Deer) { deer++; }
                else if (entity is Rabbit) { rabbit++; }
                else if (entity is Wolf) { wolf++; }
                else if (entity is Fox) { fox++; }
            }
            string text;
            text = string.Format("Grass: {0} entities\nBush: {1} entities\nDeer: {2} entities\nRabbit: {3} entities\nWolf: {4} entities\nFox: {5} entities", grass, bush, deer, rabbit, wolf, fox) ;
            return text;
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
            this.Add(new Deer(new int[] { -5, 0 }));
            this.Add(new Deer(new int[] { 5, 0 }));
            this.Add(new Deer(new int[] { -5, 50 }));
            this.Add(new Deer(new int[] { 5, 50 }));
            this.Add(new Deer(new int[] { -5, -50 }));
            this.Add(new Deer(new int[] { 5, -50 }));
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
            this.Iterate(45);
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
            this.Add(new Wolf(new int[] { -20, -10 }));
            this.Add(new Wolf(new int[] { 0, -20 }));
            this.Add(new Fox(new int[] { 20, 40 }));
            this.Add(new Fox(new int[] { 25, 30 }));
        }
        //ACCESSORS
        public List<Entity> getList() { return entities; }
    }
}