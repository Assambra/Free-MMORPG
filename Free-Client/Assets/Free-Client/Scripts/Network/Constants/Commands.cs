using System;

public sealed class Commands
{
    public const String CREATE_ACCOUNT = "createAccount";
    public const String FORGOT_PASSWORD = "forgotPassword";
    public const String FORGOT_USERNAME = "forgotUsername";

    private Commands() { }
}
