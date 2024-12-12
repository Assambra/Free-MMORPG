using Assambra.GameFramework.GameManager;
using Assambra.FreeClient.Helper;
using UnityEngine;
using TMPro;
using Assambra.FreeClient.UserInterface.PopupSystem.Manager;
using Assambra.FreeClient.UserInterface.PopupSystem.Popup;
using Assambra.FreeClient.UserInterface.PopupSystem.Enum;

namespace Assambra.FreeClient.UserInterface
{
    public class UILogin : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputFieldUsername;
        [SerializeField] private TMP_InputField _inputFieldPassword;

        private string _password;
        private string _username;


        public void OnButtonQuit()
        {
            NetworkManager.Instance.Disconnect();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        public void OnButtonLogin()
        {
            // Todo button interactable false, wait until server response or popup after timer try again to connect
            _username = _inputFieldUsername.text;
            _password = _inputFieldPassword.text;

            if (InputValidator.IsNotEmpty(_username) && InputValidator.IsNotEmpty(_password))
            {
                if(NetworkManager.Instance.LoginState == LoginState.Lobby)
                    NetworkManager.Instance.LoginState = LoginState.Game;

                NetworkManager.Instance.Login(_username, _password);
            }
            else
                InformationPopup("Username or password cannot be empty. Please enter both and try again.");
        }

        public void OnButtonNeedAccount()
        {
            GameManager.Instance.ChangeScene(Scenes.CreateAccount);
        }

        public void OnButtonForgotData()
        {
            GameManager.Instance.ChangeScene(Scenes.ForgotData);
        }

        private void InformationPopup(string info)
        {
            string title = "Info";

            PopupManager.Instance.ShowInfoPopup<InfoPopup>(PopupType.Info, title, info);
        }
    }
}
