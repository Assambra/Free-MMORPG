using System.Linq;
using System.Text.RegularExpressions;

public static class InputValidator
{
    public static bool IsEmpty(string input)
    {
        return string.IsNullOrWhiteSpace(input);
    }

    public static bool IsLengthInvalid(string input, int minChars, int maxChars)
    {
        return input.Length < minChars || input.Length > maxChars;
    }

    public static bool ContainsDisallowedChar(string input, char[] disallowedChars)
    {
        foreach (var disallowedChar in disallowedChars)
        {
            if (input.Contains(disallowedChar.ToString()))
            {
                return true;
            }
        }
        return false;
    }

    public static bool ContainsDisallowedName(string input, string[] disallowedNames)
    {
        foreach (var disallowedName in disallowedNames)
        {
            if (input.Equals(disallowedName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsValidUsername(string input, char[] allowedSpecialCharacters)
    {
        return input.All(c => char.IsLetterOrDigit(c) || allowedSpecialCharacters.Contains(c));
    }

    public static bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }
}
