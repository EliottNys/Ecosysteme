namespace Ecosysteme
{
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
        protected override void Transform(Entities entities)
        {
            entities.Add(new OrganicWaste(coordinates, 20));    //turn into organic waste
            entities.Remove(this);
        }
        public void Leave(int amount) { calories = amount; }
        //ACCESSORS
        public int getTime() { return time; }
        public int getCalories() { return calories; }
    }
}
