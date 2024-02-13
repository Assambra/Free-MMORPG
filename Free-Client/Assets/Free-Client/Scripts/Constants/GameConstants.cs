public sealed class GameConstants
{
    // Input Validator
    public const int MIN_USERNAME_LENGTH = 2;
    public const int MAX_USERNAME_LENGTH = 20;
    public static readonly string[] DISALLOWED_NAMES = { "admin", "administrator", "game master", "gamemaster", "gm" };
    public static readonly char[] VALID_USERNAME_CHARACTERS = { '_', '-', '.', ' ' };

    private GameConstants() { }
}
