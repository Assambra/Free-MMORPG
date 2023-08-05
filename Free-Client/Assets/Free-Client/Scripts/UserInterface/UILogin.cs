using UnityEngine;
using TMPro;


public class UILogin : MonoBehaviour
{
    [SerializeField] TMP_InputField inputFieldUsername;
    [SerializeField] TMP_InputField inputFieldPassword;

    private string password;
    private string username;

    private SceneHandler sceneHandler;


    private void Awake()
    {
        sceneHandler = GameObject.FindObjectOfType<SceneHandler>();
    }

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
        username = inputFieldUsername.text;
        password = inputFieldPassword.text;

        NetworkManager.Instance.Login(username, password, true);
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
