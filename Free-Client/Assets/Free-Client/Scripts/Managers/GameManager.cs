using UnityEngine;


public class GameManager : MonoBehaviour
{
    [SerializeField] SceneHandler sceneHandler;

    public static GameManager Instance { get; private set; }


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
