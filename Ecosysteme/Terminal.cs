using System;

namespace Ecosysteme
{
    public static class Terminal    //for information that needs to be displayed regularly
    {
        public static void Separate() { Console.WriteLine("______________________________________________________________"); }  //separation for more clarity in the console
        public static void Entities(Entities entities)  //Displays all the entities with their properties (ID, class, etc.)
        {
            Console.WriteLine(entities.Display());
            Terminal.Separate();
        }
        public static int AskIterations()   //asks the user to input how many iterations they would like the program to complete. If the user does not type an int, (s)he is asked again until (s)he does.
        {

            Console.WriteLine("How many iterations would you like to complete ?");
            string numberOfIterations = Console.ReadLine();
            if (int.TryParse(numberOfIterations, out int iterations))
            {
                return iterations;
            }
            Terminal.WrongIteration(numberOfIterations);
            return AskIterations();
        }
        private static void WrongIteration(string input) { Console.WriteLine($"\"{input}\" is not a number. Please type an integer."); }    //message displayed if the input is invalid
    }
}