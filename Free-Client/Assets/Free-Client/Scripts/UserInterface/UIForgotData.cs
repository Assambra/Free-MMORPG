using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIForgotData : MonoBehaviour
{
    public Button buttonBack;
    public Button buttonSendPassword;
    public Button buttonSendUsername;

    [SerializeField] private TMP_InputField inputFieldUsernameOrEMail;
    [SerializeField] private GameObject gameObjectForgotPassword;
    [SerializeField] private GameObject gameObjectForgotUsername;

    private NetworkManager networkManager;
    private SceneHandler sceneHandler;

    private void Awake()
    {
        networkManager = GameObject.FindObjectOfType<NetworkManager>();
        sceneHandler = GameObject.FindObjectOfType<SceneHandler>();
    }

    public void OnButtonBack()
    {
        sceneHandler.CurrentScene = sceneHandler.Scenes[0];
    }

    public void OnButtonSendPassword()
    {
        buttonSendPassword.interactable = false;
        buttonBack.interactable = false;

        networkManager.ForgotPassword(inputFieldUsernameOrEMail.text);
    }
    public void OnButtonSendUsername()
    {
        buttonSendUsername.interactable = false;
        buttonBack.interactable = false;

        networkManager.ForgotPassword(inputFieldUsernameOrEMail.text);
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
