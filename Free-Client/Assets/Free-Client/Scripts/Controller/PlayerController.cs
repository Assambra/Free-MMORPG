using UnityEngine;

namespace Assambra.FreeClient
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public bool IsActive { get => _isActive; set => _isActive = value; }

        public Player Player { get => _player; set => _player = value; }
        public CharacterController CharacterController { get => _characterController; }
        
        private bool _isActive;
        private Player _player;

        private CharacterController _characterController;
        private Vector3 _input;
        private Vector3 _move;
        private bool _jump;
        private Vector3 _playerVelocity;
        private bool _groundedPlayer;
        private float _playerSpeed = 5.0f;
        private float _rotationSpeed = 150f;
        private float _jumpHeight = 1.0f;
        private float _gravityValue = -9.81f;

        private bool sendOnceZero = false;

        [SerializeField] private int _clientTick = 0;

        private void Awake()
        {
            _characterController = gameObject.GetComponent<CharacterController>();
        }

        private void Start()
        {
            _characterController.center = new Vector3(0, 1, 0);
        }

        void Update()
        {
            if (!_isActive)
                return;

            if (!_player.EntityModel.IsLocalPlayer)
                return;

            // To be sure that we receive the initial tick from the server before we accept any inputs
            if (_clientTick == 0)
                return;

            _groundedPlayer = _characterController.isGrounded;
            if (_groundedPlayer && _playerVelocity.y < 0)
            {
                _playerVelocity.y = 0f;
            }

            _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            
            _characterController.Move(transform.forward * _move.z * Time.deltaTime * _playerSpeed);
            transform.Rotate(new Vector3(0, _move.x * _rotationSpeed * Time.deltaTime, 0));

            if (Input.GetButtonDown("Jump") && _groundedPlayer)
            {
                _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravityValue);
                _jump = true;
            }

            _playerVelocity.y += _gravityValue * Time.deltaTime;
            _characterController.Move(_playerVelocity * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if(!_isActive)
                return;
            if (!_player.EntityModel.IsLocalPlayer)
                return;

            if (_clientTick > 0)
                _clientTick++;

            if(_jump)
            {
                NetworkManager.Instance.SendPlayerJump(_player.EntityModel.Id, _player.EntityModel.Room);
                _jump = false;
            }
            if (_input != Vector3.zero || !sendOnceZero)
            {
                if(_input == Vector3.zero)
                    sendOnceZero = true;
                else
                    sendOnceZero = false;
                
                NetworkManager.Instance.SendPlayerInput(_player.EntityModel.Id, _player.EntityModel.Room, _input);
                _move = _input;
            }                
        }

        public void SetClientTick(int _serverTick)
        {
            _clientTick = _serverTick;
        }
    }
}

