using UnityEngine;

public class Character 
{
    public string accountUsername { get; private set; }
    public long roomId { get; private set; }
    public bool isLocalPlayer { get; private set; }
    public string characterName { get; private set; }
    public string characterModel { get; private set; }
    public Vector3 position { get; private set; }
    public Vector3 rotation { get; private set; }

    public GameObject playerGameObject;

    public Character(string accountUsername, long roomId, bool isLocalPlayer, string characterName, string characterModel, Vector3 position, Vector3 rotation)
    {
        this.accountUsername = accountUsername;
        this.roomId = roomId;
        this.isLocalPlayer = isLocalPlayer;
        this.characterName = characterName;
        this.characterModel = characterModel;
        this.position = position;
        this.rotation = rotation;
    }

    public void SetPlayerGameObject(GameObject playerGameObject)
    {
        this.playerGameObject = playerGameObject;
    }
}
