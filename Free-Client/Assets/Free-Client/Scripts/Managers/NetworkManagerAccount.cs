using com.tvd12.ezyfoxserver.client;
using com.tvd12.ezyfoxserver.client.constant;
using com.tvd12.ezyfoxserver.client.entity;
using com.tvd12.ezyfoxserver.client.factory;
using com.tvd12.ezyfoxserver.client.request;
using com.tvd12.ezyfoxserver.client.support;
using com.tvd12.ezyfoxserver.client.unity;
using UnityEngine;

public class NetworkManagerAccount : EzyDefaultController
{
    [HideInInspector] public UIClientLog UIClientLog;

    [SerializeField] private string guestPassword = "Assambra";

    public static NetworkManagerAccount Instance { get; private set; }

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

        AddHandler<EzyObject>(Commands.CREATE_ACCOUNT, OnCreateAccountResponse);
        AddHandler<EzyObject>(Commands.FORGOT_PASSWORD, OnForgotPasswordResponse);
        AddHandler<EzyObject>(Commands.FORGOT_USERNAME, OnForgotUsernameResponse);
        
        
        Login();
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
        if (Connected())
            base.Disconnect();
    }

    #region REQUEST

    public void Login()
    {
        socketProxy.onLoginSuccess<Object>(OnLoginSucess);
        socketProxy.onLoginError<Object>(OnLoginError);
        socketProxy.onAppAccessed<Object>(OnAppAccessed);

        socketProxy.setLoginUsername(CreateGuestName());
        socketProxy.setLoginPassword(guestPassword);

        socketProxy.setUrl(socketConfigVariable.Value.TcpUrl);
        socketProxy.setUdpPort(socketConfigVariable.Value.UdpPort);
        socketProxy.setDefaultAppName(socketConfigVariable.Value.AppName);

        if (socketConfigVariable.Value.UdpUsage)
        {
            socketProxy.setTransportType(EzyTransportType.UDP);
            socketProxy.onUdpHandshake<Object>(OnUdpHandshake);
        }
        else
            socketProxy.setTransportType(EzyTransportType.TCP);

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

    #region RESPONSE

    private void OnUdpHandshake(EzySocketProxy proxy, Object data)
    {
        Debug.Log("OnUdpHandshake");
        socketProxy.send(new EzyAppAccessRequest(socketConfigVariable.Value.AppName));
    }

    private void OnAppAccessed(EzyAppProxy proxy, Object data)
    {
        Debug.Log("Account: App access successfully");
    }

    private void OnLoginSucess(EzySocketProxy proxy, Object data)
    {
        Debug.Log("OnLoginSucess");
    }

    private void OnLoginError(EzySocketProxy proxy, Object data)
    {
        Debug.Log("OnLoginError");
        //Todo capture the error and handle
        //Account allready loged in try again with different guestname if connected;
        //Login();
    }

    private void OnCreateAccountResponse(EzyAppProxy proxy, EzyObject data)
    {
        UICreateAccount uICreateAccount = GameObject.FindAnyObjectByType<UICreateAccount>();
        if (uICreateAccount == null)
            Debug.LogError("UICreateAccount not found!");

        string result = data.get<string>("result");

        switch (result)
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
        if (uIForgotData == null)
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
            if (username == "")
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

    #endregion

    private string CreateGuestName()
    {
        return "Guest#" + RandomString.GetNumericString(1000001);
    }
}
