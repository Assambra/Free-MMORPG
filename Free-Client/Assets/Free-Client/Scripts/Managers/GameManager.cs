using Assambra.GameFramework.GameManager;
using Assambra.GameFramework.CameraController;
using Assambra.GameFramework.MouseHandler;
using System.Collections.Generic;
using UMA.CharacterSystem;
using UnityEngine;


namespace Assambra.FreeClient
{
    public class GameManager : BaseGameManager
    {
        public static GameManager Instance;

        [field: SerializeField] public CameraController CameraController { get; private set; }
        [field: SerializeField] public UIHandler UIHandler { get; private set; }
        [field: SerializeField] public SceneHandler SceneHandler { get; private set; }
        [field: SerializeField] public MouseHandler MouseHandler { get; private set; }
        [field: SerializeField] public Light DirectionalLight { get; private set; }
        [field: SerializeField] public Camera MainCamera { get; private set; }

        public Player Player { get; private set; }
        public DynamicCharacterAvatar Avatar { get; private set; }
        public List<EntityModel> CharacterInfos = new List<EntityModel>();
        public Dictionary<long, Entity> ClientEntities = new Dictionary<long, Entity>();

        public bool CharacterCreatedAndReadyToPlay = false;
        public long CharacterId = 0;

        [Header("Player Prefab")]
        [SerializeField] GameObject playerPrefab;

        // Private
        private GameObject playerGameObject;
    
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        private void Start()
        {
            ChangeState(GameState.Lobby);
        }

        private void Update()
        {
            CameraController.IsOverUIElement = MouseHandler.IsOverUIElement;

            switch (_currentState)
            {
                case GameState.Lobby:
                    //Debug.Log("GameManager::Update() Demo GameState Lobby");
                    break;
                case GameState.Game:
                    //Debug.Log("GameManager::Update() Demo GameState Game");
                    break;
            }
        }

        protected override void OnSceneChanged(Scene lastScene, Scene newScene)
        {
            if (newScene.name == Scenes.Login.ToString())
            {
                SetCameraDefaultValues();
            
                if(playerGameObject == null)
                {
                    playerGameObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                    CameraController.CameraTarget = playerGameObject;
                    Player = playerGameObject.GetComponent<Player>();
                    Avatar = Player.Avatar;
                }
            }

            if (newScene.name == Scenes.SelectCharacter.ToString() ||
                newScene.name == Scenes.CreateCharacter.ToString())
            {
                SetCameraPreGameValues();
            }

            if(newScene.name == Scenes.World.ToString() || newScene.name == Scenes.Newcomer.ToString()) 
            {
                if(_currentState != GameState.Game)
                {
                    ChangeState(GameState.Game);

                    SetCameraGameCameraValues();
                    Destroy(playerGameObject);
                }  
            }
            else
            {
                if(_currentState != GameState.Lobby)
                {
                    ChangeState(GameState.Lobby);
                }
            }
        }

        private void SetCameraPreGameValues()
        {
            CameraController.ChangeCameraPreset("PreGameCamera");
            CameraController.ResetCameraAngles();
            CameraController.SetCameraPanAbsolutAngle(-180f);
        }

        private void SetCameraDefaultValues()
        {
            CameraController.ChangeCameraPreset("DefaultCamera");
            CameraController.ResetCameraAngles();
        }

        private void SetCameraGameCameraValues()
        {
            CameraController.ChangeCameraPreset("GameCamera");
            CameraController.ResetCameraAngles();
        }

        public GameObject CreatePlayer(Vector3 position, Quaternion rotation)
        {
            return Instantiate(playerPrefab, position, rotation);
        }
    }
}
