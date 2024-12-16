using UnityEngine;
using TMPro;

namespace Assambra.FreeServer
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private TMP_Text _portalNameText;
        [SerializeField] string _room = "World";
        [SerializeField] Vector3 _position;
        [SerializeField] Quaternion _rotation;

        private string _playerTag = "Player";

        private void Awake()
        {
            _portalNameText.text = _room + " portal";
        }

        private void LateUpdate()
        {
            _portalNameText.transform.rotation = Camera.main.transform.rotation;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
                return;

            if (other.TryGetComponent(out Player player))
            {
                ServerManager.Instance.ServerLog.ServerLogMessageInfo($"Player {player.Name} has entered the portal.");

                NetworkManager.Instance.SendChangeServerRequest(player.Id, _room, _position, _rotation.eulerAngles);
            }
        }
    }
}
