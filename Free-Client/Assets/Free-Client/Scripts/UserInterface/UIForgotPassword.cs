using TMPro;
using UnityEngine;

public class UIForgotPassword : MonoBehaviour
{
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
        networkManager.ForgotPassword(inputFieldUsernameOrEMail.text);
    }
}
