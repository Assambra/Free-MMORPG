using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIForgotData : MonoBehaviour
{
    public Button buttonBack;
    public Button buttonSendPassword;
    public Button buttonSendUsername;

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
        // Todo #15
        buttonSendPassword.interactable = false;
        buttonBack.interactable = false;

        NetworkManager.Instance.ForgotPassword(inputFieldUsernameOrEMail.text);
    }

    public void OnButtonSendUsername()
    {
        // Todo #15
        NetworkManager.Instance.ForgotUsername(inputFieldEMail.text);
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
