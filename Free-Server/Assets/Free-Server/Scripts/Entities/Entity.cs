using System.Collections.Generic;
using UnityEngine;

namespace Assambra.FreeServer
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CapsuleCollider _capsuleCollider;
        [SerializeField] private AreaOfInterest _areaOfInterest;

        public Dictionary<uint, Player> NearbyPlayers { get => _areaOfInterest.NearbyPlayers; }
        public uint Id { get => _id; }
        public string Name { get => _name; }
        public string Sex { get => _sex; }
        public string Race { get => _race; }
        public string Model { get => _model; }
        public GameObject EntityGameObject { get => _entityGameObject; }
        public bool IsStatic { get => _isStatic; }
        public EntityType EntityType { get => _entityType; }

        public Vector3 Position { get => gameObject.transform.position; }
        public Quaternion Rotation { get => gameObject.transform.rotation; }

        private uint _id;
        private string _name;
        private string _sex;
        private string _race;
        private string _model;
        private GameObject _entityGameObject;
        private bool _isStatic;
        private EntityType _entityType;

        private Vector3 _position;
        private Vector3 _rotation;

        private Vector3 _lastPosition;
        private Quaternion _lastRotation;

        public virtual void Initialize(uint id, string name, string sex, string race, string model, GameObject entityGameObject, bool isStatic, EntityType entityType)
        {
            this._id = id;
            this._name = name;
            this._sex = sex;
            this._race = race;
            this._model = model;
            this._entityGameObject = entityGameObject;
            this._isStatic = isStatic;
            this._entityType = entityType;
        }

        protected virtual void Awake()
        {
            _areaOfInterest.PlayerEntered += OnPlayerEntered;
            _areaOfInterest.PlayerExited += OnPlayerExited;

            _lastPosition = transform.position;
            _lastRotation = transform.rotation;
        }

        protected virtual void OnDestroy()
        {
            _areaOfInterest.PlayerEntered -= OnPlayerEntered;
            _areaOfInterest.PlayerExited -= OnPlayerExited;

            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
        }

        private void FixedUpdate()
        {
            if (_isStatic)
                return;

            if (_lastPosition != transform.position || _lastRotation != transform.rotation)
            {
                Player player = gameObject.GetComponent<Player>();

                foreach (KeyValuePair<uint, Player> entry in _areaOfInterest.NearbyPlayers)
                {
                    NetworkManager.Instance.SendUpdateEntityPosition(entry.Value.Username, _id, transform.position, transform.rotation.eulerAngles);
                }
            }

            _lastPosition = transform.position;
            _lastRotation = transform.rotation;
        }

        private void OnPlayerEntered(Player otherPlayer)
        {
            Player player = gameObject.GetComponent<Player>();
            ServerManager.Instance.ServerLog.ServerLogMessageInfo($"{Name} has detected {otherPlayer.Name} entering the area.");
            if (player != null)
            {
                NetworkManager.Instance.SendSpawnToPlayer(player.Username, otherPlayer.Id, otherPlayer.Name, otherPlayer.Sex, otherPlayer.Race, otherPlayer.Model, otherPlayer.Position, otherPlayer.Rotation.eulerAngles);
            }
        }

        private void OnPlayerExited(Player otherPlayer)
        {
            ServerManager.Instance.ServerLog.ServerLogMessageInfo($"{Name} has detected {otherPlayer.Name} leaving the area.");

            Player player = gameObject.GetComponent<Player>();

            if (player != null)
            {
                if (!otherPlayer.MasterServerRequestedDespawn)
                {
                    NetworkManager.Instance.SendDespawnToPlayer(player.Username, otherPlayer.Id);
                }
            }
        }
    }
}