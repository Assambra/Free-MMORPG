using UnityEngine;
using UMA.CharacterSystem;
using UMA;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public bool IsLocalPlayer = false;
    public bool Initialized = false;
    public bool IsAvatarCreated = false;
    public Animator Animator;

    [field: SerializeField] public DynamicCharacterAvatar Avatar { get; private set; }
    
    [SerializeField] private PlayerHeadInfo playerHeadInfo;

    private UMAData umaData;
    private CapsuleCollider capsuleCollider;

    private float lastHeight = 0;
    private float currentHeight = 0;

    private void Update()
    {
        if(!Initialized)
        {
            umaData = Avatar.umaData;
            umaData.CharacterUpdated.AddListener(new UnityAction<UMAData>(OnCharacterInitialize));
            Avatar.ChangeRace("HumanMale", true);
            Initialized = true;
        }
        
        if(IsAvatarCreated)
        {
            currentHeight = capsuleCollider.height;
            if (currentHeight > lastHeight || currentHeight < lastHeight)
            {
                SetCameraOffset(lastHeight);
                SetCameraDistance(lastHeight);

                playerHeadInfo.SetPlayerInfoPosition(lastHeight - currentHeight);

                lastHeight = currentHeight;
            }
        }
    }

    public void SetPlayerName(string playerName)
    {
        playerHeadInfo.SetPlayername(playerName);
    }

    private void SetCameraOffset(float lastHeight)
    {
        Vector3 lastOffset = GameManager.Instance.CameraController.GetCameraOffset();
        float newCameraOffsetY = (currentHeight / lastHeight) * lastOffset.y;
        GameManager.Instance.CameraController.SetCameraOffset(new Vector3(lastOffset.x, newCameraOffsetY, lastOffset.z));
    }

    private void SetCameraDistance(float lastHeight)
    {
        float cameraFieldOfView = GameManager.Instance.CameraController.GetCameraFieldOfView();
        float currentCameraDistance = GameManager.Instance.CameraController.GetCameraDistance();
        float cameraDistanceOffset = CalculateCameraDistanceOffset(cameraFieldOfView, lastHeight, currentHeight);

        GameManager.Instance.CameraController.SetCameraDistance(currentCameraDistance + cameraDistanceOffset);
    }

    private float CalculateCameraDistanceOffset(float fieldOfView, float lastHeight, float currentHeight)
    {
        float lastCameraDistance = lastHeight / Mathf.Tan((fieldOfView / 2) * Mathf.Deg2Rad);
        float currentCameraDistance = currentHeight / Mathf.Tan((fieldOfView / 2) * Mathf.Deg2Rad);
        float cameraDistanceDiff = currentCameraDistance - lastCameraDistance;
        
        return cameraDistanceDiff;
    }

    private void OnCharacterInitialize(UMAData data)
    {
        umaData.CharacterUpdated.RemoveListener(new UnityAction<UMAData>(OnCharacterInitialize));
        IsAvatarCreated = true;
        capsuleCollider = GetCapsuleCollider();
        lastHeight = capsuleCollider.height;
    }

    public CapsuleCollider GetCapsuleCollider()
    {
        return Avatar.transform.GetComponent<CapsuleCollider>();
    }
}
