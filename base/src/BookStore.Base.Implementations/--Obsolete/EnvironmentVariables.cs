using System.Text;
using BookStore.Base.NonAttachedExtensions;

namespace BookStore.Base.Implementations.__Obsolete
{
    public static class EnvironmentVariables
    {
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="envVariableNameParts">
        /// Last param for property, other params for prefixes.
        /// <br/>Example: last param = "AbcDef" --> "Abc_Def"; other params: ["Abc", "Def"] --> "Abc_Def".
        /// </param>
        /// <returns>Environment variable on envVariableNameParts</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string Get(params string[] envVariableNameParts)
        {
            string envVariableName = EnvVariableNamePartsToEnvVariableName(envVariableNameParts);

            return GetVariable(envVariableName);
        }

        public static string GetOnlyDevelopment(params string[] envVariableNameParts)
        {
            return GetVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                ? Get(envVariableNameParts)
                : throw new InvalidOperationException(
                    "It is allowed to get this environment variable only from the development state");
        }

        public static string GetOnlyProduction(params string[] envVariableNameParts)
        {
            return GetVariable("ASPNETCORE_ENVIRONMENT") == "Production"
                ? Get(envVariableNameParts)
                : throw new InvalidOperationException(
                    "It is allowed to get this environment variable only from the production state");
        }

        public static string[] GetArrayFromVariable(string separator, params string[] envVariableNameParts)
        {
            string envVariableGluedValues = Get(envVariableNameParts);

            string[] envVariableValues = envVariableGluedValues.Split(separator);

            return envVariableValues;
        }

        private static string GetVariable(string envVariableName)
        {
            return Environment.GetEnvironmentVariable(envVariableName) ??
                   throw new ArgumentException($"Environment variable: {envVariableName} is not found");
        }

        private static string EnvVariableNamePartsToEnvVariableName(string[] envVariableNameParts)
        {
            if (envVariableNameParts.Length == 1)
            {
                return envVariableNameParts[^1].ToUpperSnakeCase();
            }

            string[] prefixes = envVariableNameParts[..^1];
            string property = envVariableNameParts[^1];

            string envVariableName = $"{GluePrefixes(prefixes)}_{property.ToUpperSnakeCase()}";

            return envVariableName;
        }

        private static string GluePrefixes(string[] prefixes)
        {
            var stringBuilder = new StringBuilder();

            prefixes
                .Select((prefix, i) =>
                    i == 0 ? stringBuilder.Append(prefix.ToUpper()) : stringBuilder.Append($"_{prefix}".ToUpper()))
                .ToArray();

            return stringBuilder.ToString();
        }
    }
}