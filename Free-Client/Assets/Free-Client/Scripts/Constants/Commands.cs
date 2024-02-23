using System;

namespace Assambra.FreeClient.Constants
{
    public sealed class Commands
    {
        public const String CREATE_ACCOUNT = "createAccount";
        public const String ACTIVATE_ACCOUNT = "activateAccount";
        public const String RESEND_ACTIVATION_MAIL = "resendActivationMail";
        public const String FORGOT_PASSWORD = "forgotPassword";
        public const String FORGOT_USERNAME = "forgotUsername";

        public const String CHECK = "check";
        public const String CHARACTER_LIST = "characterList";
        public const String CREATE_CHARACTER = "createCharacter";

        public const String PLAY = "play";
        public const String CHARACTER_SPAWNED = "characterSpawned";
        public const String CHARACTER_DESPAWNED = "characterDespawned";

        public const String PLAYER_INPUT = "playerInput";
        public const String SYNC_POSITION = "s";

        private Commands() { }
    }
}
