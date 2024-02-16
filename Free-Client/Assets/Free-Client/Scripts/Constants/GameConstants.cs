public sealed class GameConstants
{
    public const int MIN_USERNAME_LENGTH = 6;
    public const int MAX_USERNAME_LENGTH = 20;
    public const int MIN_PASSWORD_LENGTH = 6;
    public const int MAX_PASSWORD_LENGTH = 30;
    public static readonly char[] VALID_USERNAME_CHARACTERS = { '_', '-', '.'};
    public static readonly string[] DISALLOWED_USERNAMES = { "admin", "administrator", "game master", "gamemaster", "gm" };
    

    private GameConstants() { }
}
