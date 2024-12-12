using Assambra.GameFramework.GameManager;
using Assambra.FreeClient.Constants;
using Assambra.FreeClient.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assambra.FreeClient.UserInterface.PopupSystem.Manager;
using Assambra.FreeClient.UserInterface.PopupSystem.Enum;
using Assambra.FreeClient.UserInterface.PopupSystem.Popup;

namespace Assambra.FreeClient.UserInterface
{
    public class UICreateAccount : MonoBehaviour
    {
        public Button ButtonBack;
        public Button ButtonCreate;
        public Button ButtonForgotData;

        [SerializeField] private TMP_InputField _inputFieldEmail;
        [SerializeField] private TMP_InputField _inputFieldUsername;
        [SerializeField] private TMP_InputField _inputFieldPassword;

        public void OnButtonCreate()
        {
            if (NetworkManager.Instance.Connected())
            {
                string email = _inputFieldEmail.text;
                string username = _inputFieldUsername.text;
                string password = _inputFieldPassword.text;

                if (ValidateEmail(email) && ValidateUsername(username) && ValidatePassword(password))
                {
                    ButtonCreate.interactable = false;
                    ButtonForgotData.interactable = false;
                    ButtonBack.interactable = false;

                    NetworkManager.Instance.CreateAccount(email, username, password);
                }
            }
            else
                ErrorPopup("Please note: We are currently not connected to a server.");
        }

        public void OnButtonBack()
        {
            GameManager.Instance.ChangeScene(Scenes.Login);
        }

        public void OnButtonForgotData()
        {
            GameManager.Instance.ChangeScene(Scenes.ForgotData);
        }

        private bool ValidateEmail(string email)
        {
            if (!InputValidator.IsValidEmail(email))
            {
                ErrorPopup("This is not a valid email address.");
                return false;
            }
            else
                return true;
        }

        private bool ValidateUsername(string username)
        {
            bool isValid = true;

            if (!InputValidator.IsLengthValid(username, GameConstants.USERNAME_LENGTH_MIN, GameConstants.USERNAME_LENGTH_MAX))
            {
                isValid = false;
                ErrorPopup("The username must be at least " + GameConstants.USERNAME_LENGTH_MIN + " and at most " + GameConstants.USERNAME_LENGTH_MAX + " letters long.");
            }

            if (!InputValidator.DoesNotContainDisallowedName(username, GameConstants.DISALLOWED_NAMES))
            {
                isValid = false;
                ErrorPopup("Username are not allowed!");
            }

            return isValid;
        }

        private bool ValidatePassword(string password)
        {
            if (!InputValidator.IsValidPassword(password, GameConstants.PASSWORD_LENGTH_MIN, GameConstants.PASSWORD_LENGTH_MAX))
            {
                ErrorPopup("Password must be " + GameConstants.PASSWORD_LENGTH_MIN + "-" + GameConstants.PASSWORD_LENGTH_MAX + " include uppercase and lowercase letters, numbers, and special characters like !@#$%^&*().");
                return false;
            }
            else
                return true;
        }

        private void ErrorPopup(string error)
        {
            string title = "Error";

            PopupManager.Instance.ShowInfoPopup<InfoPopup>(PopupType.Error, title, error);
        }
    }
}
