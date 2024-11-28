using System;

namespace Assambra.FreeServer
{
    public sealed class Commands
    {
        public const String SERVER_READY = "serverReady";
        public const String SERVER_STOP = "serverStop";
        public const String PLAYER_SPAWN = "playerSpawn";
        public const String PLAYER_DESPAWN = "playerDespawn";
        public const String CLIENT_TO_SERVER = "clientToServer";
        public const String SERVER_TO_CLIENT = "serverToClient";
        public const String SERVER_TO_CLIENTS = "serverToClients";
        public const String PLAYER_INPUT = "playerInput";
        public const String UPDATE_ENTITY_POSITION = "updateEntityPosition";
        public const String CHANGE_SERVER = "changeServer";

        private Commands() { }
    }
}

