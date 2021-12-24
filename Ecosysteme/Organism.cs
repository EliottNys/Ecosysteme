namespace Ecosysteme
{
    abstract class Organism : Entity    //everything that is alive
    {
        //ATTRIBUTES
        protected int life;
        protected int energy;
        //CONSTRUCTOR
        public Organism(int[] coordinates)
        {
            life = 100;
            energy = 100;
            this.coordinates = coordinates;
        }
        //METHODS
        public override void Iterate(Entities entities)
        {
            if (this.IterateEnergyAndLife(1))
            {
                this.Transform(entities);
            }
        }
        private bool IterateEnergyAndLife(int amount)
        {
            if (energy > amount)
            {
                this.Fatigue(amount);
                if (life < 100 && energy >= 80) //if an organism has over 80% energy, his health regenerates over time
                {
                    life += amount;
                }
                return false;
            }
            else if (life > amount - energy)
            {
                this.ConvertEnergy(amount - energy);
                this.Fatigue(amount);
                return false;
            }
            else
            {
                return true;
            }
        }
        public override void Transform(Entities entities)
        {
            if (this.GetType().IsSubclassOf(typeof(Plant))) //organism is plant
            {
                entities.Add(new OrganicWaste(coordinates, 20));    //turn into organic waste
            }
            else  //organism is animal
            {
                entities.Add(new Meat(coordinates, 20));
            }
            entities.Remove(this);
        }
        private void Fatigue(int amount) { energy -= amount; }   //an organism loses energy over time ; when an animal walks or runs, it loses energy
        private void ConvertEnergy(int amount)   //when an organism does not have any energy left, it converts lifepoints into energypoints
        {
            life -= amount;
            energy += amount;
        }
        abstract protected Organism Reproduce(int[] coordinates);
        public override string ToString() { return base.ToString() + string.Format(", life={0}, energy={1}", life, energy); }
        //ACCESSORS
        public int getLife() { return life; }
        public int getEnergy() { return energy; }
    }
}
