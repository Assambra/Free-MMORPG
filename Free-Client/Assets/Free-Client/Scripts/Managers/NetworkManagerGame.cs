using com.tvd12.ezyfoxserver.client;
using com.tvd12.ezyfoxserver.client.entity;
using com.tvd12.ezyfoxserver.client.factory;
using com.tvd12.ezyfoxserver.client.request;
using com.tvd12.ezyfoxserver.client.support;
using com.tvd12.ezyfoxserver.client.unity;
using UnityEngine;
using Object = System.Object;

public class NetworkManagerGame : EzyDefaultController
{
    
    public static NetworkManagerGame Instance { get; private set; }

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
        base.OnEnable();

        AddHandler<EzyArray>(Commands.CHARACTER_LIST, OnCharacterListResponse);
        AddHandler<EzyObject>(Commands.CREATE_CHARACTER, OnCreateCreateCharacterResponse);
        AddHandler<EzyArray>(Commands.PLAY, OnPlayResponse);
        AddHandler<EzyObject>(Commands.CHARACTER_SPAWNED, OnCharacterSpawned);
        AddHandler<EzyObject>(Commands.CHARACTER_DESPAWNED, OnCharacterDespawned);
        AddHandler<EzyArray>(Commands.SYNC_POSITION, OnPlayerSyncPosition);
    }

    private void Update()
    {
        EzyClients.getInstance()
            .getClient(socketConfigVariable.Value.ZoneName)
            .processEvents();
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

    public void Login(string username, string password)
    {
        socketProxy.onLoginSuccess<Object>(OnLoginSucess);
        socketProxy.onLoginError<Object>(OnLoginError);
        socketProxy.onAppAccessed<Object>(OnAppAccessed);

        socketProxy.setLoginUsername(username);
        socketProxy.setLoginPassword(password);

        socketProxy.setUrl(socketConfigVariable.Value.TcpUrl);
        socketProxy.setUdpPort(socketConfigVariable.Value.UdpPort);
        socketProxy.setDefaultAppName(socketConfigVariable.Value.AppName);
        
        if (socketConfigVariable.Value.UdpUsage)
            socketProxy.onUdpHandshake<Object>(OnUdpHandshake);
       

        socketProxy.connect();
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

        appProxy.udpSend(Commands.PLAYER_INPUT, data);
    }

    #endregion

    #region SERVER RESPONSE

    private void OnLoginError(EzySocketProxy proxy, Object data)
    {
        Debug.Log("OnLoginError");
        //Todo capture the error and handle
    }

    private void OnUdpHandshake(EzySocketProxy proxy, Object data)
    {
        Debug.Log("OnUdpHandshake");
        socketProxy.send(new EzyAppAccessRequest(socketConfigVariable.Value.AppName));
    }

    private void OnAppAccessed(EzyAppProxy proxy, Object data)
    {
        Debug.Log("Game: App access successfully");

        GameManager.Instance.CharacterInfos.Clear();

        GetCharacterList();
    }

    private void OnLoginSucess(EzySocketProxy proxy, Object data)
    {
        Debug.Log("OnLoginSucess");
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
