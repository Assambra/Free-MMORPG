using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public bool IsPersistentCamera;

    private void Awake()
    {
        if(!IsPersistentCamera)
            gameObject.SetActive(false);
    }
}
