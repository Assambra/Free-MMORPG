using Assambra.FreeClient.Helper;
using Assambra.FreeClient.UserInterface.PopupSystem.Enum;
using Assambra.FreeClient.UserInterface.PopupSystem.Manager;
using Assambra.FreeClient.UserInterface.PopupSystem.Popup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface
{
    public class UIAccountActivation : MonoBehaviour
    {
        [SerializeField] private Button _buttonSendActivationCode;
        [SerializeField] private Button _buttonResendActivationEmail;
        [SerializeField] private Button _buttonQuit;
        [SerializeField] private TMP_InputField _inputFieldActivationCode;

        public void OnButtonSendActivationCode()
        {
            string activationCode = _inputFieldActivationCode.text;

            if (InputValidator.IsNotEmpty(activationCode))
                NetworkManager.Instance.ActivateAccount(activationCode);
            else
                ErrorPopup("Please note: The activation input field cannot be empty. Please enter your activation code and try again.");

        }

        public void OnButtonResendActivationEmail()
        {
            _buttonResendActivationEmail.interactable = false;
            NetworkManager.Instance.ResendActivationCodeEmail();
        }

        public void OnButtonQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void ErrorPopup(string error)
        {
            string title = "Error";

            PopupManager.Instance.ShowInfoPopup<InfoPopup>(PopupType.Error, title, error);
        }
    }
}
