using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICreateAccount : MonoBehaviour
{
    [SerializeField] TMP_InputField inputFieldEmail;
    [SerializeField] TMP_InputField inputFieldUsername;
    [SerializeField] TMP_InputField inputFieldPassword;

    private string email;
    private string password;
    private string username;

    //private NetworkManager networkManager;
    private SceneHandler sceneHandler;


    private void Awake()
    {
        //networkManager = GameObject.FindObjectOfType<NetworkManager>();
        sceneHandler = GameObject.FindObjectOfType<SceneHandler>();
    }

    public void OnButtonCreate()
    {
        email = inputFieldEmail.text;
        username = inputFieldUsername.text;
        password = inputFieldPassword.text;

        //networkManager.CreateAccount(email, username, password);
    }

    public void OnButtonBack()
    {
        sceneHandler.CurrentScene = sceneHandler.Scenes[0];
    }
}
