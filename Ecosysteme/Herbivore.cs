namespace Ecosysteme
{
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
}
