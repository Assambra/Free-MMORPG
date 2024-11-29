using Assambra.GameFramework.GameManager;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assambra.FreeServer
{
    public class ServerManager : BaseGameManager
    {
        public static ServerManager Instance;
        
        public bool IsDebug = false;
        public string Room { get => _room; }
        public string Password { get => _password; }

        public Dictionary<uint, Entity> ServerEntities = new Dictionary<uint, Entity>();
        public UIServerLog ServerLog;

        [SerializeField] private GameObject _playerPrefab;

        private string _username;
        private string _password;
        private string _room;

        private bool doOnce;


        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;

            string[] args = Environment.GetCommandLineArgs();

            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] == "--username" && i + 1 < args.Length)
                {
                    _username = args[i + 1];
                }
                else if (args[i] == "--password" && i + 1 < args.Length)
                {
                    _password = args[i + 1];
                }
                else if (args[i] == "--room" && i + 1 < args.Length)
                {
                    _room = args[i + 1];
                }
            }

            if (IsDebug)
            {
                _username = "Test";
                _password = "123456";
                _room = "Newcomer";
            }
        }

        void Update()
        {
            if (!doOnce && !String.IsNullOrEmpty(_room) && !String.IsNullOrEmpty(_username) && !String.IsNullOrEmpty(_password))
            {
                doOnce = true;

                switch (_room)
                {
                    case "Newcomer":
                        ChangeScene(Scenes.Newcomer);
                        Debug.Log("Change server scene to Newcomer");
                        break;
                    case "World":
                        ChangeScene(Scenes.World);
                        Debug.Log("Change server scene to World");
                        break;
                    default:
                        Debug.LogError("Receive unhandled room args");
                        break;
                }
                
                if (!IsDebug)
                    NetworkManager.Instance.Login(_username, _password);
                
            }
        }
        
        protected override void OnSceneChanged(Scene lastScene, Scene newScene)
        {
            
        }

        public GameObject CreatePlayer(Vector3 position, Vector3 rotation)
        {
            return Instantiate(_playerPrefab, position, Quaternion.Euler(rotation));
        }
    }
}

