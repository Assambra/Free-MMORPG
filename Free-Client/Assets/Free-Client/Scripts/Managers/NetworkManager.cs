using com.tvd12.ezyfoxserver.client;
using com.tvd12.ezyfoxserver.client.config;
using com.tvd12.ezyfoxserver.client.constant;
using com.tvd12.ezyfoxserver.client.entity;
using com.tvd12.ezyfoxserver.client.factory;
using com.tvd12.ezyfoxserver.client.request;
using com.tvd12.ezyfoxserver.client.support;
using com.tvd12.ezyfoxserver.client.unity;
using UnityEngine;
using Object = System.Object;

public class NetworkManager : EzyDefaultController
{
    [Header("Server Settings")]
    
    [SerializeField] private string host = "127.0.0.1";
    [SerializeField] private int tcpPort = 3005;
    [SerializeField] private int udpPort = 2611;
    [SerializeField] private bool useUdp = false;
    [SerializeField] private string guestPassword = "Assambra";
    [Header("Account Server Zone/App Settings")]
    [SerializeField] private string accountZoneName = "free-account-server";
    [SerializeField] private string accountAppName = "free-account-server";
    [Header("Game Server Zone/App Settings")]
    [SerializeField] private string gameZoneName = "free-game-server";
    [SerializeField] private string gameAppName = "free-game-server";
    

    [HideInInspector] public UIClientLog UIClientLog;
    public static NetworkManager Instance { get; private set; }

    // private variables

    private string currentZone = "";
    private string currentApp = "";

    private bool nowProcessEvents = false;

    private string email;
    private string username;
    private string password;
    private string usernameOrEMail;

    private bool createAccount = false;
    private bool forgotPassword = false;
    private bool forgotUsername = false;

    private bool characterListReseived;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private new void OnEnable()
    {
        // empty override, dont delete we use a other way as the base class
        // we canot create the socket OnEnabled because we have different server for account/game
    }

    private void Update()
    {
        if(nowProcessEvents)
        {
            EzyClients.getInstance()
                .getClient(currentZone)
                .processEvents();
        }   
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
        if(Connected())
            base.Disconnect();

        characterListReseived = false;
    }

    #region REQUESTS

    public void Login(string username, string password, bool toGameServer)
    {
        if(toGameServer)
        {
            currentZone = gameZoneName;
            currentApp = gameAppName;
        }
        else 
        {
            currentZone = accountZoneName;
            currentApp = accountAppName;
        }

        CreateSocketProxy();
        AddEventHandlers();

        socketProxy.setHost(host);
        socketProxy.setTcpPort(tcpPort);
        socketProxy.setUdpPort(udpPort);
        
        socketProxy.setLoginUsername(username);
        socketProxy.setLoginPassword(password);

        socketProxy.setDefaultAppName(currentApp);

        if (!useUdp)
            socketProxy.setTransportType(EzyTransportType.TCP);
        else
        {
            socketProxy.setTransportType(EzyTransportType.UDP);
        }

        socketProxy.connect();

        Debug.Log("Login: username = " + username + ", password = " + password);
    }

    private void CreateSocketProxy()
    {
        LOGGER.debug("OnEnable");
        
        var socketProxyManager = EzySocketProxyManager.getInstance();
        if (!socketProxyManager.hasInited())
        {
            socketProxyManager.init();
        }
        socketProxy = socketProxyManager.getSocketProxy(
            currentZone
        );
        if (socketProxy.getClient() == null)
        {
            LOGGER.debug("Creating ezyClient");
            var config = EzyClientConfig.builder()
                .clientName(currentZone)
                .zoneName(currentZone)
                .build();
            EzyClientFactory
                .getInstance()
                .getOrCreateClient(
                    config,
                    useUdp
                );
        }
        appProxy = socketProxy.getAppProxy(
            currentApp,
            true
        );

        nowProcessEvents = true;
    }

    private void AddEventHandlers()
    {
        socketProxy.onLoginSuccess<Object>(OnLoginSucess);
        socketProxy.onAppAccessed<Object>(OnAppAccessed);
        socketProxy.onUdpHandshake<Object>(OnUdpHandshake);

        // Account
        AddHandler<EzyObject>(Commands.CREATE_ACCOUNT, OnCreateAccountResponse);
        AddHandler<EzyObject>(Commands.FORGOT_PASSWORD, OnForgotPasswordResponse);
        AddHandler<EzyObject>(Commands.FORGOT_USERNAME, OnForgotUsernameResponse);

        // Game
        AddHandler<EzyArray>(Commands.CHARACTER_LIST, OnCharacterListResponse);
        AddHandler<EzyObject>(Commands.CREATE_CHARACTER, OnCreateCreateCharacterResponse);
        AddHandler<EzyArray>(Commands.PLAY, OnPlayResponse);
        AddHandler<EzyObject>(Commands.CHARACTER_SPAWNED, OnCharacterSpawned);
        AddHandler<EzyObject>(Commands.CHARACTER_DESPAWNED, OnCharacterDespawned);
        AddHandler<EzyArray>(Commands.SYNC_POSITION, OnPlayerSyncPosition);
    }

    public void CreateAccount(string email, string username, string password)
    {
        createAccount = true;

        Login(CreateGuestName(), guestPassword, false);

        this.email = email;
        this.username = username;
        this.password = password;
    }

    public void ForgotPassword(string usernameoremail)
    {
        forgotPassword = true;

        Login(CreateGuestName(), guestPassword, false);

        this.usernameOrEMail = usernameoremail;
    }

    public void ForgotUsername(string email)
    {
        forgotUsername = true;

        Login(CreateGuestName(), guestPassword, false);

        this.email = email;
    }

    private string CreateGuestName()
    {
        return "Guest#" + RandomString.GetNumericString(1000001);
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

    public void PlayRequest(long characterId)
    {
        EzyObject data = EzyEntityFactory
            .newObjectBuilder()
            .append("characterId", characterId)
            .build();

        appProxy.send(Commands.PLAY, data);
    }

    public void SendPlayerInput(int time, bool[] inputs, Quaternion rotation)
    {
        EzyObject data = EzyEntityFactory
            .newObjectBuilder()
            .append("t", time)
            .append("i", inputs)
            .append(
                "r",
                EzyEntityFactory.newArrayBuilder()
                    .append(rotation.eulerAngles.x)
                    .append(rotation.eulerAngles.y)
                    .append(rotation.eulerAngles.z)
                    .build()
            )
            .build();

        appProxy.send(Commands.PLAYER_INPUT, data);
    }

    #endregion

    #region SERVER RESPONSE

    private void OnUdpHandshake(EzySocketProxy proxy, Object data)
    {
        Debug.Log("OnUdpHandshake");
        socketProxy.send(new EzyAppAccessRequest(currentApp));
    }

    private void OnAppAccessed(EzyAppProxy proxy, Object data)
    {
        Debug.Log("App access successfully");

        if ("free-game-server" == currentApp)
        {
            GameManager.Instance.CharacterInfos.Clear();

            GetCharacterList();
        }

        if (createAccount)
        {
            createAccount = false;

            EzyObject accountdata = EzyEntityFactory
            .newObjectBuilder()
            .append("email", email)
            .append("username", username)
            .append("password", password)
            .build();

            appProxy.send(Commands.CREATE_ACCOUNT, accountdata);

            email = "";
            username = "";
            password = "";
        }

        if (forgotPassword)
        {
            forgotPassword = false;

            EzyObject usernameoremail = EzyEntityFactory
            .newObjectBuilder()
            .append("usernameOrEMail", usernameOrEMail)
            .build();

            appProxy.send(Commands.FORGOT_PASSWORD, usernameoremail);

            usernameOrEMail = "";
        }

        if (forgotUsername)
        {
            forgotUsername = false;

            EzyObject username = EzyEntityFactory
                .newObjectBuilder()
                .append("email", email)
                .build();
            appProxy.send(Commands.FORGOT_USERNAME, username);

            email = "";
        }
    }

    private void OnLoginSucess(EzySocketProxy proxy, Object data)
    {
        Debug.Log("OnLoginSucess");
    }

    private void OnCreateAccountResponse(EzyAppProxy proxy, EzyObject data)
    {
        UICreateAccount uICreateAccount = GameObject.FindAnyObjectByType<UICreateAccount>();
        if (uICreateAccount == null)
            Debug.LogError("UICreateAccount not found!");

        string result = data.get<string>("result");

        switch(result)
        {
            case "successfully":
                Debug.Log("Account successfully created");
                UIClientLog.ServerLogMessageSuccess("Account successfully created");
                break;
            case "email_already_registered":
                Debug.Log("E-Mail already registered");
                UIClientLog.ServerLogMessageError("E-Mail already registered, please use the Forgot password function");      
                break;
            case "username_already_in_use":
                Debug.Log("Username already in use");
                UIClientLog.ServerLogMessageError("Username are not allowed");
                break;
            case "username_are_not_allowed":
                Debug.Log("Username not allowed");
                UIClientLog.ServerLogMessageError("Username are not allowed");
                break;
            default:
                Debug.LogError("Create Account: Unknown message");
                break;
        }
        // Todo Disconnect from server until we dont have a solution to communicate with the server without login
        Disconnect();

        uICreateAccount.buttonCreate.interactable = true;
        uICreateAccount.buttonForgotData.interactable = true;
        uICreateAccount.buttonBack.interactable = true;
    }

    private void OnForgotPasswordResponse(EzyAppProxy proxy, EzyObject data)
    {
        UIForgotData uIForgotData = GameObject.FindObjectOfType<UIForgotData>();
        if(uIForgotData == null)
            Debug.LogError("UIForgotData not found!");

        string result = data.get<string>("result");
        string pwd = data.get<string>("password");

        switch (result)
        {
            case "no_account":
                UIClientLog.ServerLogMessageError("No Account found for given username or email address");
                break;
            case "sending_password":
                UIClientLog.ServerLogMessageSuccess("Your new password is: " + pwd);
                break;
            case "sending_email":
                UIClientLog.ServerLogMessageSuccess("Your new password has been sent to your registered e-mail address");
                break;
            default:
                Debug.LogError("Forgot Password: Unknown result: " + result);
                break;
        }
        // Todo Disconnect from server until we dont have a solution to communicate with the server without login
        Disconnect();

        uIForgotData.buttonSendPassword.interactable = true;
        uIForgotData.buttonBack.interactable = true;
        uIForgotData.buttonTabUsername.interactable = true;
    }

    private void OnForgotUsernameResponse(EzyAppProxy proxy, EzyObject data)
    {
        UIForgotData uIForgotData = GameObject.FindObjectOfType<UIForgotData>();
        if (uIForgotData == null)
            Debug.LogError("UIForgotData not found!");

        string result = data.get<string>("result");
        string username = data.get<string>("username");

        if (result == "success")
        {
            if(username == "")
                UIClientLog.ServerLogMessageSuccess("Your username has been sent to your email address");
            else
                UIClientLog.ServerLogMessageSuccess("Your username is: " + username);
        }
        if (result == "not_found")
            UIClientLog.ServerLogMessageError("This e-mail address are not registered");

        // Todo Disconnect from server until we dont have a solution to communicate with the server without login
        Disconnect();

        uIForgotData.buttonSendUsername.interactable = true;
        uIForgotData.buttonBack.interactable = true;
        uIForgotData.buttonTabPassword.interactable = true;
    }

    private void OnCharacterListResponse(EzyAppProxy proxy, EzyArray data)
    {
        if (data.isEmpty())
            GameManager.Instance.ChangeScene(Scenes.CreateCharacter);
        else
        {
            if (GameManager.Instance.CharacterInfos.Count > 0)
                GameManager.Instance.CharacterInfos.Clear();
            
            for (int i = 0; i < data.size(); i++)
            {
                EzyArray character = data.get<EzyArray>(i);

                CharacterInfo characterInfo = new CharacterInfo();
                characterInfo.id = character.get<long>(0);
                characterInfo.accountId = character.get<long>(1);
                characterInfo.name = character.get<string>(2);
                characterInfo.sex = character.get<string>(3);
                characterInfo.race = character.get<string>(4);
                characterInfo.model = character.get<string>(5);

                GameManager.Instance.CharacterInfos.Add(characterInfo);
            }

            if(!characterListReseived)
                GameManager.Instance.ChangeScene(Scenes.SelectCharacter);
        }
        characterListReseived = true;
    }

    private void OnCreateCreateCharacterResponse(EzyAppProxy proxy, EzyObject data)
    {
        string result = data.get<string>("result");
        long characterId = data.get<long>("characterId");

        switch (result)
        {
            case "successfully":
                Debug.Log("successfully");
                GameManager.Instance.CharacterCreatedAndReadyToPlay = true;
                GameManager.Instance.CharacterId = characterId;
                break;
            case "name_already_in_use":
                Debug.Log("Username already in use");
                break;
            case "max_allowed_characters":
                Debug.Log("You have reached the maximum number of characters");
                break;
            default:
                Debug.LogError("Create Account: Unknown message");
                break;
        }
    }

    private void OnPlayResponse(EzyAppProxy proxy, EzyArray data)
    {
        GameManager.Instance.ChangeScene(Scenes.World);

        Debug.Log("OnPlayResponse");

        for (int i = 0; i < data.size(); i++)
        {
            EzyObject item = data.get<EzyObject>(i);
            string accountUsername = item.get<string>("accountUsername");
            long roomId = item.get<long>("roomId");
            bool isLocalPlayer = item.get<bool>("isLocalPlayer");
            string characterName = item.get<string>("characterName");
            string characterModel = item.get<string>("characterModel");
            EzyArray position = item.get<EzyArray>("position");
            EzyArray rotation = item.get<EzyArray>("rotation");

            GameManager.Instance.CharacterList.Add(
                new Character(accountUsername, roomId, isLocalPlayer, characterName, characterModel,
                    new Vector3(position.get<float>(0), position.get<float>(1), position.get<float>(2)),
                    new Vector3(rotation.get<float>(0), rotation.get<float>(1), rotation.get<float>(2))));
        }

        foreach(Character c in GameManager.Instance.CharacterList)
        {
            c.SetPlayerGameObject(GameManager.Instance.SpawnPlayer(c));
        }
    }

    private void OnCharacterSpawned(EzyAppProxy proxy, EzyObject data)
    {
        string accountUsername = data.get<string>("accountUsername");
        long roomId = data.get<long>("roomId");
        bool isLocalPlayer = data.get<bool>("isLocalPlayer");
        string characterName = data.get<string>("characterName");
        string characterModel = data.get<string>("characterModel");
        EzyArray position = data.get<EzyArray>("position");
        EzyArray rotation = data.get<EzyArray>("rotation");

        Character character = new Character(accountUsername, roomId, isLocalPlayer, characterName, characterModel,
                    new Vector3(position.get<float>(0), position.get<float>(1), position.get<float>(2)),
                    new Vector3(rotation.get<float>(0), rotation.get<float>(1), rotation.get<float>(2)));

        GameManager.Instance.CharacterList.Add(character);

        character.SetPlayerGameObject(GameManager.Instance.SpawnPlayer(character));

        Debug.Log("Player spawned: " + accountUsername);
    }

    private void OnCharacterDespawned(EzyAppProxy proxy, EzyObject data)
    {
        string userName = data.get<string>("userName");

        Character characterToRemove = null;

        foreach(Character character in GameManager.Instance.CharacterList)
        {
            if (character.accountUsername == userName)
            {
                GameObject.Destroy(character.playerGameObject);
                
                characterToRemove = character;
                break;
            }
        }
        if(characterToRemove != null)
            GameManager.Instance.CharacterList.Remove(characterToRemove);
    }

    private void OnPlayerSyncPosition(EzyAppProxy proxy, EzyArray data)
    {
        string playerName = data.get<string>(0);
        EzyArray positionArray = data.get<EzyArray>(1);
        EzyArray rotationArray = data.get<EzyArray>(2);
        int time = data.get<int>(3);
        Vector3 position = new Vector3(
            positionArray.get<float>(0),
            positionArray.get<float>(1),
            positionArray.get<float>(2)
        );
        Vector3 rotation = new Vector3(
            rotationArray.get<float>(0),
            rotationArray.get<float>(1),
            rotationArray.get<float>(2)
        );

        //Debug.Log("SyncPosition for player: " + playerName + " reseive position: " + position + ", rotation: " + rotation + " time: " + time);

        foreach(Character character in GameManager.Instance.CharacterList)
        {
            if (playerName == character.accountUsername)
            {
                GameObject pgo = character.playerGameObject;

                Player player = pgo.GetComponent<Player>();
                player.PlayerController.Move(position);
                player.PlayerController.Rotate(rotation);

                if (time == 0 && player.IsLocalPlayer)
                    GameManager.Instance.CameraController.SetCameraPanAbsolutAngle(rotation.y);

                break;
            }
        }
    }

    #endregion
}
