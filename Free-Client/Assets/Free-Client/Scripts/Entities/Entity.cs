using UnityEngine;

namespace Assambra.FreeClient
{
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] protected EntityNameInfo _entityNameInfo;
        [SerializeField] protected EntityModel _entityModel;

        public EntityModel EntityModel { get => _entityModel; }
        public NetworkTransform NetworkTransform { get => _networkTransform; }

        public Vector3 Position { get => gameObject.transform.position; }
        public Quaternion Rotation { get => gameObject.transform.rotation; }

        protected NetworkTransform _networkTransform;

        private void Awake()
        {
            _networkTransform = gameObject.AddComponent<NetworkTransform>();
        }

        public virtual void Initialize(EntityModel entityModel, GameObject entityGameObject)
        {
            this._entityModel = entityModel;
            _entityModel.SetEntityGameObject(entityGameObject);
            SetEntityName(_entityModel.Name);
        }

        private void SetEntityName(string playerName)
        {
            gameObject.name = playerName;
            _entityNameInfo.SetName(playerName);
        }
    }
}


