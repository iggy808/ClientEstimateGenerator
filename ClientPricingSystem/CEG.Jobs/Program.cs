using CEG.Jobs.Runners;

namespace CEG.Jobs;
class Program
{ 
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Provide a unique job argument describing which job you would like to run. A list of CEG jobs and their respective arguments is available in the section of documentation dedicated to CEG Jobs.");
            return;
        }

        switch (args.First())
        {
            case "-seedtestdb":
                DatabaseSeeder databaseSeeder = new DatabaseSeeder();
                Console.WriteLine(databaseSeeder.SeedTestDatabase());
                break;
        }

    }
}