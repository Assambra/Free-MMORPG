using UnityEngine;
using TMPro;

namespace Assambra.FreeClient
{
    public class EntityNameInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;

        private void LateUpdate()
        {
            transform.rotation = GameManager.Instance.CameraController.transform.rotation;
        }

        public void SetName(string name)
        {
            _nameText.text = name;
        }

        public void SetNameInfoPosition(float heightDiff)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - heightDiff, transform.position.z);
        }
    }
}
