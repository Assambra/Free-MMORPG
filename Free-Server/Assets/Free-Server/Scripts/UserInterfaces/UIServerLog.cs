using UnityEngine;
using TMPro;

namespace Assambra.FreeServer
{
    public class UIServerLog : MonoBehaviour
    {
        [SerializeField] TMP_Text textFieldServerLog;
        [SerializeField] ServerLogFile serverLogFile;

        private bool needsUpdate = false;

        private void Start()
        {            
            ServerManager.Instance.ServerLog = this;
            ClearLog();
        }

        void Update()
        {
            if (needsUpdate)
            {
                textFieldServerLog.text = serverLogFile.File;
                needsUpdate = false;
            }
        }

        public void ClearLog()
        {
            serverLogFile.File = "";
        }

        public void ServerLogMessageInfo(string message)
        {
            Debug.Log(message);
            serverLogFile.File += "<color=white>Info: " + message + "</color><br>";
            needsUpdate = true;
        }

        public void ServerLogMessageSuccess(string message)
        {
            serverLogFile.File += "<color=green>Success: " + message + "</color><br>";
            needsUpdate = true;
        }

        public void ServerLogMessageError(string message)
        {
            serverLogFile.File += "<color=red>Error: " + message + "</color><br>";
            needsUpdate = true;
        }
    }
}
