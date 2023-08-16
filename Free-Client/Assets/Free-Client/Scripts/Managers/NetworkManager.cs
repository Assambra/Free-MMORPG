using System;
using System.Collections.Generic;
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

public class NetworkManager : MonoBehaviour
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
    private EzySocketProxyManager ezySocketProxyManager;
    private EzySocketProxy socketProxy;
    private EzyAppProxy appProxy;
    private List<Tuple<String, Object>> handlers = new();

    private string currentZone = "";
    private string currentApp = "";

    private bool connected = false;
    private bool nowProcessEvents = false;

    private string email;
    private string username;
    private string password;
    private string usernameOrEMail;

    private bool createAccount = false;
    private bool forgotPassword = false;
    private bool forgotUsername = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        EzyUnityLoggerFactory.getLogger<EzyUnityLogger>();
    }

    private void Update()
    {
        if(nowProcessEvents)
            EzyClients.getInstance().getClient(currentZone).processEvents();
    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy");

        if (UIClientLog != null)
            UIClientLog.ClearLog();
        
        foreach (Tuple<String, Object> tuple in handlers)
        {
            appProxy.unbind(tuple.Item1, tuple.Item2);
        }
    }

    public bool Connected()
    {
        return connected;
    }

    private void on<T>(String cmd, EzyAppProxyDataHandler<T> handler)
    {
        handlers.Add(
            new Tuple<String, Object>(cmd, appProxy.on(cmd, handler))
        );
    }

    public void Disconnect()
    {
        nowProcessEvents = false;

        if (connected)
            EzyClients.getInstance().getClient(currentZone).disconnect();

        connected = false;
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
            socketProxy.onUdpHandshake<Object>(OnUdpHandshake);
        }

        AddServerEvents();

        socketProxy.connect();
        connected = true;

        Debug.Log("Login: username = " + username + ", password = " + password);
    }

    private void CreateSocketProxy()
    {
        ezySocketProxyManager = EzySocketProxyManager.getInstance();
        if (!ezySocketProxyManager.hasInited())
            ezySocketProxyManager.init();

        socketProxy = ezySocketProxyManager.getSocketProxy(currentZone);
        if (socketProxy.getClient() == null)
        {
            Debug.Log("Creating Client");
            var config = EzyClientConfig.builder().clientName(currentZone).zoneName(currentZone).build();
            EzyClientFactory.getInstance().getOrCreateClient(config, useUdp);
        }
        appProxy = socketProxy.getAppProxy(currentApp, true);

        nowProcessEvents = true;
    }

    private void AddServerEvents()
    {
        socketProxy.onLoginSuccess<Object>(OnLoginSucess);
        socketProxy.onAppAccessed<Object>(OnAppAccessed);
        
        // Account
        on<EzyObject>(Commands.CREATE_ACCOUNT, OnCreateAccountResponse);
        on<EzyObject>(Commands.FORGOT_PASSWORD, OnForgotPasswordResponse);
        on<EzyObject>(Commands.FORGOT_USERNAME, OnForgotUsernameResponse);

        // Game
        on<EzyArray>(Commands.CHARACTER_LIST, OnCharacterListResponse);
        on<EzyObject>(Commands.CREATE_CHARACTER, OnCreateCreateCharacterResponse);
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

    #endregion

    #region SERVER RESPONSE

    private void OnUdpHandshake(EzySocketProxy proxy, Object data)
    {
        Debug.Log("OnUdpHandshake");
        socketProxy.send(new EzyAppAccessRequest(currentApp));
    }

    private void OnAppAccessed(EzyAppProxy proxy, Object data)
    {
        if ("free-game-server" == currentApp)
        {
            GetCharacterList();
        }
        
        Debug.Log("App access successfully");

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
        if(data.isEmpty())
            GameManager.Instance.ChangeScene(Scenes.CreateCharacter);
        else
        {
            GameManager.Instance.ChangeScene(Scenes.SelectCharacter);
            
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

                GameManager.Instance.characterInfos.Add(characterInfo);
            }
        }
    }

    private void OnCreateCreateCharacterResponse(EzyAppProxy proxy, EzyObject data)
    {
        string result = data.get<string>("result");

        switch (result)
        {
            case "success":
                Debug.Log("Character successful created");
                break;
            case "charactername_already_in_use":
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


    #endregion
}
