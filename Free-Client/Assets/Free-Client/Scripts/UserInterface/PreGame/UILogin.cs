using UnityEngine;
using TMPro;


public class UILogin : MonoBehaviour
{
    [SerializeField] TMP_InputField inputFieldUsername;
    [SerializeField] TMP_InputField inputFieldPassword;

    private string password;
    private string username;


    public void OnButtonQuit()
    {
        NetworkManagerGame.Instance.Disconnect();
        NetworkManagerAccount.Instance.Disconnect();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
         #endif
    }

    public void OnButtonLogin()
    {
        // Todo button interactable false, wait until server response or popup after timer try again to connect
        username = inputFieldUsername.text;
        password = inputFieldPassword.text;

        NetworkManagerGame.Instance.Login(username, password);
    }

    public void OnButtonNeedAccount()
    {
        GameManager.Instance.ChangeScene(Scenes.CreateAccount);
    }

    public void OnButtonForgotData()
    {
        GameManager.Instance.ChangeScene(Scenes.ForgotData);
    }
}
