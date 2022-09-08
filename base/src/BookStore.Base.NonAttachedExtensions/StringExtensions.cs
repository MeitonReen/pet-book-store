namespace BookStore.Base.NonAttachedExtensions
{
    //without semantic analysis
    public static class StringExtensions
    {
        public static Guid? ToGuid(this string strGuid)
        {
            return Guid.TryParse(strGuid, out var result) ? result : default;
        }

        public static string GetDefaultLastMemberName(this string name)
        {
            var lastDotIndex = name.LastIndexOf('.');
            var startIndex = lastDotIndex == -1 ? 0 : ++lastDotIndex;

            return name[startIndex..];
        }

        public static string GetLastMemberName(this string name)
        {
            var lastSeparatorIndex = name.LastIndexOf('.');
            var openingParenthesisIndex = name.LastIndexOf('(');

            var startIndex = lastSeparatorIndex == -1 ? 0 : ++lastSeparatorIndex;
            var endIndex = openingParenthesisIndex == -1 ? name.Length : openingParenthesisIndex;

            return name[startIndex..endIndex];
        }

        public static string ToText(this string str)
        {
            return string
                .Concat(
                    str.Select((x, i) =>
                        (i == 0 && char.IsLower(x)) ? char.ToUpper(x).ToString()
                        : (i > 0 && char.IsUpper(x)) ? " " + char.ToLower(x)
                        : x.ToString()
                    ));
        }

        public static string ToUpperSnakeCase(this string str)
        {
            return string
                .Concat(
                    str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
                .ToUpper();
        }

        public static string ToKebabCaseFromCamelCase(this string camelCaseStr)
            => string.Concat(camelCaseStr
                .Select((el, i) =>
                {
                    if (i == 0) return $"{char.ToLower(el)}";

                    return char.IsUpper(el) ? $"-{char.ToLower(el)}" : $"{el}";
                }));

        public static string ToKebabCaseNameFromNamespace(this string namespaceStr)
            => string.Concat(namespaceStr
                .Select((el, i) =>
                {
                    if (i == 0) return $"{char.ToLower(el)}";

                    if (el == '.') return "-";

                    if (char.IsUpper(el))
                    {
                        return namespaceStr[i - 1] == '.'
                            ? $"{char.ToLower(el)}"
                            : $"-{char.ToLower(el)}";
                    }

                    return $"{el}";
                }));

        public static string ToKebabCaseNameFromTypeFullName(this string typeFullName)
            => ToKebabCaseNameFromNamespace(typeFullName);

        public static string ToLowerCamelCase(this string str)
        {
            str = char.IsUpper(str[0]) ? char.ToLower(str[0]) + str[1..str.Length] : str;

            var up = false;
            return string.Concat(
                str.Select((x, i) =>
                {
                    if (up)
                    {
                        up = false;
                        return char.ToUpper(x).ToString();
                    }

                    if (i > 0 && x == '_')
                    {
                        up = true;
                        return "";
                    }

                    return x.ToString();
                })
            );
        }
    }
}