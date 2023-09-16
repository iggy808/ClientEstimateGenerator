using CEG.Jobs.Runners;

namespace CEG.Jobs;
class Program
{ 
    public static void Main(string[] args)
    {
        if (args.Length == 0)
            return;

        switch (args.First())
        {
            case "-seedtestdb":
                DatabaseSeeder databaseSeeder = new DatabaseSeeder();
                Console.WriteLine(databaseSeeder.SeedTestDatabase());
                break;
        }

    }
}