using System.Collections.Generic;
namespace Ecosysteme
{
    abstract class Plant : Organism //all plant species
    {
        //ATTRIBUTES
        protected int rootRadius; //how far a plant can consume organic waste
        protected int sowingRadius;   //how far new plants can appear
        protected List<int[]> occupiedArea; //area in which no other plant can grow,
        protected int propagationSpeed; //how frequently a plant sows (%)
        protected int calorieDensity; //! calories per life point (==> calories = calorieDensity * life)
        //CONSTRUCTOR
        public Plant(int[] coordinates) :
        base(coordinates)
        {
            this.occupiedArea = Coordinates.Area(coordinates, 2);   //if you want to override this in the species class: 1 occupies 5 points, 2 occupies 13 points, 3 occupies 29 points ...
        }
        //METHODS
        public override void Iterate(Entities entities)
        {
            //losing/regenerating of energy/life
            base.Iterate(entities);
            if (entities.getList().Contains(this))  //not dead yet
            {
                //feeding
                OrganicWaste food = this.FindOrganicWaste(entities);
                if (entities.getList().Contains(this)) { this.Consume(food, entities); }
                //reproduction
                int[] location = this.DetermineReproduction(entities);
                if (location != null && entities.getList().Contains(this))
                {
                    entities.Add(this.Reproduce(location));
                }
            }
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
                    distance = Coordinates.Distance(coordinates, entity.getCoordinates());
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
                if (entities.NoPlant(newCoordinates) && Coordinates.IsInside(newCoordinates, 55))
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
}
