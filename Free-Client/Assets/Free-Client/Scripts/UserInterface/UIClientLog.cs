using TMPro;
using UnityEngine;


public class UIClientLog : MonoBehaviour
{
    [SerializeField] TMP_Text textFieldServerLog;
    [SerializeField] ClientLogFile clientLogFile;


    private void Awake()
    {
        NetworkManager.Instance.UIClientLog = this;
    }

    void Update()
    {
        textFieldServerLog.text = clientLogFile.File;
    }

    public void ClearLog()
    {
        clientLogFile.File = "";
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
}
