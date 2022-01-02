namespace Ecosysteme
{
    class Program
    {
        static void Main(string[] args)
        {
            Entities entities = new Entities();
            //entities.OneOfEach(3);
            //entities.GrassOrganicWaste();     //to observe a plant eating
            //entities.DeerGrass();             //to observe a herbivore eating
            //entities.WolfMeat();              //to observe a carnivore eating
            //entities.WolfDeer();              //to observe hunting
            //entities.OnlyGrass();             //to observe the propagation of a plant
            //entities.DeerDeer();              //to observe mating (3 couples to increase chance of opposite sex)
            entities.Scenario1();
            //entities.Add(new Meat(new int[] { 0, 0 }, 20));   //example to manually add an entity
            Terminal.Entities(entities);
            while (true)
            {
                int numberOfIterations = Terminal.AskIterations();
                entities.Iterate(numberOfIterations);
                Terminal.Separate();
                Terminal.Entities(entities);
            }
        }
    }
}
