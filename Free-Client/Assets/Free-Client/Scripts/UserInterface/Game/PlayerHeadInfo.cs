using UnityEngine;


public class PlayerHeadInfo : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = GameManager.Instance.CameraController.transform.rotation;
    }
}
