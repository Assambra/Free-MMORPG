using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICreateAccount : MonoBehaviour
{
    public Button buttonCreate;
    public Button buttonBack;
    public Button buttonForgotPassword;

    [SerializeField] TMP_InputField inputFieldEmail;
    [SerializeField] TMP_InputField inputFieldUsername;
    [SerializeField] TMP_InputField inputFieldPassword;
    

    private string email;
    private string password;
    private string username;

    private NetworkManager networkManager;
    private SceneHandler sceneHandler;


    private void Awake()
    {
        networkManager = GameObject.FindObjectOfType<NetworkManager>();
        sceneHandler = GameObject.FindObjectOfType<SceneHandler>();
    }

    public void OnButtonCreate()
    {
        buttonCreate.interactable = false;
        buttonBack.interactable = false;
        buttonForgotPassword.interactable = false;

        email = inputFieldEmail.text;
        username = inputFieldUsername.text;
        password = inputFieldPassword.text;

        networkManager.CreateAccount(email, username, password);
    }

    public void OnButtonBack()
    {
        sceneHandler.CurrentScene = sceneHandler.Scenes[0];
    }

    public void OnButtonForgotPassword()
    {
        sceneHandler.CurrentScene = sceneHandler.Scenes[2];
    }
}
