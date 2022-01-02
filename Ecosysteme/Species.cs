namespace Ecosysteme
{
    class Deer : Herbivore
    {
        //CONSTRUCTOR
        public Deer(int[] coordinates) :
        base(coordinates)
        {
            visionRadius = 20;
            contactRadius = 3;
            walkSpeed = 5;
            runSpeed = 8;
            gestationPeriod = 30;
        }
        //METHODS
        protected override Organism Reproduce(int[] coordinates) { return new Deer((int[])coordinates.Clone()); }
    }
    class Rabbit : Herbivore
    {
        //CONSTRUCTOR
        public Rabbit(int[] coordinates) :
        base(coordinates)
        {
            visionRadius = 8;
            contactRadius = 2;
            walkSpeed = 7;
            runSpeed = 8;
            gestationPeriod = 20;
        }
        //METHODS
        protected override Organism Reproduce(int[] coordinates) { return new Rabbit((int[])coordinates.Clone()); }
    }
    class Wolf : Carnivore
    {
        //CONSTRUCTOR
        public Wolf(int[] coordinates) :
        base(coordinates)
        {
            visionRadius = 30;
            contactRadius = 3;
            walkSpeed = 5;
            runSpeed = 10;
            gestationPeriod = 50;
        }
        //METHODS
        protected override Organism Reproduce(int[] coordinates) { return new Wolf((int[])coordinates.Clone()); }
    }
    class Fox : Carnivore
    {
        //CONSTRUCTOR
        public Fox(int[] coordinates) :
        base(coordinates)
        {
            visionRadius = 25;
            contactRadius = 3;
            walkSpeed = 5;
            runSpeed = 8;
            gestationPeriod = 70;
        }
        //METHODS
        protected override Organism Reproduce(int[] coordinates) { return new Wolf((int[])coordinates.Clone()); }
    }
    class Grass : Plant
    {
        //CONSTRUCTOR
        public Grass(int[] coordinates) :
        base(coordinates)
        {
            rootRadius = 30;
            sowingRadius = 5;
            propagationSpeed = 3;
            calorieDensity = 1;   //calories par life point
        }
        //METHODS
        protected override Organism Reproduce(int[] coordinates) { return new Grass((int[])coordinates.Clone()); }
    }
    class Bush : Plant
    {
        //CONSTRUCTOR
        public Bush(int[] coordinates) :
        base(coordinates)
        {
            rootRadius = 40;
            sowingRadius = 15;
            propagationSpeed = 2;
            calorieDensity = 3;   //calories par life point
        }
        //METHODS
        protected override Organism Reproduce(int[] coordinates) { return new Bush((int[])coordinates.Clone()); }
    }
}
