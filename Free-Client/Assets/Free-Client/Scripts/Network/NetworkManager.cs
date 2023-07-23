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
    [SerializeField] private string zoneName = "free-server";
    [SerializeField] private string appName = "free-server";
    [SerializeField] private string host = "127.0.0.1";
    [SerializeField] private int tcpPort = 3005;
    [SerializeField] private int udpPort = 2611;
    [SerializeField] private bool useUdp = false;

    [SerializeField] private string email;
    [SerializeField] private string username;
    [SerializeField] private string password;

    public bool CreateAccountError = false;
    
    public UIClientLog UIClientLog;

    private SceneHandler sceneHandler;

    private EzySocketProxy socketProxy;
    private EzyAppProxy appProxy;

    private List<Tuple<String, Object>> handlers = new();

    private bool createAccount = false;

    private void Awake()
    {
        sceneHandler = GameObject.FindObjectOfType<SceneHandler>();
    }

    private void Start()
    {
        EzyUnityLoggerFactory.getLogger<EzyUnityLogger>();
        Debug.Log("Start");

        var socketProxyManager = EzySocketProxyManager.getInstance();
        if (!socketProxyManager.hasInited())
            socketProxyManager.init();

        socketProxy = socketProxyManager.getSocketProxy(zoneName);
        if (socketProxy.getClient() == null)
        {
            Debug.Log("Creating Client");
            var config = EzyClientConfig.builder().clientName(zoneName).zoneName(zoneName).build();
            EzyClientFactory.getInstance().getOrCreateClient(config, useUdp);
        }
        appProxy = socketProxy.getAppProxy(appName, true);
    }

    private void Update()
    {
        EzyClients.getInstance().getClient(zoneName).processEvents();
    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
        foreach (Tuple<String, Object> tuple in handlers)
        {
            appProxy.unbind(tuple.Item1, tuple.Item2);
        }
    }

    private void on<T>(String cmd, EzyAppProxyDataHandler<T> handler)
    {
        handlers.Add(
            new Tuple<String, Object>(cmd, appProxy.on(cmd, handler))
        );
    }

    public void Login(string username, string password)
    {
        this.username = username;
        this.password = password;

        Debug.Log("Login username = " + username + ", password = " + password);
        Debug.Log("Socket clientName = " + socketProxy.getClient().getName());

        socketProxy.onLoginSuccess<Object>(OnLoginSucess);
        socketProxy.onAppAccessed<Object>(OnAppAccessed);

        socketProxy.setLoginUsername(username);
        socketProxy.setLoginPassword(password);

        socketProxy.setHost(host);
        socketProxy.setTcpPort(tcpPort);
        socketProxy.setUdpPort(udpPort);

        socketProxy.setDefaultAppName(appName);

        if (!useUdp)
        {
            socketProxy.setTransportType(EzyTransportType.TCP);
        }
        else
        {
            socketProxy.setTransportType(EzyTransportType.UDP);
            socketProxy.onUdpHandshake<Object>(OnUdpHandshake);
        }

        socketProxy.connect();

        on<EzyObject>(Commands.CREATE_ACCOUNT, OnCreateAccountResponse);
    }

    public void CreateAccount(string email, string username, string password)
    {
        // Todo only if(isConnected)
        Login("YoungMonkey", "YoungMonkey");

        this.email = email;
        this.username = username;
        this.password = password;

        createAccount = true;
    }

    public void ForgotPassword(string usernameoremail)
    {
        Debug.LogError("NetworkManager: ForgotPassword not implemented! Got request for: " + usernameoremail);
    }

    private void OnUdpHandshake(EzySocketProxy proxy, Object data)
    {
        Debug.Log("OnUdpHandshake");
        socketProxy.send(new EzyAppAccessRequest(appName));
    }

    private void OnLoginSucess(EzySocketProxy proxy, Object data)
    {
        Debug.Log("OnLoginSucess");
    }

    private void OnAppAccessed(EzyAppProxy proxy, Object data)
    {
        Debug.Log("App access successfully");

        if(createAccount)
        {
            EzyObject accountdata = EzyEntityFactory
            .newObjectBuilder()
            .append("email", email)
            .append("username", username)
            .append("password", password)
            .build();

            appProxy.send(Commands.CREATE_ACCOUNT, accountdata);

            createAccount = false;
        }
        //email = "";
        //username = "";
        //password = "";
    }

    #region SERVER RESPONSE

    private void OnCreateAccountResponse(EzyAppProxy proxy, EzyObject data)
    {
        string result = data.get<string>("result");

        switch(result)
        {
            case "successfully":
                Debug.Log("Account successfully created");
                UIClientLog.ServerLogMessageSuccess("Account successfully created");
                CreateAccountError = false;
                break;
            case "email_already_registered":
                Debug.Log("E-Mail already registered");
                UIClientLog.ServerLogMessageError("E-Mail already registered");
                CreateAccountError = true;
                break;
            case "username_already_in_use":
                Debug.Log("Username not allowed");
                UIClientLog.ServerLogMessageError("Username not allowed");
                CreateAccountError = true;
                break;
            default:
                Debug.LogError("Create Account: Unknown message");
                break;
        }

        // Todo Need to disconnect from server
    }

    #endregion

}
