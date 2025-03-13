
using DbUp.Engine;
using DbUp;
using DomusApi.Helpers;
using System.Reflection;
using Dapper;

namespace DomusApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            SetupDatabase();

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHostedService<HueEventHandlingService>();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        public static void SetupDatabase()
        {
            string connectionString = ConnectionStringProvider.GetConnectionString();

            UpgradeEngine upgrader =
                DeployChanges.To
                    .PostgresqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), (string s) => { return s.Contains("DatabaseMigrations") && s.Split(".").Last() == "sql"; })
                    .LogToConsole()
                    .Build();

            DatabaseUpgradeResult result = upgrader.PerformUpgrade();

            if (!result.Successful)
                throw new Exception($"Error when performing database upgrade, failing on script: {result.ErrorScript.Name} with error {result.Error}");

            DefaultTypeMap.MatchNamesWithUnderscores = true; // set up dapper to match column names with underscore
        }
    }
}
