using System.Collections.Generic;
using UMA.CharacterSystem;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: SerializeField] public CameraController CameraController { get; private set; }
    [field: SerializeField] public UIHandler UIHandler { get; private set; }
    [field: SerializeField] public SceneHandler SceneHandler { get; private set; }
    [field: SerializeField] public MouseHandler MouseHandler { get; private set; }

    public Player Player { get; private set; }
    public DynamicCharacterAvatar Avatar { get; private set; }
    public List<CharacterInfo> CharacterInfos { get; set; } = new List<CharacterInfo>();

    [Header("Player Prefab")]
    [SerializeField] GameObject playerPrefab;
    
    // Private
    private GameObject playerGameObject;
    private bool cameraInitialized =  false;


    private void Awake()
    {
        SceneHandler.OnSceneChanged += OnSceneChanged;

        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        playerGameObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        CameraController.CameraTarget = playerGameObject;
        Player = playerGameObject.GetComponent<Player>();
        Avatar = Player.Avatar;
    }

    private void OnDestroy()
    {
        SceneHandler.OnSceneChanged -= OnSceneChanged;
    }

    private void Update()
    {
        CameraController.IsOverUIElement = MouseHandler.IsOverUIElement;
    }

    public void ChangeScene(Scenes scene)
    {
        SceneHandler.CurrentScene = SceneHandler.Scenes[(int)scene];
    }

    private void OnSceneChanged()
    {
        if ((SceneHandler.CurrentScene.name == Scenes.SelectCharacter.ToString() ||
            SceneHandler.CurrentScene.name == Scenes.CreateCharacter.ToString()) &&
            !cameraInitialized
            )
        {
            SetCameraPreGameValues();
            
            if(SceneHandler.CurrentScene.name == Scenes.CreateCharacter.ToString())
            {
                //Avatar.ChangeRace("HumanMale", true);
            }
        }

        if(SceneHandler.CurrentScene.name == Scenes.Login.ToString() && 
            cameraInitialized)
        {
            SetCameraDefaultValues();
        }
    }

    private void SetCameraPreGameValues()
    {
        CameraController.ChangeCameraPreset("PreGameCamera");
        CameraController.SetCameraPanAbsolutAngle(-180f);

        cameraInitialized = true;
    }

    private void SetCameraDefaultValues()
    {
        CameraController.ChangeCameraPreset("DefaultCamera");
        CameraController.ResetCameraAngles();

        cameraInitialized = false;
    }
}
