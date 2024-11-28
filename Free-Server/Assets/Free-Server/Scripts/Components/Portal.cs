using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeServer
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private Text _portalNameText;
        [SerializeField] string _room = "World";
        [SerializeField] Vector3 _position;
        [SerializeField] Vector3 _rotation;

        private string _playerTag = "Player";

        private void Awake()
        {
            _portalNameText.text = _room + " portal";
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_playerTag))
            {
                Player player = other.GetComponent<Player>();

                ServerManager.Instance.ServerLog.ServerLogMessageInfo($"Player {player.Name} has entered the portal.");

                NetworkManager.Instance.SendChangeServerRequest(player.Id, _room, _position, _rotation);
            }
        }
    }
}
