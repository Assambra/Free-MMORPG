using System.Collections.Generic;
using UnityEngine;

namespace Assambra.FreeServer
{
    [RequireComponent(typeof(SphereCollider))]
    public class AreaOfInterest : MonoBehaviour
    {
        [SerializeField] Entity _entity;
        [SerializeField] SphereCollider _triggerCollider;
        public Dictionary<uint, Player> NearbyPlayers { get => _nearbyPlayers; }
        public delegate void PlayerInteraction(Player player);
        public event PlayerInteraction PlayerEntered;
        public event PlayerInteraction PlayerExited;

        private Dictionary<uint, Player> _nearbyPlayers = new Dictionary<uint, Player>();

        private void Start()
        {
            if (_triggerCollider == null)
                _triggerCollider = gameObject.GetComponent<SphereCollider>();

            _triggerCollider.isTrigger = true;
            _triggerCollider.center = new Vector3(0, 1, 0);
            _triggerCollider.radius = ServerConstants.AREA_OF_INTEREST;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out Entity otherEntity))
            {
                if (otherEntity == _entity)
                    return;
            }

            if (other.TryGetComponent(out Player otherPlayer))
            {
                uint id = otherPlayer.Id;

                ServerManager.Instance.ServerLog.ServerLogMessageInfo(
                    $"{_entity.Name} OnTriggerEnter detected {otherPlayer.Name} entering the area."
                );

                if (!_nearbyPlayers.ContainsKey(id))
                {
                    _nearbyPlayers.Add(id, otherPlayer);
                    PlayerEntered?.Invoke(otherPlayer);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Entity otherEntity))
            {
                if (otherEntity == _entity)
                    return;
            }

            if (other.TryGetComponent(out Player otherPlayer))
            {
                ServerManager.Instance.ServerLog.ServerLogMessageInfo(
                    $"{_entity.Name} OnTriggerExit has detected {otherPlayer.Name} leaving the area."
                );

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


