using TMPro;
using UnityEngine;


public class UIClientLog : MonoBehaviour
{
    [SerializeField] TMP_Text textFieldServerLog;
    [SerializeField] ClientLogFile clientLogFile;

    private NetworkManager networkManager;


    private void Awake()
    {
        networkManager = GameObject.FindObjectOfType<NetworkManager>();
        networkManager.UIClientLog = this;

    }

    void Update()
    {
        textFieldServerLog.text = clientLogFile.File;
    }

    public void ServerLogMessageInfo(string message)
    {
        clientLogFile.File += "<color=white>Info: " + message + "</color><br>";
    }

    public void ServerLogMessageSuccess(string message)
    {
        clientLogFile.File += "<color=green>Success: " + message + "</color><br>";
    }

    public void ServerLogMessageError(string message)
    {
        clientLogFile.File += "<color=red>Error: " + message + "</color><br>";
    }

    private string CreateNewLine(string text)
    {
        return text + "\n";
    }
}
