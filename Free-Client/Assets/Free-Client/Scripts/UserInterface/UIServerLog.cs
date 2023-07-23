using TMPro;
using UnityEngine;


public class UIServerLog : MonoBehaviour
{
    [SerializeField] TMP_Text textFieldServerLog;

    private NetworkManager networkManager;

    private string logFile;

    private void Awake()
    {
        networkManager = GameObject.FindObjectOfType<NetworkManager>();
        networkManager.UIServerLog = this;
    }

    void Update()
    {
        textFieldServerLog.text = logFile;
    }

    public void ServerLogMessageInfo(string message)
    {
        logFile += "<color=white>Info: " + message + "</color><br>";
    }

    public void ServerLogMessageSuccess(string message)
    {
        logFile += "<color=green>Success: " + message + "</color><br>";
    }

    public void ServerLogMessageError(string message)
    {
        logFile += "<color=red>Error: " + message + "</color><br>";
    }

    private string CreateNewLine(string text)
    {
        return text + "\n";
    }
}
