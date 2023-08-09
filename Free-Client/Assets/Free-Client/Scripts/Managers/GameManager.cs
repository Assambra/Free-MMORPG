using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [SerializeField] SceneHandler sceneHandler;

    public static GameManager Instance { get; private set; }

    public List<CharacterInfo> characterInfos = new List<CharacterInfo>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void ChangeScene(Scenes scene)
    {
        sceneHandler.CurrentScene = sceneHandler.Scenes[(int)scene];
    }
}
