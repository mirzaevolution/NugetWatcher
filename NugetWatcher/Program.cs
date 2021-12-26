using System;

namespace NugetWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Nuget Watcher";
            Console.WriteLine("------------------------------");
            Console.WriteLine("      Nuget Watcher v1.0");
            Console.WriteLine("           (c) 2021");
            Console.WriteLine("        mgr@travlr.com");
            Console.WriteLine("------------------------------\n\n");
            using(Watcher watcher  = new Watcher())
            {
                watcher.Init();

                Console.WriteLine("\nPress ENTER to quit");
                Console.ReadLine();
            }
     

        }
    }
}
