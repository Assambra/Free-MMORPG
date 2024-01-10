using System.Collections;
using System.Collections.Generic;
using UMA.CharacterSystem;
using UnityEngine;


public class GameManager : BaseGameManager
{
    [field: SerializeField] public CameraController CameraController { get; private set; }
    [field: SerializeField] public UIHandler UIHandler { get; private set; }
    [field: SerializeField] public SceneHandler SceneHandler { get; private set; }
    [field: SerializeField] public MouseHandler MouseHandler { get; private set; }
    [field: SerializeField] public Light DirectionalLight { get; private set; }
    [field: SerializeField] public Camera MainCamera { get; private set; }

    public Player Player { get; private set; }
    public DynamicCharacterAvatar Avatar { get; private set; }
    public List<CharacterInfo> CharacterInfos { get; set; } = new List<CharacterInfo>();
    public List<Character> CharacterList { get; set; } = new List<Character>();
    public Dictionary<string, PlayerController> PlayerSyncPositionDictionary = new Dictionary<string, PlayerController>();
    
    public bool CharacterCreatedAndReadyToPlay = false;
    public long CharacterId = 0;

    [Header("Player Prefab")]
    [SerializeField] GameObject playerPrefab;
    
    // Private
    private GameObject playerGameObject;

    protected override void Awake()
    {
        base.Awake();

        Application.targetFrameRate = 60;
    }


    private void Update()
    {
        CameraController.IsOverUIElement = MouseHandler.IsOverUIElement;
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

        if(newScene.name == Scenes.World.ToString())
        {
            SetCameraGameCameraValues();
            Destroy(playerGameObject);
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

    public GameObject SpawnPlayer(Character character)
    {
        GameObject pgo = GameObject.Instantiate(playerPrefab, character.position, Quaternion.Euler(character.rotation));
        pgo.name = character.characterName;
        PlayerController playerController = pgo.AddComponent<PlayerController>();

        PlayerSyncPositionDictionary.Add(character.accountUsername, playerController);

        Player player = pgo.GetComponent<Player>();
        playerController.Player = player;

        player.SetPlayerName(character.characterName);

        if(character.isLocalPlayer)
        {
            player.IsLocalPlayer = true;
            
            CameraController.CameraTarget = pgo;
            CameraController.ResetCameraAngles();
        }
        
        StartCoroutine(WaitForCharacterCreated(player, character.characterModel));

        return pgo;
    }

    IEnumerator WaitForCharacterCreated(Player player, string model)
    {
        while (!player.Initialized && !player.IsAvatarCreated)
        {
            Debug.Log("WaitForCharacterCreated");
            yield return new WaitForSeconds(0.05f);
        }
        
        player.Animator = player.Avatar.GetComponent<Animator>();
        player.GetCapsuleCollider();
        UMAHelper.SetAvatarString(player.Avatar, model);
    }
}
