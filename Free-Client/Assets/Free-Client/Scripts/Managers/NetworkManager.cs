using Assambra.FreeClient.Constants;
using Assambra.FreeClient.Helper;
using Assambra.FreeClient.UserInterface;
using Assambra.FreeClient.UserInterface.PopupSystem.Enum;
using Assambra.FreeClient.UserInterface.PopupSystem.Manager;
using Assambra.FreeClient.UserInterface.PopupSystem.Popup;
using Assambra.GameFramework.GameManager;
using com.tvd12.ezyfoxserver.client;
using com.tvd12.ezyfoxserver.client.constant;
using com.tvd12.ezyfoxserver.client.entity;
using com.tvd12.ezyfoxserver.client.factory;
using com.tvd12.ezyfoxserver.client.request;
using com.tvd12.ezyfoxserver.client.support;
using com.tvd12.ezyfoxserver.client.unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace Assambra.FreeClient
{
    public class NetworkManager : EzyAbstractController
    {
        public static NetworkManager Instance {  get; private set; }

        [field: SerializeField] public LoginState LoginState { get; set; }

        [SerializeField] private EzySocketConfig _socketConfig;
        [SerializeField] private string _guestPassword = "Assambra";

        private bool _isInitialized = false;
        private LoginState _lastLoginState;
        private bool _isInitialChangeSceneAfterLoginDone;
        private bool _despawnInProgress;
        

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;

            LoginState = LoginState.Lobby;
            _lastLoginState = LoginState;
        }

        private new void OnEnable()
        {
            base.OnEnable();

            AccountAppHandler();
            Login(CreateGuestName(), _guestPassword);
        }

        private void OnDisable()
        {
            Disconnect();
            UnbindSocketHandlers();
            UnbindAppHandlers();
        }

        private void Update()
        {
            if(_lastLoginState != LoginState)
            {
                if(LoginState == LoginState.Lobby)
                {
                    _isInitialChangeSceneAfterLoginDone = false;
                    Disconnect();
                    UnbindAppHandlers();
                    AccountAppHandler();
                    Login(CreateGuestName(), _guestPassword);
                }
                else
                {
                    Disconnect();
                    UnbindAppHandlers();
                    GameAppHandler();
                }

                _lastLoginState = LoginState;
            }

            EzyClients.getInstance()
                .getClient(_socketConfig.ZoneName)
                .processEvents();
        }

        private void AccountAppHandler()
        {
            AddHandler<EzyObject>(Commands.CREATE_ACCOUNT, OnCreateUserResponse);
            AddHandler<EzyObject>(Commands.ACTIVATE_ACCOUNT, OnActivateUserResponse);
            AddHandler<EzyObject>(Commands.RESEND_ACTIVATION_MAIL, OnResendActivationMail);
            AddHandler<EzyObject>(Commands.FORGOT_PASSWORD, OnForgotPasswordResponse);
            AddHandler<EzyObject>(Commands.FORGOT_USERNAME, OnForgotUsernameResponse);
        }

        private void GameAppHandler()
        {
            AddHandler<EzyObject>(Commands.CHECK, OnCheckResponse);
            AddHandler<EzyArray>(Commands.CHARACTER_LIST, ReceiveCharacterList);
            AddHandler<EzyObject>(Commands.CREATE_CHARACTER, OnCreateCharacterResponse);
            AddHandler<EzyObject>(Commands.PLAYER_SPAWN, ReceivePlayerSpawn);
            AddHandler<EzyObject>(Commands.PLAYER_DESPAWN, ReceivePlayerDespawn);
            AddHandler<EzyObject>(Commands.UPDATE_ENTITY_POSITION, ReceiveUpdateEntityPosition);
        }

        protected override EzySocketConfig GetSocketConfig()
        {
            return _socketConfig;
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
        }

        public void Login(string username, string password)
        {
            if(!_isInitialized)
            {   
                socketProxy.onLoginSuccess<Object>(OnLoginSucess);
                socketProxy.onLoginError<Object>(OnLoginError);
                socketProxy.onAppAccessed<Object>(OnAppAccessed);

                socketProxy.setUrl(_socketConfig.TcpUrl);
                socketProxy.setUdpPort(_socketConfig.UdpPort);
                socketProxy.setDefaultAppName(_socketConfig.AppName);

                if (_socketConfig.UdpUsage)
                {
                    socketProxy.setTransportType(EzyTransportType.UDP);
                    socketProxy.onUdpHandshake<Object>(OnUdpHandshake);
                }
                else
                    socketProxy.setTransportType(EzyTransportType.TCP);

                _isInitialized = true;
            }

            socketProxy.setLoginUsername(username);
            socketProxy.setLoginPassword(password);

            StartCoroutine(WaitForDisconnectIfConnected());
        }

        private IEnumerator WaitForDisconnectIfConnected()
        {
            if (Connected())
            {
                yield return new WaitUntil(() => !Connected());
            }
            socketProxy.connect();
        }



        #region MASTER SERVER REQUESTS

        #region ACCOUNT

        public void CreateAccount(string email, string username, string password)
        {
            EzyObject data = EzyEntityFactory
            .newObjectBuilder()
            .append("email", email)
            .append("username", username)
            .append("password", password)
            .build();

            appProxy.send(Commands.CREATE_ACCOUNT, data, _socketConfig.EnableSSL);
        }

        public void ActivateAccount(string activationcode)
        {
            EzyObject data = EzyEntityFactory
            .newObjectBuilder()
            .append("activationCode", activationcode)
            .build();

            appProxy.send(Commands.ACTIVATE_ACCOUNT, data);
        }

        public void ResendActivationCodeEmail()
        {
            EzyObject data = EzyEntityFactory
                .newObjectBuilder()
                .build();

            appProxy.send(Commands.RESEND_ACTIVATION_MAIL, data);
        }

        public void ForgotPassword(string usernameOrEmail)
        {
            EzyObject data = EzyEntityFactory
            .newObjectBuilder()
            .append("usernameOrEMail", usernameOrEmail)
            .build();

            appProxy.send(Commands.FORGOT_PASSWORD, data);
        }

        public void ForgotUsername(string email)
        {
            EzyObject data = EzyEntityFactory
                .newObjectBuilder()
                .append("email", email)
                .build();
            appProxy.send(Commands.FORGOT_USERNAME, data);
        }

        #endregion

        #region GAME

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

        public void PlayRequest(long playerId)
        {
            EzyObject data = EzyEntityFactory
                .newObjectBuilder()
                .append("playerId", playerId)
                .build();

            appProxy.send(Commands.PLAY, data);
        }

        #endregion

        #endregion

        #region MASTER SERVER RESPONSE

        #region GENERAL

        private void OnLoginSucess(EzySocketProxy proxy, Object data)
        {
            Debug.Log("OnLoginSucess");
        }

        private void OnLoginError(EzySocketProxy proxy, Object data)
        {
            string error = data.ToString();
            if (error.Contains("invalid password") || error.Contains("invalid user name"))
            {
                ErrorPopup("Username or password is incorrect. Please check your entries and try again.");

                LoginState = LoginState.Lobby;
                Login(CreateGuestName(), _guestPassword);
            } 
        }

        private void OnUdpHandshake(EzySocketProxy proxy, Object data)
        {
            Debug.Log("OnUdpHandshake");
            socketProxy.send(new EzyAppAccessRequest(_socketConfig.AppName));
        }

        private void OnAppAccessed(EzyAppProxy proxy, Object data)
        {
            Debug.Log("App access successfully");

            if (LoginState == LoginState.Game)
                Check();
        }

        #endregion

        #region ACCOUNT

        private void OnCreateUserResponse(EzyAppProxy proxy, EzyObject data)
        {
            string result = data.get<string>("result");

            switch (result)
            {
                case "successful":
                    InfoPopupCreateAccount("Account successfully created");
                    break;
                case "email_already_registered":
                    ErrorPopup("E-Mail already registered, please use the Forgot password function");
                    break;
                case "username_already_in_use":
                    ErrorPopup("Username are not allowed");
                    break;
                case "username_are_not_allowed":
                    ErrorPopup("Username are not allowed");
                    break;
                default:
                    Debug.LogError("Create Account: Unknown message");
                    break;
            }

            UICreateAccount uICreateAccount = GameObject.FindObjectOfType<UICreateAccount>();
            if (uICreateAccount != null)
            {
                uICreateAccount.ButtonCreate.interactable = true;
                uICreateAccount.ButtonForgotData.interactable = true;
                uICreateAccount.ButtonBack.interactable = true;
            }
            else
                Debug.LogError("UICreateAccount not found!");
        }

        private void OnActivateUserResponse(EzyAppProxy proxy, EzyObject data)
        {
            string result = data.get<string>("result");
            switch (result)
            {
                case "successful":
                    InfoPopupAccountActivation("Your account has been successfully activated");
                    break;
                case "wrong_activation_code":
                    ErrorPopup("Wrong activation code");
                    break;
            }
        }

        private void OnResendActivationMail(EzyAppProxy proxy, EzyObject data)
        {
            InfoPopup("Your account activation code has been sent again to your email address.");
        }

        private void OnForgotPasswordResponse(EzyAppProxy proxy, EzyObject data)
        {
            string result = data.get<string>("result");

            switch (result)
            {
                case "successful":
                    InfoPopup("Your new password has been sent to your registered e-mail address");
                    break;
                case "no_account":
                    InfoPopup("No Account found for given username or email address");
                    break;
                default:
                    Debug.LogError("Forgot Password: Unknown result: " + result);
                    break;
            }

            UIForgotData uIForgotData = GameObject.FindObjectOfType<UIForgotData>();
            if (uIForgotData != null)
            {
                uIForgotData.ButtonSendPassword.interactable = true;
                uIForgotData.ButtonBackPassword.interactable = true;
                uIForgotData.ButtonTabUsername.interactable = true;
            }
            else
                Debug.LogError("UIForgotData not found!");
        }

        private void OnForgotUsernameResponse(EzyAppProxy proxy, EzyObject data)
        {
            string result = data.get<string>("result");
            switch (result)
            {
                case "successful":
                    InfoPopup("Your username has been sent to your email address");
                    break;
                case "not_found":
                    InfoPopup("This e-mail address are not registered");
                    break;
                default:
                    Debug.LogError("Forgot Username: Unknown result: " + result);
                    break;
            }

            UIForgotData uIForgotData = GameObject.FindObjectOfType<UIForgotData>();
            if (uIForgotData != null)
            {
                uIForgotData.ButtonSendUsername.interactable = true;
                uIForgotData.ButtonBackUsername.interactable = true;
                uIForgotData.ButtonTabPassword.interactable = true;
            }
            else
                Debug.LogError("UIForgotData not found!");
        }

        #endregion

        #region GAME

        private void OnCheckResponse(EzyAppProxy proxy, EzyObject data)
        {
            string result = data.get<string>("result");

            switch (result)
            {
                case "ok":
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
            {
                if (!_isInitialChangeSceneAfterLoginDone)
                {
                    GameManager.Instance.ChangeScene(Scenes.CreateCharacter);
                    _isInitialChangeSceneAfterLoginDone = true;
                }
                else
                    Debug.LogError("Unexpected empty character list");
            }
            else
            {
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

                if (!_isInitialChangeSceneAfterLoginDone)
                {
                    GameManager.Instance.ChangeScene(Scenes.SelectCharacter);
                    _isInitialChangeSceneAfterLoginDone = true;
                }
            }
        }

        private void OnCreateCharacterResponse(EzyAppProxy proxy, EzyObject data)
        {
            string result = data.get<string>("result");
            long characterId = data.get<long>("id");

            switch (result)
            {
                case "successfully":
                    Debug.Log("successfully");
                    InfoPopup("Character created successfully");
                    GetCharacterList();
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

        #endregion

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

        public void SendPlayerJump(long id, string room)
        {
            SendClientToServer(room, "playerJump", new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("id", id),
                new KeyValuePair<string, object>("room", room)
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

        private string CreateGuestName()
        {
            return "Guest#" + RandomString.GetNumericString(1000001);
        }

        #region POPUP

        private void InfoPopup(string info)
        {
            string title = "Error";

            PopupManager.Instance.ShowInfoPopup<InfoPopup>(PopupType.Info, title, info);
        }

        private void InfoPopupCreateAccount(string info)
        {
            string title = "Info";

            PopupManager.Instance.ShowInfoPopup<InfoPopup>(
                PopupType.Info,
                title,
                info,
                () => OnInfoPopupCreateAccountHandler()
            );
        }

        private void OnInfoPopupCreateAccountHandler()
        {
            GameManager.Instance.ChangeScene(Scenes.Login);
            Disconnect();
        }

        private void InfoPopupAccountActivation(string info)
        {
            string title = "Info";

            PopupManager.Instance.ShowInfoPopup<InfoPopup>(
                PopupType.Info,
                title,
                info,
                () => OnInfoPopupAccountActivationHandler()
            );
        }

        private void OnInfoPopupAccountActivationHandler()
        {
            if (NetworkManager.Instance.Connected())
                NetworkManager.Instance.GetCharacterList();
            else
                GameManager.Instance.ChangeScene(Scenes.Login);
        }

        private void ErrorPopup(string error)
        {
            string title = "Error";

            PopupManager.Instance.ShowInfoPopup<InfoPopup>(PopupType.Error, title, error);
        }

        #endregion
    }
}

