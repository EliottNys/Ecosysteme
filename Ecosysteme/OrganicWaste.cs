namespace Ecosysteme
{
    class OrganicWaste : Entity  //created when a plant dies, meat rots or an animal poops
    {
        //ATTRIBUTES
        private int nutrients;   //equivalent to "calories" attribute for Meat class; defines how much energy it would provide to a plant
        //CONSTRUCTOR
        public OrganicWaste(int[] coordinates, int nutrients)
        {
            this.coordinates = coordinates;
            this.nutrients = nutrients;
        }
        //METHODS
        public override void Iterate(Entities entities) { }
        public override string ToString() { return base.ToString() + string.Format(", nutrients={0}", nutrients); }
        public void Leave(int leftover) { nutrients = leftover; }   //if only a part is consumed
        protected override void Transform(Entities entities) { }   //Organic waste does not transform
        //ACCESSORS
        public int getNutrients() { return nutrients; }
    }
}
