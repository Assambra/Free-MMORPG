public sealed class GameConstants
{
    public static readonly string[] DISALLOWED_NAMES = { "admin", "administrator", "game master", "gamemaster", "gm" };

    public const int USERNAME_LENGTH_MIN = 6;
    public const int USERNAME_LENGTH_MAX = 20;
    public const int PASSWORD_LENGTH_MIN = 6;
    public const int PASSWORD_LENGTH_MAX = 30;

    public const int CHARACTER_NAME_LENGTH_MIN = 2;
    public const int CHARACTER_NAME_LENGTH_MAX = 20;

    private GameConstants() { }
}
