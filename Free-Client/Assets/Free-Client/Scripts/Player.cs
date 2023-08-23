using UnityEngine;
using UMA;
using UMA.CharacterSystem;

public class Player : MonoBehaviour
{
    [field: SerializeField] public DynamicCharacterAvatar Avatar { get; private set; }

    private CapsuleCollider capsuleCollider;

    private void Update()
    {
        GetCenterOfThePlayer();
    }

    public Vector3 GetCenterOfThePlayer()
    {
        capsuleCollider = Avatar.gameObject.GetComponent<CapsuleCollider>();
        
        if (capsuleCollider != null)
        {
            Debug.Log(capsuleCollider.center);
            return capsuleCollider.center;
        }
        else
        {
            Debug.Log("No Capsule Colider found!");
            return Vector3.zero;
        }
    }
}
