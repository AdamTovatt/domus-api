using Sakur.WebApiUtilities.Helpers;

namespace DomusApi.Helpers
{
    public static class ConnectionStringProvider
    {
        public static string GetConnectionString()
        {
            return ConnectionStringHelper.GetConnectionStringFromUrl(EnvironmentHelper.GetEnvironmentVariable("DATABASE_URL"), SslMode.Prefer);
        }
    }
}
