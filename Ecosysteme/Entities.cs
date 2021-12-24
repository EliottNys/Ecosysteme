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
                this.Add(new Grass(new int[] { random.Next(-100, 101), random.Next(-100, 101) }));
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
}