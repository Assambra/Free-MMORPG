using com.tvd12.ezyfoxserver.client;
using com.tvd12.ezyfoxserver.client.constant;
using com.tvd12.ezyfoxserver.client.entity;
using com.tvd12.ezyfoxserver.client.factory;
using com.tvd12.ezyfoxserver.client.request;
using com.tvd12.ezyfoxserver.client.support;
using com.tvd12.ezyfoxserver.client.unity;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Windows;
using Object = System.Object;

namespace Assambra.FreeServer
{
    public class NetworkManager : EzyAbstractController
    {
        public static NetworkManager Instance { get; private set; }

        [SerializeField] private EzySocketConfig socketConfig;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;

            socketConfig = GetSocketConfig();
        }

        private new void OnEnable()
        {
            base.OnEnable();
            AddHandler<EzyObject>(Commands.SERVER_STOP, ServerStopRequest);
            AddHandler<EzyObject>(Commands.PLAYER_SPAWN, PlayerSpawnRequest);
            AddHandler<EzyObject>(Commands.PLAYER_DESPAWN, PlayerDespawnRequest);
            AddHandler<EzyObject>(Commands.PLAYER_INPUT, ReceivePlayerInput);
            AddHandler<EzyObject>(Commands.PLAYER_JUMP, ReceivePlayerJump);
        }

        private void Update()
        {
            EzyClients.getInstance()
                .getClient(socketConfig.ZoneName)
                .processEvents();
        }

        protected override EzySocketConfig GetSocketConfig()
        {
            return socketConfig;
        }

        public void Login(string username, string password)
        {
            Debug.Log("Login username = " + username + ", password = " + password);
            Debug.Log("Socket clientName = " + socketProxy.getClient().getName());

            socketProxy.onLoginSuccess<Object>(HandleLoginSuccess);
            socketProxy.onUdpHandshake<Object>(HandleUdpHandshake);
            socketProxy.onAppAccessed<Object>(HandleAppAccessed);

            // Login to socket server
            socketProxy.setLoginUsername(username);
            socketProxy.setLoginPassword(password);

            socketProxy.setUrl(socketConfig.TcpUrl);
            socketProxy.setUdpPort(socketConfig.UdpPort);
            socketProxy.setDefaultAppName(socketConfig.AppName);
            socketProxy.setTransportType(EzyTransportType.TCP);

            socketProxy.connect();
        }

        public new void Disconnect()
        {
            base.Disconnect();
        }

        #region SEND TO MASTER SERVER

        private void SendServerReadyRequest()
        {
            EzyObject data = EzyEntityFactory.newObjectBuilder()
                .append("password", ServerManager.Instance.Password)
                .build();

            appProxy.send(Commands.SERVER_READY, data);
        }

        public void SendChangeServerRequest(long playerId, string room, Vector3 position, Vector3 rotation)
        {
            EzyObject data = EzyEntityFactory.newObjectBuilder()
                .append("playerId", playerId)
                .append("room", room)
                .append("position", EzyEntityFactory.newArrayBuilder()
                        .append(position.x)
                        .append(position.y)
                        .append(position.z)
                        .build())
                .append("rotation", EzyEntityFactory.newArrayBuilder()
                    .append(rotation.x)
                    .append(rotation.y)
                    .append(rotation.z)
                    .build())
                .build();

            appProxy.send(Commands.CHANGE_SERVER, data);
        }

        #endregion

        #region MASTER SERVER REQUESTS

        private void ServerStopRequest(EzyAppProxy proxy, EzyObject data)
        {
            Debug.Log("Receive SERVER_STOP request");

            Disconnect();
            Application.Quit();
        }

        private void PlayerSpawnRequest(EzyAppProxy proxy, EzyObject data)
        {
            long id = data.get<long>("id");
            string username = data.get<string>("username");
            string name = data.get<string>("name");
            string sex = data.get<string>("sex");
            string race = data.get<string>("race");
            string model = data.get<string>("model");
            EzyArray position = data.get<EzyArray>("position");
            EzyArray rotation = data.get<EzyArray>("rotation");
            Vector3 pos = new Vector3(position.get<float>(0), position.get<float>(1), position.get<float>(2));
            Vector3 rot = new Vector3(rotation.get<float>(0), rotation.get<float>(1), rotation.get<float>(2));

            ServerManager.Instance.ServerLog.ServerLogMessageInfo($"Receive command.PLAYER_SPAWN for Id: {id} Name: {name}");

            GameObject playerGameObject = ServerManager.Instance.CreatePlayer(pos, rot);
            playerGameObject.name = name;

            Player player = playerGameObject.GetComponent<Player>();

            if (player != null)
            {
                player.Initialize((uint)id, name, sex, race, model, playerGameObject, false, EntityType.Player, username);
                player.SetPlayerHeadinfoName(name);
                ServerManager.Instance.ServerEntities.Add((uint)id, player);
            }
            else
            {
                Debug.LogError("Player component not found on the playerGameObject.");
            }

            // Send PlayerSpawn to client
            bool isLocalPlayer = true;
            SendServerToClient(username, "playerSpawn", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("id", id),
                new KeyValuePair<string, object>("entityType", 1),
                new KeyValuePair<string, object>("name", name),
                new KeyValuePair<string, object>("model", model),
                new KeyValuePair<string, object>("position", position),
                new KeyValuePair<string, object>("rotation", rotation),
                new KeyValuePair<string, object>("sex", sex),
                new KeyValuePair<string, object>("race", race),
                new KeyValuePair<string, object>("room", ServerManager.Instance.Room),
                new KeyValuePair<string, object>("isLocalPlayer", isLocalPlayer),
            });
        }

        private void PlayerDespawnRequest(EzyAppProxy proxy, EzyObject data)
        {
            long id = data.get<long>("id");

            if (ServerManager.Instance.ServerEntities.TryGetValue((uint)id, out Entity entity))
            {
                ServerManager.Instance.ServerLog.ServerLogMessageInfo($"Receive command.PLAYER_DESPAWN for Id: {id} Name: {entity.Name}");

                if (entity is Player player)
                {
                    player.MasterServerRequestedDespawn = true;

                    SendDespawnToPlayer(player.Username, player.Id);

                    List<string> usernames = player.NearbyPlayers.Values.Select(player => player.Username).ToList();
                    SendServerToClients(usernames, "playerDespawn", new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>("id", (long)player.Id),
                    });

                    Destroy(player.EntityGameObject);
                    ServerManager.Instance.ServerEntities.Remove(player.Id);
                }
            }
        }

        #endregion

        #region MASTER SERVER RESPONSE

        private void HandleLoginSuccess(EzySocketProxy proxy, Object data)
        {
            Debug.Log("Log in successfully");
        }

        private void HandleUdpHandshake(EzySocketProxy proxy, Object data)
        {
            Debug.Log("HandleUdpHandshake");
            socketProxy.send(new EzyAppAccessRequest(socketConfig.AppName));
        }

        private void HandleAppAccessed(EzyAppProxy proxy, Object data)
        {
            Debug.Log("App access successfully");

            Debug.Log("ServerReadyRequest");
            SendServerReadyRequest();
        }

        #endregion

        #region SEND TO CLIENT

        public void SendSpawnToPlayer(string username, long id, string name, string sex, string race, string model, Vector3 position, Vector3 rotation)
        {
            bool isLocalPlayer = false;
            string room = "";

            EzyArray positionArray = EzyEntityFactory.newArrayBuilder()
                .append(position.x)
                .append(position.y)
                .append(position.z)
                .build();

            EzyArray rotationArray = EzyEntityFactory.newArrayBuilder()
                .append(rotation.x)
                .append(rotation.y)
                .append(rotation.z)
                .build();

            SendServerToClient(username, "playerSpawn", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("id", id),
                new KeyValuePair<string, object>("entityType", 1),
                new KeyValuePair<string, object>("name", name),
                new KeyValuePair<string, object>("model", model),
                new KeyValuePair<string, object>("position", positionArray),
                new KeyValuePair<string, object>("rotation", rotationArray),
                new KeyValuePair<string, object>("sex", sex),
                new KeyValuePair<string, object>("race", race),
                new KeyValuePair<string, object>("room", room),
                new KeyValuePair<string, object>("isLocalPlayer", isLocalPlayer)  
            });
        }

        public void SendDespawnToPlayer(string username, long id)
        {
            SendServerToClient(username, "playerDespawn", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("id", id),
            });
        }

        public void SendUpdateEntityPosition(string username, long id, Vector3 position, Vector3 rotation)
        {
            EzyArray positionArray = EzyEntityFactory.newArrayBuilder()
                .append(position.x)
                .append(position.y)
                .append(position.z)
                .build();

            EzyArray rotationArray = EzyEntityFactory.newArrayBuilder()
                .append(rotation.x)
                .append(rotation.y)
                .append(rotation.z)
                .build();

            SendServerToClient(username, "updateEntityPosition", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("id", id),
                new KeyValuePair<string, object>("position", positionArray),
                new KeyValuePair<string, object>("rotation", rotationArray)
            });
        }

        #endregion

        #region RECEIVE FROM CLIENT

        private void ReceivePlayerInput(EzyAppProxy proxy, EzyObject data)
        {
            long id = data.get<long>("id");
            EzyArray inputArray = data.get<EzyArray>("input");

            Vector2 input = new Vector2(inputArray.get<float>(0), inputArray.get<float>(1));

            if (ServerManager.Instance.ServerEntities.TryGetValue((uint)id, out Entity entity))
            {
                if (entity is Player player)
                {
                    PlayerController playerController = player.EntityGameObject.GetComponent<PlayerController>();
                    playerController.Move = new Vector3(input.x, 0, input.y);
                }
            }
        }

        public void ReceivePlayerJump(EzyAppProxy proxy, EzyObject data)
        {
            long id = data.get<long>("id");

            if (ServerManager.Instance.ServerEntities.TryGetValue((uint)id, out Entity entity))
            {
                if (entity is Player player)
                {
                    PlayerController playerController = player.EntityGameObject.GetComponent<PlayerController>();
                    playerController.Jump = true;
                }
            }
        }

        #endregion

        #region ROOM SERVER TO CLIENT MESSAGES

        private void SendServerToClient(string recipient, string command, List<KeyValuePair<string, object>> additionalParams)
        {
            Debug.Log("SendServerToClient");

            var dataBuilder = EzyEntityFactory.newObjectBuilder()
                .append("recipient", recipient)
                .append("command", command);

            foreach (var pair in additionalParams)
            {
                dataBuilder.append(pair.Key, pair.Value);
            }

            EzyObject data = dataBuilder.build();

            appProxy.send(Commands.SERVER_TO_CLIENT, data);
        }

        private void SendServerToClients(List<string> recipients, string command, List<KeyValuePair<string, object>> additionalParams)
        {
            var recipientsArray = EzyEntityFactory.newArrayBuilder();
            foreach (string recipient in recipients)
            {
                recipientsArray.append(recipient);
            }

            var dataBuilder = EzyEntityFactory.newObjectBuilder()
                .append("recipients", recipientsArray.build())
                .append("command", command);

            foreach (var pair in additionalParams)
            {
                dataBuilder.append(pair.Key, pair.Value);
            }

            EzyObject data = dataBuilder.build();

            appProxy.send(Commands.SERVER_TO_CLIENTS, data);
        }

        #endregion
    }
}

