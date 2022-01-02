using System.Diagnostics;
namespace Ecosysteme
{
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
                if ((entity.GetType() == typeof(Meat) || (entity is Herbivore && foundMeat == false)) //meat or animal if no meat was found yet
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
}
