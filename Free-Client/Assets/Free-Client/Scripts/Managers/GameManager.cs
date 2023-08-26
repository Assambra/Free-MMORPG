using System.Collections.Generic;
using UMA.CharacterSystem;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [field: SerializeField] public CameraController cameraController { get; private set; }

    public GameObject Player;
    public DynamicCharacterAvatar Avatar { get; private set; }

    public List<CharacterInfo> characterInfos = new List<CharacterInfo>();
    public static GameManager Instance { get; private set; }

    [Header("Script references")]
    [SerializeField] SceneHandler sceneHandler;
    [SerializeField] MouseHandler mouseHandler;
    
    
    [Header("PlayerPrefab")]
    [SerializeField] GameObject playerPrefab;

    private bool cameraInitialized =  false;

    private void Awake()
    {
        SceneHandler.OnSceneChanged += OnSceneChanged;

        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        Player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        cameraController.CameraTarget = Player;
        Avatar = Player.GetComponent<Player>().Avatar;
    }

    private void OnDestroy()
    {
        SceneHandler.OnSceneChanged -= OnSceneChanged;
    }

    private void Update()
    {
        cameraController.IsOverUIElement = mouseHandler.IsOverUIElement;
    }

    public void ChangeScene(Scenes scene)
    {
        sceneHandler.CurrentScene = sceneHandler.Scenes[(int)scene];
    }

    private void OnSceneChanged()
    {
        if ((sceneHandler.CurrentScene.name == Scenes.SelectCharacter.ToString() ||
            sceneHandler.CurrentScene.name == Scenes.CreateCharacter.ToString()) &&
            !cameraInitialized
            )
        {
            SetCameraPreGameValues();
            
            if(sceneHandler.CurrentScene.name == Scenes.CreateCharacter.ToString())
            {
                //Avatar.ChangeRace("HumanMale", true);
            }
        }

        if(sceneHandler.CurrentScene.name == Scenes.Login.ToString() && 
            cameraInitialized)
        {
            SetCameraDefaultValues();
        }
    }

    private void SetCameraPreGameValues()
    {
        cameraController.ChangeCameraPreset("PreGameCamera");
        cameraController.SetCameraPanAbsolutAngle(-180f);

        cameraInitialized = true;
    }

    private void SetCameraDefaultValues()
    {
        cameraController.ChangeCameraPreset("DefaultCamera");
        cameraController.ResetCameraAngles();

        cameraInitialized = false;
    }
}
