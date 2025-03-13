using Dapper;
using DomusApi.Helpers;
using DomusApi.Models;
using Npgsql;

namespace DomusApi.Repositories
{
    public class HueCredentialsRepository
    {
        public static HueCredentialsRepository Instance
        {
            get
            {
                if (_instance == null) _instance = new HueCredentialsRepository();
                return _instance;
            }
        }

        private static HueCredentialsRepository? _instance;
        private string connectionString;

        public HueCredentialsRepository()
        {
            connectionString = ConnectionStringProvider.GetConnectionString();
        }

        private async Task<NpgsqlConnection> GetConnectionAsync()
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task<HueCredentials?> GetAsync()
        {
            const string query = $"SELECT * FROM hue_key";

            using (NpgsqlConnection connection = await GetConnectionAsync())
                return (await connection.QueryAsync<HueCredentials>(query)).FirstOrDefault();
        }

        public async Task<HueCredentials> InsertAsync(HueCredentials credentials)
        {
            HueCredentials? existingCredentials = await GetAsync();

            string query = string.Empty;

            if (existingCredentials != null)
            {
                query = @$"UPDATE hue_key SET
                           key_value = @{nameof(credentials.KeyValue)},
                           ip_address = @{nameof(credentials.IpAddress)},
                           username = @{nameof(credentials.Username)};";
            }
            else
            {
                query = @$"INSERT INTO hue_key (key_value, ip_address, username) VALUES
                       (@{nameof(credentials.KeyValue)}, @{nameof(credentials.IpAddress)}, @{nameof(credentials.Username)})";
            }

            using (NpgsqlConnection connection = await GetConnectionAsync())
                await connection.ExecuteAsync(query, credentials);

            return credentials;
        }
    }
}
