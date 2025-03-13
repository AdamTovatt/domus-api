namespace DomusApiTests
{
    public class LaunchSettingsLoader
    {
        public static string GetConnectionString()
        {
            if (File.Exists("connectionString.txt")) // if you add a file called "connectionString.txt" in DomusApi\Tests\bin\Debug\net8.0 it will use that
            {
                return File.ReadAllText("connectionString.txt");
            }

            throw new Exception("No connection string file found");
        }
    }
}
