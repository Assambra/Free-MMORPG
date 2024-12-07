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
        public const String PLAYER_SPAWN = "playerSpawn";
        public const String PLAYER_DESPAWN = "playerDespawn";

        public const String CLIENT_TO_SERVER = "clientToServer";
        public const String SERVER_TO_CLIENT = "serverToClient";
        public const String SERVER_TO_CLIENTS = "serverToClients";

        public const String PLAYER_INPUT = "playerInput";
        public const String PLAYER_JUMP = "playerJump";
        public const String UPDATE_ENTITY_POSITION = "updateEntityPosition";

        private Commands() { }
    }
}
