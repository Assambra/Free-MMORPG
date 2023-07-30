using UnityEngine;
using TMPro;


public class UILogin : MonoBehaviour
{
    [SerializeField] TMP_InputField inputFieldUsername;
    [SerializeField] TMP_InputField inputFieldPassword;

    private string password;
    private string username;

    private NetworkManager networkManager;
    private SceneHandler sceneHandler;

    private void Awake()
    {
        networkManager = GameObject.FindObjectOfType<NetworkManager>();
        sceneHandler = GameObject.FindObjectOfType<SceneHandler>();
    }

    public void OnButtonQuit()
    {
        networkManager.Disconnect();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnButtonLogin()
    {
        username = inputFieldUsername.text;
        password = inputFieldPassword.text;

        networkManager.Login(username, password);
    }

    public void OnButtonNeedAccount()
    {
        sceneHandler.CurrentScene = sceneHandler.Scenes[1];
    }

    public void OnButtonForgotData()
    {
        sceneHandler.CurrentScene = sceneHandler.Scenes[2];
    }
}
