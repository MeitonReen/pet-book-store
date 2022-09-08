using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BookStore.Base.Implementations.Testing
{
    public class LoadEnvironmentVariablesFromLaunchSettingsFixture
    {
        public LoadEnvironmentVariablesFromLaunchSettingsFixture()
        {
            using (var file = File.OpenText("Properties\\launchSettings.json"))
            {
                var reader = new JsonTextReader(file);
                var jObject = JObject.Load(reader);

                var variables = jObject
                    .GetValue("profiles")
                    .SelectMany(profiles => profiles.Children())
                    .SelectMany(profile => profile.Children<JProperty>())
                    .Where(property => property.Name == "environmentVariables")
                    .SelectMany(property => property.Value.Children<JProperty>())
                    .ToList();

                foreach (var variable in variables)
                {
                    Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
                }
            }
        }
    }
}