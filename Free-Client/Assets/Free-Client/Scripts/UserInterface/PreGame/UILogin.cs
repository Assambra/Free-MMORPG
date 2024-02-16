using UnityEngine;
using TMPro;


public class UILogin : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputFieldUsername;
    [SerializeField] private TMP_InputField _inputFieldPassword;

    private string _password;
    private string _username;


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
        _username = _inputFieldUsername.text;
        _password = _inputFieldPassword.text;

        if(InputValidator.IsNotEmpty(_username) && InputValidator.IsNotEmpty(_password))
            NetworkManagerGame.Instance.Login(_username, _password);
        else
            ErrorPopup("Please note: Username or password cannot be empty. Please enter both and try again.");
    }

    public void OnButtonNeedAccount()
    {
        GameManager.Instance.ChangeScene(Scenes.CreateAccount);
    }

    public void OnButtonForgotData()
    {
        GameManager.Instance.ChangeScene(Scenes.ForgotData);
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
