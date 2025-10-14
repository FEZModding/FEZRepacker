using System.CommandLine.Parsing;

namespace FEZRepacker.Interface
{
    public static class CommandLineUtils
    {
        /// <summary>
        /// Parses the enum argument result into enum value using the <see cref="AliasesAttribute"/> attribute.
        /// If the attribute was not specified, the parser uses default behaviour of the <see cref="System.CommandLine"/> parser.  
        /// </summary>
        /// <param name="result">CLI Argument parsing result</param>
        /// <typeparam name="T">Enum type template</typeparam>
        /// <returns>An enum value</returns>
        /// <exception cref="ArgumentException">Throws if the template type is not an enum.</exception>
        public static T CustomAliasedEnumParser<T>(ArgumentResult result) where T : struct, IConvertible
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            foreach (var enumValue in Enum.GetValues(type))
            {
                var enumName = Enum.GetName(type, enumValue)!;
                var memberInfo = type.GetMember(enumValue.ToString()!);
                var attributes = memberInfo[0].GetCustomAttributes(typeof(AliasesAttribute), false);
                
                if (attributes.Length > 0)
                {
                    var aliases = (attributes.First() as AliasesAttribute)!.Aliases;
                    var aliasFound = aliases.Any(a => result.Tokens
                        .Any(t => t.Value.ToLower().Equals(a.ToLower())));
                    if (aliasFound) return (T)enumValue;
                }

                var nameFound = result.Tokens.Any(t => t.Value.ToLower().Equals(enumName.ToLower()));
                if (nameFound) return (T)enumValue;
            }

            return default;
        }

        /// <summary>
        /// Returns formatted argument and aliases list of provided enum value.
        /// </summary>
        /// <param name="enumValue">An enum value</param>
        /// <returns>Formatted list of available arguments</returns>
        public static string ArgumentsOf(Enum enumValue)
        {
            var type = enumValue.GetType();
            var name = Enum.GetName(type, enumValue)!;
            
            var memberInfo = type.GetMember(enumValue.ToString()!);
            var attributes = memberInfo[0].GetCustomAttributes(typeof(AliasesAttribute), false);

            var arguments = new List<string> { name };
            if (attributes.Length > 0)
            {
                var aliases = (attributes.First() as AliasesAttribute)!.Aliases;
                arguments.AddRange(aliases);
            }

            return String.Join("|", arguments);
        }
    }
}