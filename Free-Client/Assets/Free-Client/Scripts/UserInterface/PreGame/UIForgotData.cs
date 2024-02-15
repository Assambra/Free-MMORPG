using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIForgotData : MonoBehaviour
{
    public Button buttonBack;
    public Button buttonSendPassword;
    public Button buttonSendUsername;
    public Button buttonTabUsername;
    public Button buttonTabPassword;

    [SerializeField] private TMP_InputField inputFieldUsernameOrEMail;
    [SerializeField] private TMP_InputField inputFieldEMail;

    [SerializeField] private GameObject gameObjectForgotPassword;
    [SerializeField] private GameObject gameObjectForgotUsername;


    public void OnButtonBack()
    {
        GameManager.Instance.ChangeScene(Scenes.Login);
    }

    public void OnButtonSendPassword()
    {
        if(NetworkManagerAccount.Instance.Connected())
        {
            buttonSendPassword.interactable = false;
            buttonBack.interactable = false;
            buttonTabUsername.interactable = false;

            if (InputValidator.IsNotEmpty(inputFieldUsernameOrEMail.text))
                NetworkManagerAccount.Instance.ForgotPassword(inputFieldUsernameOrEMail.text);
            else
                ErrorPopup("Please note: You must enter either a username or email address. Please fill in the required field and try again.");
        }
        else
            ErrorPopup("Please note: We are currently not connected to a server. Check your internet connection and try again later.");
    }

    public void OnButtonSendUsername()
    {
        if(NetworkManagerAccount.Instance.Connected())
        {
            buttonSendUsername.interactable = false;
            buttonBack.interactable = false;
            buttonTabPassword.interactable = false;
            
            if (InputValidator.IsNotEmpty(inputFieldEMail.text))
                NetworkManagerAccount.Instance.ForgotUsername(inputFieldEMail.text);
            else
                ErrorPopup("Please note: The email address field cannot be empty. Please enter your email address and try again.");
        }
        else
            ErrorPopup("Please note: We are currently not connected to a server. Check your internet connection and try again later.");
    }

    public void OnButtonTabPassword()
    {
        if(!gameObjectForgotPassword.activeSelf)
            gameObjectForgotPassword.SetActive(true);
        if (gameObjectForgotUsername.activeSelf)
            gameObjectForgotUsername.SetActive(false);
    }

    public void OnButtonTapUsername()
    {
        if (!gameObjectForgotUsername.activeSelf)
            gameObjectForgotUsername.SetActive(true);
        if (gameObjectForgotPassword.activeSelf)
            gameObjectForgotPassword.SetActive(false);
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
}
