namespace Ecosysteme
{
    class Program
    {
        static void Main(string[] args)
        {
            Entities entities = new Entities();
            entities.OneOfEach(3);
            //entities.GrassOrganicWaste();     //to observe a plant eating
            //entities.DeerGrass();             //to observe a herbivore eating
            //entities.WolfMeat();              //to observe a carnivore eating
            //entities.WolfDeer();              //to observe hunting
            //entities.OnlyGrass();             //to observe the propagation of a plant
            //entities.DeerDeer();              //to observe mating (3 couples to increase chance of opposite sex)
            Terminal.Entities(entities);
            while (true)
            {
                int numberOfIterations = Terminal.AskIterations();
                for (int iteration=0; iteration < numberOfIterations; iteration++)
                {
                    entities.Iterate();
                }
                Terminal.Separate();
                Terminal.Entities(entities);
            }
        }
    }
}
