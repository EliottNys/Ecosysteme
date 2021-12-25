namespace Ecosysteme
{
    public abstract class Entity   //all objects that interact (organisms, meat & organic waste)
    {
        //ATTRIBUTES
        protected int[] coordinates;    //position in the plane
        public bool IsFirstTime;    //needed for the ID generator (public so the generator can modify it)
        //METHODS
        abstract public void Iterate(Entities entities); //what happens to the entity or what the entity does at each iteration
        //eventually, this has to take as input the list of all entities and will send as ouput an enumerate of actions that must be executed by the program / an updated list of entities
        public override string ToString() { return string.Format("coordinates=[{0},{1}]", coordinates[0], coordinates[1]); }
        abstract protected void Transform(Entities entities);
        //ACCESSORS
        public int[] getCoordinates() { return coordinates; }
    }
}
