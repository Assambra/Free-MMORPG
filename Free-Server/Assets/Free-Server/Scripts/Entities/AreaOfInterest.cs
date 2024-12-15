using System.Collections.Generic;
using UnityEngine;

namespace Assambra.FreeServer
{
    [RequireComponent(typeof(SphereCollider))]
    public class AreaOfInterest : MonoBehaviour
    {
        [SerializeField] SphereCollider _triggerCollider;
        public Dictionary<uint, Player> NearbyPlayers { get => _nearbyPlayers; }
        public delegate void PlayerInteraction(Player player);
        public event PlayerInteraction PlayerEntered;
        public event PlayerInteraction PlayerExited;

        private Dictionary<uint, Player> _nearbyPlayers = new Dictionary<uint, Player>();

        private void Start()
        {
            _triggerCollider.isTrigger = true;
            _triggerCollider.center = new Vector3(0, 1, 0);
            _triggerCollider.radius = ServerConstants.AREA_OF_INTEREST;
        }

        private void OnTriggerEnter(Collider other)
        {
            Entity entity = GetComponentInParent<Entity>();
            Player otherPlayer = other.GetComponent<Player>();
            ServerManager.Instance.ServerLog.ServerLogMessageInfo($"{entity.Name} OnTriggerEnter has detected {otherPlayer.Name} entering the area.");
            if (otherPlayer != null)
            {
                uint id = otherPlayer.Id;

                if (!_nearbyPlayers.ContainsKey(id))
                {
                    _nearbyPlayers.Add(id, otherPlayer);
                    PlayerEntered?.Invoke(otherPlayer);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Entity entity = GetComponentInParent<Entity>();
            Player otherPlayer = other.GetComponent<Player>();
            ServerManager.Instance.ServerLog.ServerLogMessageInfo($"{entity.Name} OnTriggerExit has detected {otherPlayer.Name} entering the area.");
            if (otherPlayer != null)
            {
                uint id = otherPlayer.Id;

                if (_nearbyPlayers.ContainsKey(id))
                {
                    _nearbyPlayers.Remove(id);
                    PlayerExited?.Invoke(otherPlayer);
                }
            }
        }
    }
}


