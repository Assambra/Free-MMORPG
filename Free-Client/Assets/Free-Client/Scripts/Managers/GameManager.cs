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

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        Player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        cameraController.CameraTarget = Player;
        Avatar = Player.GetComponent<Player>().Avatar;
    }

    private void Update()
    {
        cameraController.IsOverUIElement = mouseHandler.IsOverUIElement;
    }

    public void ChangeScene(Scenes scene)
    {
        sceneHandler.CurrentScene = sceneHandler.Scenes[(int)scene];
    }
}
