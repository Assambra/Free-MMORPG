using UnityEngine;

namespace Assambra.FreeServer
{
    public class Player : Entity
    {
        [SerializeField] private PlayerHeadInfo _playerHeadInfo;

        public string Username { get => _username; }
        public bool MasterServerRequestedDespawn { get => _masterServerRequestedDespawn; set => _masterServerRequestedDespawn = value; }

        private string _username;
        private bool _masterServerRequestedDespawn;

        public void Initialize(uint id, string name, string sex, string race, string model, GameObject entityGameObject, bool isStatic, EntityType entityType, string username)
        {
            base.Initialize(id, name, sex, race, model, entityGameObject, isStatic, entityType);
            this._username = username;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void SetPlayerHeadinfoName(string playerName)
        {
            _playerHeadInfo.SetPlayerName(playerName);
        }
    }
}

