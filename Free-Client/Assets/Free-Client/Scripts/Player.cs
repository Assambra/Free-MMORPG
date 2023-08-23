using UnityEngine;
using UMA;
using UMA.CharacterSystem;

public class Player : MonoBehaviour
{
    [field: SerializeField] public DynamicCharacterAvatar Avatar { get; private set; }

    private CapsuleCollider capsuleCollider;

    private void Update()
    {
    }

    public Vector3 GetCenterOfThePlayer()
    {
        capsuleCollider = Avatar.gameObject.GetComponent<CapsuleCollider>();
        
        if (capsuleCollider != null)
            return capsuleCollider.center;
        else 
            return Vector3.zero;
    }
}
