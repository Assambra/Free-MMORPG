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
            
            if(ValidateEmail(_email) && ValidateUsername(_username) && ValidatePassword(_password))
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

    private bool ValidateEmail(string email)
    {
        return InputValidator.IsValidEmail(email);
    }

    private bool ValidateUsername(string username)
    {
        bool isValid = true;

        if (!InputValidator.IsLengthValid(username, GameConstants.MIN_USERNAME_LENGTH, GameConstants.MAX_USERNAME_LENGTH))
        {
            isValid = false;
            ErrorPopup("The username must be at least " + GameConstants.MIN_USERNAME_LENGTH + " and at most " + GameConstants.MAX_USERNAME_LENGTH + " letters long.");
        }

        if (!InputValidator.DoesNotContainDisallowedName(username, GameConstants.DISALLOWED_USERNAMES))
        {
            isValid = false;
            ErrorPopup("Username are not allowed!");
        }

        return isValid;
    }

    private bool ValidatePassword(string password)
    {
        if (!InputValidator.IsValidPassword(password, GameConstants.MIN_PASSWORD_LENGTH, GameConstants.MAX_PASSWORD_LENGTH))
        {
            ErrorPopup("Password must be " + GameConstants.MIN_PASSWORD_LENGTH + "-" + GameConstants.MAX_PASSWORD_LENGTH + " include uppercase and lowercase letters, numbers, and special characters like !@#$%^&*().");
            return false;
        }
        else
            return true;
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
