using Assambra.GameFramework.GameManager;
using Assambra.FreeClient.Constants;
using Assambra.FreeClient.UserInterface;
using com.tvd12.ezyfoxserver.client;
using com.tvd12.ezyfoxserver.client.constant;
using com.tvd12.ezyfoxserver.client.entity;
using com.tvd12.ezyfoxserver.client.factory;
using com.tvd12.ezyfoxserver.client.request;
using com.tvd12.ezyfoxserver.client.support;
using com.tvd12.ezyfoxserver.client.unity;
using UnityEngine;
using Object = System.Object;
using System.Collections.Generic;
using System.Collections;
using Assambra.FreeClient.Helper;

namespace Assambra.FreeClient
{
    public class NetworkManagerGame : EzyAbstractController
    {
        public static NetworkManagerGame Instance { get; private set; }

        [SerializeField] EzySocketConfig socketConfig;

        private bool characterListReseived;
        private bool _despawnInProgress;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        private new void OnEnable()
        {
            base.OnEnable();

            AddHandler<EzyObject>(Commands.CHECK, OnCheckResponse);
            AddHandler<EzyArray>(Commands.CHARACTER_LIST, ReceiveCharacterList);
            AddHandler<EzyObject>(Commands.CREATE_CHARACTER, OnCreateCreateCharacterResponse);
            AddHandler<EzyObject>(Commands.PLAYER_SPAWN, ReceivePlayerSpawn);
            AddHandler<EzyObject>(Commands.PLAYER_DESPAWN, ReceivePlayerDespawn);
            AddHandler<EzyObject>(Commands.UPDATE_ENTITY_POSITION, ReceiveUpdateEntityPosition);
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

        public bool Connected()
        {
            if (socketProxy != null)
                return socketProxy.getClient().isConnected();
            else
                return false;
        }

        public new void Disconnect()
        {
            if (Connected())
                base.Disconnect();

            characterListReseived = false;
        }

        public void Login(string username, string password)
        {
            GameManager.Instance.Account = username;

            socketProxy.onLoginSuccess<Object>(OnLoginSucess);
            socketProxy.onLoginError<Object>(OnLoginError);
            socketProxy.onAppAccessed<Object>(OnAppAccessed);

            socketProxy.setLoginUsername(username);
            socketProxy.setLoginPassword(password);

            socketProxy.setUrl(socketConfig.TcpUrl);
            socketProxy.setUdpPort(socketConfig.UdpPort);
            socketProxy.setDefaultAppName(socketConfig.AppName);

            if (socketConfig.UdpUsage)
            {
                socketProxy.setTransportType(EzyTransportType.UDP);
                socketProxy.onUdpHandshake<Object>(OnUdpHandshake);
            }
            else
                socketProxy.setTransportType(EzyTransportType.TCP);

            socketProxy.connect();
        }

        #region MASTER SERVER REQUESTS

        public void Check()
        {
            appProxy.send(Commands.CHECK);
        }

        public void GetCharacterList()
        {
            appProxy.send(Commands.CHARACTER_LIST);
        }

        public void CreateCharacter(string name, string sex, string race, string model)
        {
            EzyObject characterdata = EzyEntityFactory
                .newObjectBuilder()
                .append("name", name)
                .append("sex", sex)
                .append("race", race)
                .append("model", model)
                .build();

            appProxy.send(Commands.CREATE_CHARACTER, characterdata);
        }

        private void OnCreateCreateCharacterResponse(EzyAppProxy proxy, EzyObject data)
        {
            string result = data.get<string>("result");
            long characterId = data.get<long>("characterId");

            switch (result)
            {
                case "successfully":
                    Debug.Log("successfully");
                    InformationPopup("Character created successfully");
                    GameManager.Instance.CharacterCreatedAndReadyToPlay = true;
                    GameManager.Instance.CharacterId = characterId;
                    break;
                case "name_already_in_use":
                    ErrorPopup("Username already in use");
                    break;
                case "max_allowed_characters":
                    ErrorPopup("You have reached the maximum number of characters");
                    break;
                default:
                    Debug.LogError("Create Account: Unknown message");
                    break;
            }
        }

        public void PlayRequest(long id)
        {
            EzyObject data = EzyEntityFactory
                .newObjectBuilder()
                .append("id", id)
                .build();

            appProxy.send(Commands.PLAY, data);
        }

        #endregion

        #region MASTER SERVER RESPONSE

        private void OnLoginSucess(EzySocketProxy proxy, Object data)
        {
            Debug.Log("OnLoginSucess");
        }

        private void OnLoginError(EzySocketProxy proxy, Object data)
        {
            string error = data.ToString();
            if (error.Contains("invalid password") || error.Contains("invalid user name"))
                ErrorPopup("Username or password is incorrect. Please check your entries and try again.");
        }

        private void OnUdpHandshake(EzySocketProxy proxy, Object data)
        {
            Debug.Log("OnUdpHandshake");
            socketProxy.send(new EzyAppAccessRequest(socketConfig.AppName));
        }

        private void OnAppAccessed(EzyAppProxy proxy, Object data)
        {
            Debug.Log("Game: App access successfully");

            Check();
        }

        private void OnCheckResponse(EzyAppProxy proxy, EzyObject data)
        {
            string result = data.get<string>("result");

            switch (result)
            {
                case "ok":
                    GameManager.Instance.CharacterInfos.Clear();
                    GetCharacterList();
                    break;
                case "need_activation":
                    GameManager.Instance.ChangeScene(Scenes.AccountActivation);
                    break;
            }
        }

        private void ReceiveCharacterList(EzyAppProxy proxy, EzyArray data)
        {
            if (data.isEmpty())
                GameManager.Instance.ChangeScene(Scenes.CreateCharacter);
            else
            {
                if (GameManager.Instance.CharacterInfos.Count > 0)
                    GameManager.Instance.CharacterInfos.Clear();

                for (int i = 0; i < data.size(); i++)
                {
                    EzyObject characterInfo = data.get<EzyObject>(i);

                    EntityModel characterInfoModel = new EntityModel(
                        characterInfo.get<long>("id"),
                        0,
                        characterInfo.get<string>("name"),
                        characterInfo.get<string>("model"),
                        Vector3.zero,
                        Quaternion.identity,
                        characterInfo.get<string>("sex"),
                        characterInfo.get<string>("race"),
                        characterInfo.get<string>("room")
                        );

                    GameManager.Instance.CharacterInfos.Add(characterInfoModel);
                }

                if (!characterListReseived)
                    GameManager.Instance.ChangeScene(Scenes.SelectCharacter);
            }
            characterListReseived = true;
        }

        #endregion

        #region SEND TO ROOM SERVER

        public void SendPlayerInput(long id, string room, Vector3 input)
        {
            EzyArray inputArray = EzyEntityFactory.newArrayBuilder()
                .append(input.x)
                .append(input.z)
                .build();

            SendClientToServer(room, "playerInput", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("id", id),
                new KeyValuePair<string, object>("room", room),
                new KeyValuePair<string, object>("input", inputArray)
            });
        }

        #endregion

        #region RECEIVE FROM ROOM SERVER 

        private void ReceivePlayerSpawn(EzyAppProxy proxy, EzyObject data)
        {
            EntityModel entityModel = new(
                data.get<long>("id"),
                data.get<int>("entityType"),
                data.get<string>("name"),
                data.get<string>("model"),
                new Vector3(
                    data.get<EzyArray>("position").get<float>(0),
                    data.get<EzyArray>("position").get<float>(1),
                    data.get<EzyArray>("position").get<float>(2)
                ),
                Quaternion.Euler(
                    data.get<EzyArray>("rotation").get<float>(0),
                    data.get<EzyArray>("rotation").get<float>(1),
                    data.get<EzyArray>("rotation").get<float>(2)
                ),
                data.get<string>("sex"),
                data.get<string>("race"),
                data.get<string>("room"),
                data.get<bool>("isLocalPlayer")
            );

            if (!string.IsNullOrEmpty(entityModel.Room))
            {
                Scenes scenes = GameManager.Instance.getScenesByName(entityModel.Room);

                GameManager.Instance.ChangeScene(scenes);
            }

            StartCoroutine(WaitUntilDespawnDone(entityModel));
        }

        private IEnumerator WaitUntilDespawnDone(EntityModel entityModel)
        {
            yield return new WaitUntil(() => _despawnInProgress == false);

            GameObject playerGameObject = GameManager.Instance.CreatePlayer(entityModel.Position, entityModel.Rotation);

            Player player = playerGameObject.GetComponent<Player>();
            PlayerController playerController = playerGameObject.GetComponent<PlayerController>();

            StartCoroutine(DelayToEnableCharacterController(playerController));

            if (playerController != null)
                playerController.Player = player;
            else
                Debug.LogError("PlayerController component not found on the playerGameObject.");

            if (player != null)
            {
                StartCoroutine(WaitForCharacterCreated(player, entityModel.Model));

                player.Initialize(entityModel, playerGameObject);

                if (!entityModel.IsLocalPlayer)
                {
                    player.NetworkTransform.IsActive = true;
                    player.NetworkTransform.Initialize(entityModel.Position, entityModel.Rotation);
                }
                else
                {
                    GameManager.Instance.CameraController.ChangeCameraPreset("GameCamera");
                    GameManager.Instance.CameraController.CameraTarget = playerGameObject;
                    GameManager.Instance.CameraController.Active = true;
                    player.PlayerController.IsActive = true;
                    player.NetworkTransform.IsActive = false;
                }

                GameManager.Instance.ClientEntities.Add(entityModel.Id, player);
            }
            else
            {
                Debug.LogError("Player component not found on the playerGameObject.");
            }
        }

        IEnumerator WaitForCharacterCreated(Player player, string model)
        {
            while (!player.Initialized && !player.IsAvatarCreated)
            {
                Debug.Log("WaitForCharacterCreated");
                yield return new WaitForSeconds(0.05f);
            }

            player.Animator = player.Avatar.GetComponent<Animator>();
            player.GetCapsuleCollider();
            UMAHelper.SetAvatarString(player.Avatar, model);
        }

        private IEnumerator DelayToEnableCharacterController(PlayerController playerController)
        {
            yield return new WaitForSeconds(0.5f);

            playerController.CharacterController.enabled = true;
        }

        private void ReceivePlayerDespawn(EzyAppProxy proxy, EzyObject data)
        {
            _despawnInProgress = true;

            long id = data.get<long>("id");
            //Debug.Log($"Receive PLAYER_DESPAWN request for {id}");

            if (GameManager.Instance.ClientEntities.TryGetValue(id, out Entity entity))
            {
                if (entity is Player player)
                {
                    if (player.EntityModel.IsLocalPlayer)
                    {
                        GameManager.Instance.CameraController.Active = false;
                        GameManager.Instance.CameraController.CameraTarget = null;

                        foreach (KeyValuePair<long, Entity> e in GameManager.Instance.ClientEntities)
                        {
                            Destroy(e.Value.EntityModel.EntityGameObject);
                        }
                        GameManager.Instance.ClientEntities.Clear();
                    }
                    else
                    {
                        Destroy(player.EntityModel.EntityGameObject);
                        GameManager.Instance.ClientEntities.Remove(player.EntityModel.Id);
                    }
                }
            }

            _despawnInProgress = false;
        }

        private void ReceiveUpdateEntityPosition(EzyAppProxy proxy, EzyObject data)
        {
            long id = data.get<long>("id");
            EzyArray position = data.get<EzyArray>("position");
            EzyArray rotation = data.get<EzyArray>("rotation");

            Vector3 pos = new Vector3(position.get<float>(0), position.get<float>(1), position.get<float>(2));
            Vector3 rot = new Vector3(rotation.get<float>(0), rotation.get<float>(1), rotation.get<float>(2));

            if (GameManager.Instance.ClientEntities.TryGetValue(id, out Entity entity))
            {
                if (entity.EntityModel.Id == id)
                {
                    if (entity is Player player)
                    {
                        if (!player.EntityModel.IsLocalPlayer)
                        {
                            entity.NetworkTransform.UpdateTargetPosition(pos);
                            entity.NetworkTransform.UpdateTargetRotation(Quaternion.Euler(rot));
                        }
                    }
                }
            }
            //Debug.Log($"Receive UPDATE_ENTITY_POSITION request Id: {id} ");
        }

        #endregion

        #region CLIENT TO ROOM SERVER MESSAGE

        private void SendClientToServer(string room, string command, List<KeyValuePair<string, object>> additionalParams)
        {
            //Debug.Log("SendClientToServer");

            var dataBuilder = EzyEntityFactory.newObjectBuilder()
                .append("room", room)
                .append("command", command);

            foreach (var pair in additionalParams)
            {
                dataBuilder.append(pair.Key, pair.Value);
            }

            EzyObject data = dataBuilder.build();

            appProxy.send(Commands.CLIENT_TO_SERVER, data);
        }

        #endregion

        #region POPUP

        private void InformationPopup(string information)
        {
            string title = "Info";
            string info = information;

            InformationPopup popup = PopupManager.Instance.ShowInformationPopup<InformationPopup>(title, info, null);

            popup.Setup(
                title,
                info,
                () => { popup.Destroy(); }
            );
        }

        private void ErrorPopup(string error)
        {
            string title = "Error";
            string info = error;

            ErrorPopup popup = PopupManager.Instance.ShowErrorPopup<ErrorPopup>(title, info, null);

            popup.Setup(
                title,
                info,
                () => { popup.Destroy(); }
            );
        }

        #endregion
    }
}
