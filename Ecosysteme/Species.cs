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
            walkSpeed = 7;
            runSpeed = 8;
            gestationPeriod = 20;
            lifeExpectancy = 350;
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
            visionRadius = 10;
            contactRadius = 2;
            walkSpeed = 6;
            runSpeed = 8;
            gestationPeriod = 15;
            lifeExpectancy = 300;
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
            visionRadius = 40;
            contactRadius = 3;
            walkSpeed = 5;
            runSpeed = 10;
            gestationPeriod = 45;
            lifeExpectancy = 450;
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
            visionRadius = 30;
            contactRadius = 3;
            walkSpeed = 5;
            runSpeed = 10;
            gestationPeriod = 60;
            lifeExpectancy = 500;
        }
        //METHODS
        protected override Organism Reproduce(int[] coordinates) { return new Fox((int[])coordinates.Clone()); }
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
