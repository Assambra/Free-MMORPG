using System;
using UnityEngine;

namespace Assambra.FreeClient
{
    [Serializable]
    public class EntityModel
    {
        [SerializeField] private GameObject _entityGameObject;
        [SerializeField] private long _id;
        [SerializeField] private EntityType _entityType;
        [SerializeField] private string _name;
        [SerializeField] private string _model;
        [SerializeField] private Vector3 _position;
        [SerializeField] private Quaternion _rotation;
        [SerializeField] private string _sex;
        [SerializeField] private string _race;
        [SerializeField] private string _room;
        [SerializeField] private bool _isLocalPlayer;

        public GameObject EntityGameObject => _entityGameObject;
        public long Id => _id;
        public EntityType EntityType => _entityType;
        public string Name => _name;
        public string Model => _model;
        public Vector3 Position => _position;
        public Quaternion Rotation => _rotation;
        public string Sex => _sex;
        public string Race => _race;
        public string Room => _room;
        public bool IsLocalPlayer => _isLocalPlayer;

        public EntityModel(long id, int entityType, string name, string model, Vector3 position, Quaternion rotation, string sex = null, string race = null, string room = null, bool isLocalPlayer = false)
        {
            this._id = id;
            this._entityType = (EntityType)entityType;
            this._name = name;
            this._model = model;
            this._position = position;
            this._rotation = rotation;
            this._sex = sex;
            this._race = race;
            this._room = room;
            this._isLocalPlayer = isLocalPlayer;
        }

        public void SetEntityGameObject(GameObject go)
        {
            this._entityGameObject = go;
        }
    }
}

