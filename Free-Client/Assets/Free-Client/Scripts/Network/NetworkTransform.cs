using UnityEngine;

namespace Assambra.FreeClient
{
    public class NetworkTransform : MonoBehaviour
    {
        public bool IsActive { get => _isActive; set => _isActive = value; }

        private bool _isActive;

        private float _updateInterval;
        private float _elapsedTime = 0f;

        private Vector3 _startPosition;
        public Vector3 _targetPosition;

        private Quaternion _startRotation;
        public Quaternion _targetRotation;

        public void Initialize(Vector3 startPosition, Quaternion startRotation)
        {
            _startPosition = startPosition;
            _targetPosition = startPosition;
            _startRotation = startRotation;
            _targetRotation = startRotation;
        }

        private void Awake()
        {
            _updateInterval = Time.fixedDeltaTime;
        }

        private void Update()
        {
            if (_isActive)
            {
                _elapsedTime += Time.deltaTime;

                transform.position = Vector3.Lerp(_startPosition, _targetPosition, _elapsedTime / _updateInterval);
                transform.rotation = Quaternion.Lerp(_startRotation, _targetRotation, _elapsedTime / _updateInterval);

                if (_elapsedTime >= _updateInterval)
                {
                    _elapsedTime = 0f;
                    _startPosition = transform.position;
                    _startRotation = transform.rotation;
                }
            }
        }

        public void UpdateTargetPosition(Vector3 newPosition)
        {
            _targetPosition = newPosition;
            _elapsedTime = 0f;
            _startPosition = transform.position;
        }

        public void UpdateTargetRotation(Quaternion newRotation)
        {
            _targetRotation = newRotation;
            _elapsedTime = 0f;
            _startRotation = transform.rotation;
        }
    }
}
