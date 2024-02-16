using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICreateAccount : MonoBehaviour
{
    public Button ButtonBack;
    public Button ButtonCreate;
    public Button ButtonForgotData;

    [SerializeField] private TMP_InputField _inputFieldEmail;
    [SerializeField] private TMP_InputField _inputFieldUsername;
    [SerializeField] private TMP_InputField _inputFieldPassword;
    
    private string _email;
    private string _password;
    private string _username;

    public void OnButtonCreate()
    {
        if (NetworkManagerAccount.Instance.Connected())
        {
            ButtonCreate.interactable = false;
            ButtonForgotData.interactable = false;
            ButtonBack.interactable = false;
            
            _email = _inputFieldEmail.text;
            _username = _inputFieldUsername.text;
            _password = _inputFieldPassword.text;

            NetworkManagerAccount.Instance.CreateAccount(_email, _username, _password);
        }
        else
            ErrorPopup("Please note: We are currently not connected to a server.");
    }

    public void OnButtonBack()
    {
        GameManager.Instance.ChangeScene(Scenes.Login);
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
