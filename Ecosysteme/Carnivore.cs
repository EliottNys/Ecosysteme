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
            int distanceMin = 10000;
            foreach (Entity entity in entities.getList())
            {
                int distance = Coordinates.Distance(coordinates, entity.getCoordinates());
                if (entity is Meat || (entity is Herbivore && foundMeat == false) //meat or animal if no meat was found yet
                    && distance < visionRadius    //within vision radius
                    && distance < distanceMin)   //closer than previous finding
                {
                    if (entity is Meat) { foundMeat = true; }
                    response = entity;
                    distanceMin = Coordinates.Distance(coordinates, entity.getCoordinates());
                }
            }
            return response;
        }
    }
}
