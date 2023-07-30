using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIForgotData : MonoBehaviour
{
    public Button buttonBack;
    public Button buttonSendPassword;

    [SerializeField] private TMP_InputField inputFieldUsernameOrEMail;

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
}
