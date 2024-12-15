using UnityEngine;
using TMPro;

namespace Assambra.FreeClient
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private string _portalName;
        [SerializeField] private TMP_Text _portalNameText;

        private void Awake()
        {
            _portalNameText.text = _portalName + " portal";
        }

        private void LateUpdate()
        {
            _portalNameText.transform.rotation = GameManager.Instance.CameraController.transform.rotation;
        }
    }
}

