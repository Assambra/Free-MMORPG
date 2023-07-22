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

    [SerializeField] private string username;
    [SerializeField] private string password;

    private SceneHandler sceneHandler;

    private EzySocketProxy socketProxy;
    private EzyAppProxy appProxy;

    private List<Tuple<String, Object>> handlers = new();

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
    }

    public void CreateAccount(string email, string username, string password)
    {
        EzyObject data = EzyEntityFactory
            .newObjectBuilder()
            .append("email", email)
            .append("username", username)
            .append("password", password)
            .build();

        appProxy.send(Commands.CREATE_ACCOUNT, data);
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
    }
}
