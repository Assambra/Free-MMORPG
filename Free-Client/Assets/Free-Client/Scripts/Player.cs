using UnityEngine;
using UMA.CharacterSystem;
using UMA;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [field: SerializeField] public DynamicCharacterAvatar Avatar { get; private set; }

    private UMAData umaData;
    private CapsuleCollider capsuleCollider;

    //public delegate void PlayerEvent();
    //public static event PlayerEvent OnPlayerHightChanged;

    private float lastHeight = 0;

    private bool doOnce;
    private bool isAvatarCreated = false;

    private void Awake()
    {
        umaData = Avatar.umaData;            
    }

    private void Update()
    {
        if(!doOnce)
        {
            doOnce = true;
            umaData.CharacterUpdated.AddListener(new UnityAction<UMAData>(OnCharacterUpdated));
            Avatar.ChangeRace("HumanMale", true);
        }
        
        if(isAvatarCreated)
        {
            if (lastHeight != capsuleCollider.height)
            {
                //Debug.Log(capsuleCollider.height);
                //OnPlayerHightChanged?.Invoke();

                float heightDiff = lastHeight - capsuleCollider.height;

                Vector3 lastOffset = GameManager.Instance.cameraController.GetCameraOffset();
                GameManager.Instance.cameraController.SetCameraOffset(new Vector3(lastOffset.x, lastOffset.y - heightDiff, lastOffset.z));

                lastHeight = capsuleCollider.height;
            }
        }
    }

    private void OnCharacterUpdated(UMAData data)
    {
        isAvatarCreated = true;
        capsuleCollider = GetPlayerCapsuleCollider();
        lastHeight = capsuleCollider.height;
    }

    private CapsuleCollider GetPlayerCapsuleCollider()
    {
        return Avatar.gameObject.GetComponent<CapsuleCollider>();
    }
}
