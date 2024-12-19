using System;
using UnityEngine;

namespace Assambra.FreeClient.Network
{
    public class NetworkInputSender : MonoBehaviour
    {
        public bool IsActive { get => _isActive; set =>_isActive = value; }
        public Player Player { set => _player = value; }

        private bool _isActive = false;
        private Player _player;

        [SerializeField] private int _clientTick = 0;
        private int _lastTick;
        private DateTime newTickTime;
        
        private bool[] _inputArray = new bool[8];

        private void Awake()
        {
            _lastTick = _clientTick;
        }

        private void Update()
        {
            if (!_isActive)
                return;

            // To be sure that we receive the initial tick from the server before we accept any inputs
            if (_clientTick == 0)
                return;

            if (_clientTick > _lastTick)
            {
                newTickTime = DateTime.Now;
                _lastTick = _clientTick;
            }
        }

        private void FixedUpdate()
        {
            if (_clientTick > 0)
                _clientTick++;
        }

        public void SetClientTick(int _serverTick)
        {
            _clientTick = _serverTick;
            _player.PlayerController.IsActive = true;
        }
    }
}


