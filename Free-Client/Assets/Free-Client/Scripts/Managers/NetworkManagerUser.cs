using Assambra.GameFramework.GameManager;
using Assambra.FreeClient.Constants;
using Assambra.FreeClient.Helper;
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

namespace Assambra.FreeClient.Managers
{
    public class NetworkManagerUser : EzyAbstractController
    {
        public static NetworkManagerUser Instance { get; private set; }

        [SerializeField] EzySocketConfig socketConfig;
        [SerializeField] private string guestPassword = "Assambra";

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

            AddHandler<EzyObject>(Commands.CREATE_USER, OnCreateUserResponse);
            AddHandler<EzyObject>(Commands.ACTIVATE_USER, OnActivateUserResponse);
            AddHandler<EzyObject>(Commands.RESEND_ACTIVATION_MAIL, OnResendActivationMail);
            AddHandler<EzyObject>(Commands.FORGOT_PASSWORD, OnForgotPasswordResponse);
            AddHandler<EzyObject>(Commands.FORGOT_USERNAME, OnForgotUsernameResponse);

            Login();
        }

        private void OnDisable()
        {
            Disconnect();
            UnbindSocketHandlers();
            UnbindAppHandlers();
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
        }

        #region REQUEST

        public void Login()
        {
            socketProxy.onLoginSuccess<Object>(OnLoginSucess);
            socketProxy.onLoginError<Object>(OnLoginError);
            socketProxy.onAppAccessed<Object>(OnAppAccessed);

            socketProxy.setLoginUsername(CreateGuestName());
            socketProxy.setLoginPassword(guestPassword);

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

        public void CreateAccount(string email, string username, string password)
        {
            GameManager.Instance.Account = username;

            EzyObject data = EzyEntityFactory
            .newObjectBuilder()
            .append("email", email)
            .append("username", username)
            .append("password", password)
            .build();

            appProxy.send(Commands.CREATE_USER, data, socketConfig.EnableSSL);
        }

        public void ActivateAccount(string activationcode)
        {
            EzyObject data = EzyEntityFactory
            .newObjectBuilder()
            .append("username", GameManager.Instance.Account)
            .append("activationCode", activationcode)
            .build();

            appProxy.send(Commands.ACTIVATE_USER, data);
        }

        public void ResendActivationCodeEmail()
        {
            EzyObject data = EzyEntityFactory
                .newObjectBuilder()
                .append("username", GameManager.Instance.Account)
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

        #region RESPONSE

        private void OnUdpHandshake(EzySocketProxy proxy, Object data)
        {
            Debug.Log("OnUdpHandshake");
            socketProxy.send(new EzyAppAccessRequest(socketConfig.AppName));
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

        private void OnCreateUserResponse(EzyAppProxy proxy, EzyObject data)
        {
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
                    InformationPopupAccountActivation("Your account has been successfully activated");
                    break;
                case "wrong_activation_code":
                    ErrorPopup("Wrong activation code");
                    break;
            }
        }

        private void OnResendActivationMail(EzyAppProxy proxy, EzyObject data)
        {
            InformationPopup("Your account activation code has been sent again to your email address.");
        }

        private void OnForgotPasswordResponse(EzyAppProxy proxy, EzyObject data)
        {
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
                    InformationPopup("Your username has been sent to your email address");
                    break;
                case "not_found":
                    InformationPopup("This e-mail address are not registered");
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
            if (NetworkManagerGame.Instance.Connected())
                NetworkManagerGame.Instance.GetCharacterList();
            else
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
}
