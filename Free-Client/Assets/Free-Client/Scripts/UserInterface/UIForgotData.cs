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
        if(NetworkManager.Instance.Connected())
        {
            buttonSendPassword.interactable = false;
            buttonBack.interactable = false;
            buttonTabUsername.interactable = false;

            NetworkManager.Instance.ForgotPassword(inputFieldUsernameOrEMail.text);
        }
        else
        {
            //Todo inform the user/player that we aren't connected to the Server, Popup
            Debug.Log("Todo inform the user/player that we aren't connected to the Server, Popup");
        }
    }

    public void OnButtonSendUsername()
    {
        if(NetworkManager.Instance.Connected())
        {
            buttonSendUsername.interactable = false;
            buttonBack.interactable = false;
            buttonTabPassword.interactable = false;

            NetworkManager.Instance.ForgotUsername(inputFieldEMail.text);
        }
        else
        {
            //Todo inform the user/player that we aren't connected to the Server, Popup
            Debug.Log("Todo inform the user/player that we aren't connected to the Server, Popup");
        }
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
}
