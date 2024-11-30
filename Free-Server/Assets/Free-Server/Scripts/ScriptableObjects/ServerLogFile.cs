using UnityEngine;

namespace Assambra.FreeServer
{
    [CreateAssetMenu(fileName = "ServerLogFile", menuName = "Assambra/ServerLogFile", order = 1)]
    public class ServerLogFile : ScriptableObject
    {
        public string File = "";
    }
}

