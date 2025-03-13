using System.Text;

namespace DomusApi.Helpers
{
    /// <summary>
    /// Used to generate random string ids
    /// </summary>
    public class StringIdProvider
    {
        private const string availableCharacters = "BCDFGHJKLMNPQRSTVWXYZ346789";

        public static StringIdProvider Instance { get { if (_instance == null) LoadAsync().Wait(); return _instance!; } }
        private static StringIdProvider? _instance = null;

        private Random random;

        public StringIdProvider()
        {
            random = new Random();
        }

        public static async Task LoadAsync() // doesn't actually do anything now, but maybe in the future we want to load some list of banned words or something
        {
            _instance = new StringIdProvider();

            await Task.CompletedTask;
        }

        /// <summary>
        /// Will generate a new random string id
        /// </summary>
        /// <param name="length">The length of the id to generate</param>
        /// <returns></returns>
        public string GenerateId(int length)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                result.Append(availableCharacters[random.Next(availableCharacters.Length)]);
            }

            return result.ToString();
        }
    }
}
