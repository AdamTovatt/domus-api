using DomusApi;

namespace DomusApiTests
{
    [TestClass]
    public class Setup
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            Environment.SetEnvironmentVariable("DATABASE_URL", LaunchSettingsLoader.GetConnectionString());

            Program.SetupDatabase();
        }
    }
}
