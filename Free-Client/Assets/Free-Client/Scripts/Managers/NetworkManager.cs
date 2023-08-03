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
    [SerializeField] private string zoneName = "free-server";
    [SerializeField] private string appName = "free-server";
    [SerializeField] private string host = "127.0.0.1";
    [SerializeField] private int tcpPort = 3005;
    [SerializeField] private int udpPort = 2611;
    [SerializeField] private bool useUdp = false;

    public UIClientLog UIClientLog;

    // private variables
    private EzySocketProxy socketProxy;
    private EzyAppProxy appProxy;
    private List<Tuple<String, Object>> handlers = new();

    private string email;
    private string username;
    private string password;
    private string usernameOrEMail;

    private bool createAccount = false;
    private bool forgotPassword = false;
    private bool forgotUsername = false;

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

        if (UIClientLog != null)
            UIClientLog.ClearLog();
        
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

    public void Disconnect()
    {
        EzyClients.getInstance().getClient(zoneName).disconnect();
    }

    #region REQUESTS

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
            socketProxy.setTransportType(EzyTransportType.TCP);
        else
        {
            socketProxy.setTransportType(EzyTransportType.UDP);
            socketProxy.onUdpHandshake<Object>(OnUdpHandshake);
        }

        socketProxy.connect();

        on<EzyObject>(Commands.CREATE_ACCOUNT, OnCreateAccountResponse);
        on<EzyObject>(Commands.FORGOT_PASSWORD, OnForgotPasswordResponse);
        on<EzyObject>(Commands.FORGOT_USERNAME, OnForgotUsernameResponse);
    }

    public void CreateAccount(string email, string username, string password)
    {
        createAccount = true;

        Login("Guest#" + RandomString.GetNumericString(1000001), "");

        this.email = email;
        this.username = username;
        this.password = password;
    }

    public void ForgotPassword(string usernameoremail)
    {
        forgotPassword = true;

        Login("Guest#" + RandomString.GetNumericString(1000001), "");

        this.usernameOrEMail = usernameoremail;
    }

    public void ForgotUsername(string email)
    {
        forgotUsername = true;

        Login("Guest#" + RandomString.GetNumericString(1000001), "");
        this.email = email;
    }

    #endregion

    #region SERVER RESPONSE

    private void OnUdpHandshake(EzySocketProxy proxy, Object data)
    {
        Debug.Log("OnUdpHandshake");
        socketProxy.send(new EzyAppAccessRequest(appName));
    }

    private void OnAppAccessed(EzyAppProxy proxy, Object data)
    {
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

        if(forgotUsername)
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

        uICreateAccount.buttonForgotData.interactable = true;
        uICreateAccount.buttonBack.interactable = true;
        uICreateAccount.buttonCreate.interactable = true;
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

        uIForgotData.buttonBack.interactable = true;
        uIForgotData.buttonSendPassword.interactable = true;
    }

    private void OnForgotUsernameResponse(EzyAppProxy proxy, EzyObject data)
    {
        string result = data.get<string>("result");

        if (result == "success")
            UIClientLog.ServerLogMessageSuccess("Your username has been sent to your email address");
        else if (result == "not_found")
            UIClientLog.ServerLogMessageError("This e-mail address are not registered");
    }

    #endregion
}
