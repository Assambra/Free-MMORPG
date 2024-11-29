using UnityEngine;

namespace Assambra.FreeClient
{
    [RequireComponent(typeof(NetworkTransform))]
    public abstract class Entity : MonoBehaviour
    {
        public uint Id { get => _id; }
        public string Name { get => _name; }
        public GameObject EntityGameObject { get => _entityGameObject; }
        public NetworkTransform NetworkTransform { get => _networkTransform; }

        public Vector3 Position { get => gameObject.transform.position; }
        public Quaternion Rotation { get => gameObject.transform.rotation; }

        private uint _id;
        private string _name;
        private GameObject _entityGameObject;
        private NetworkTransform _networkTransform;

        private void Awake()
        {
            _networkTransform = gameObject.GetComponent<NetworkTransform>();
        }

        public virtual void Initialize(uint id, string name, GameObject entityGameObject)
        {
            this._id = id;
            this._name = name;
            this._entityGameObject = entityGameObject;
        }
    }
}


