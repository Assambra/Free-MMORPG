using com.tvd12.ezyfoxserver.client;
using com.tvd12.ezyfoxserver.client.constant;
using com.tvd12.ezyfoxserver.client.entity;
using com.tvd12.ezyfoxserver.client.factory;
using com.tvd12.ezyfoxserver.client.request;
using com.tvd12.ezyfoxserver.client.support;
using com.tvd12.ezyfoxserver.client.unity;
using UnityEngine;
using Object = System.Object;

public class NetworkManagerAccount : EzyDefaultController
{
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
        AddHandler<EzyObject>(Commands.ACTIVATE_ACCOUNT, OnActivateAccountResponse);
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

        appProxy.send(Commands.CREATE_ACCOUNT, data, socketConfigVariable.Value.EnableSSL);
<<<<<<< HEAD
    }

    public void ActivateAccount(string activationcode)
    {
        EzyObject data = EzyEntityFactory
        .newObjectBuilder()
        .append("activationcode", activationcode)
        .build();

        appProxy.send(Commands.ACTIVATE_ACCOUNT, data);
=======
>>>>>>> 614543d02b997fa1245ae9e80e81ce863a588910
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
            case "successful":
                InformationPopupCrateAccount("Account successfully created");
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
        // Todo Disconnect from server until we dont have a solution to communicate with the server without login
        Disconnect();

        uICreateAccount.buttonCreate.interactable = true;
        uICreateAccount.buttonForgotData.interactable = true;
        uICreateAccount.buttonBack.interactable = true;
    }

    private void OnActivateAccountResponse(EzyAppProxy proxy, EzyObject data)
    {
        string result = data.get<string>("result");
        switch (result)
        {
            case "successful":
                InformationPopupAccountActivation("Your account has been successfully activated");
                break;
            case "wrong_activation_code":
                ErrorPopup("Wrong activation code");
                break;
        }
    }

    private void OnForgotPasswordResponse(EzyAppProxy proxy, EzyObject data)
    {
        UIForgotData uIForgotData = GameObject.FindObjectOfType<UIForgotData>();
        if (uIForgotData == null)
            Debug.LogError("UIForgotData not found!");

        string result = data.get<string>("result");

        switch (result)
        {
            case "successful":
                InformationPopup("Your new password has been sent to your registered e-mail address");
                break;
            case "no_account":
                InformationPopup("No Account found for given username or email address");
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
        switch (result)
        {
            case "successful":
                InformationPopup("Your username has been sent to your email address");
                break;
            case "not_found":
                InformationPopup("This e-mail address are not registered");
                break;
            default:
                Debug.LogError("Forgot Username: Unknown result: " + result);
                break;
        }

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

    private void InformationPopupCrateAccount(string information)
    {
        string title = "Info";
        string info = information;

        InformationPopup popup = PopupManager.Instance.ShowInformationPopup<InformationPopup>(title, info, null);

        popup.Setup(
            title,
            info,
            () => { OnInformationPopupCrateAccountOK(); popup.Destroy(); }
        );
    }

    private void OnInformationPopupCrateAccountOK()
    {
        GameManager.Instance.ChangeScene(Scenes.AccountActivation);
    }

    private void InformationPopupAccountActivation(string information)
    {
        string title = "Info";
        string info = information;

        InformationPopup popup = PopupManager.Instance.ShowInformationPopup<InformationPopup>(title, info, null);

        popup.Setup(
            title,
            info,
            () => { OnInformationPopupAccountActivationOK(); popup.Destroy(); }
        );
    }

    private void OnInformationPopupAccountActivationOK()
    {
        GameManager.Instance.ChangeScene(Scenes.Login);
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
