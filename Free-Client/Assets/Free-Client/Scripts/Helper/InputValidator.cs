using System.Linq;
using System.Text.RegularExpressions;

public static class InputValidator
{
    public static bool IsNotEmpty(string input)
    {
        return !string.IsNullOrWhiteSpace(input);
    }

    public static bool IsLengthValid(string input, int minChars, int maxChars)
    {
        return input.Length >= minChars && input.Length <= maxChars;
    }

    public static bool DoesNotContainDisallowedChar(string input, char[] disallowedChars)
    {
        return !disallowedChars.Any(input.Contains);
    }

    public static bool DoesNotContainDisallowedName(string input, string[] disallowedNames)
    {
        return !disallowedNames.Any(disallowedName => input.Equals(disallowedName, System.StringComparison.OrdinalIgnoreCase));
    }

    public static bool IsValidUsername(string input, char[] allowedSpecialCharacters, bool beginWithSpaceCheck = false)
    {
        if(beginWithSpaceCheck)
        {
            if (input[0] == ' ')
                return false;
        }

        return input.All(c => char.IsLetterOrDigit(c) || allowedSpecialCharacters.Contains(c));
    }

    public static bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    public static bool IsValidPassword(string password, int minLength, int maxLength)
    {

        if (string.IsNullOrWhiteSpace(password) || password.Length < minLength || password.Length > maxLength)
        {
            return false;
        }

        if (!Regex.IsMatch(password, @"\d"))
        {
            return false;
        }

        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            return false;
        }

        
        if (!Regex.IsMatch(password, @"[a-z]"))
        {
            return false;
        }

        
        if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':;{}|<>]"))
        {
            return false;
        }

        return true;
    }
}
