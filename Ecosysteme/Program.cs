namespace Ecosysteme
{
    class Program
    {
        static void Main(string[] args)
        {
            Entities entities = new Entities();
            entities.MatingTest();
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
