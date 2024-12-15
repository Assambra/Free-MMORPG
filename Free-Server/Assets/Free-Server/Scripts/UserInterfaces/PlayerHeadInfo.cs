using UnityEngine;
using TMPro;

namespace Assambra.FreeServer
{
    public class PlayerHeadInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playerNameText;

        private void LateUpdate()
        {
            transform.rotation = Camera.main.transform.rotation;
        }

        public void SetPlayerName(string playerName)
        {
            _playerNameText.text = playerName;
        }

        public void SetPlayerInfoPosition(float heightDiff)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - heightDiff, transform.position.z);
        }
    }
}

