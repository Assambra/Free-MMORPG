using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICreateAccount : MonoBehaviour
{
    public Button buttonCreate;
    public Button buttonBack;
    public Button buttonForgotData;

    [SerializeField] TMP_InputField inputFieldEmail;
    [SerializeField] TMP_InputField inputFieldUsername;
    [SerializeField] TMP_InputField inputFieldPassword;
    

    private string email;
    private string password;
    private string username;


    public void OnButtonCreate()
    {
        if (NetworkManagerAccount.Instance.Connected())
        {
            buttonCreate.interactable = false;
            buttonForgotData.interactable = false;
            buttonBack.interactable = false;
            

            email = inputFieldEmail.text;
            username = inputFieldUsername.text;
            password = inputFieldPassword.text;

            NetworkManagerAccount.Instance.CreateAccount(email, username, password);
        }
        else
        {
            //Todo inform the user/player that we aren't connected to the Server, Popup
            Debug.Log("Todo inform the user/player that we aren't connected to the Server, Popup");
        }
    }

    public void OnButtonBack()
    {
        GameManager.Instance.ChangeScene(Scenes.Login);
    }

    public void OnButtonForgotData()
    {
        GameManager.Instance.ChangeScene(Scenes.ForgotData);
    }
}
