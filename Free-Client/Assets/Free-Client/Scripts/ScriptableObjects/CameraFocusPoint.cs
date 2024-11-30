using UMA.CharacterSystem;
using UnityEngine;

namespace Assambra.FreeClient.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CameraFocusPoint", menuName = "Assambra/CameraFocusPoint", order = 1)]
    public class CameraFocusPoint : ScriptableObject
    {
        public string BoneName;
        public Vector3 FocusPoint;

        private DynamicCharacterAvatar avatar;
        private Transform hips;
        private GameObject refferenceToBone;


        private void OnDisable()
        {
            FocusPoint = Vector3.zero;
            avatar = null;
            hips = null;
            refferenceToBone = null;
        }

        public void UpdateFocusPoint()
        {
            FocusPoint = refferenceToBone.transform.position;
        }

        public void GetFocusPoint()
        {
            avatar = GameManager.Instance.Avatar;
            hips = avatar.transform.Find("Root").Find("Global").Find("Position").Find("Hips");

            if (hips != null)
            {
                Transform[] boneTransformes = hips.gameObject.GetComponentsInChildren<Transform>();

                foreach (Transform bone in boneTransformes)
                {
                    if (bone.name == BoneName)
                    {
                        FocusPoint = bone.transform.position;
                        refferenceToBone = bone.gameObject;

                        break;
                    }
                }
            }
            else
                Debug.LogError("No Hips Transform found!");
        }
    }
}
