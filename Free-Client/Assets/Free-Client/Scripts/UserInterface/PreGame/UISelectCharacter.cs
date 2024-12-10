using Assambra.GameFramework.GameManager;
using Assambra.FreeClient.Helper;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UMA.CharacterSystem;

namespace Assambra.FreeClient.UserInterface
{
    public class UISelectCharacter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textCharacterName;
        [SerializeField] private TMP_Text _textCharacterSex;
        [SerializeField] private TMP_Text _textCharacterRace;
        [SerializeField] private TMP_Text _textCharacterLocation;
        [SerializeField] private Button _buttonPreviousCharacter;
        [SerializeField] private Button _buttonNextCharacter;

        private List<EntityModel> _characterInfos;
        private int _charactersCount;
        private DynamicCharacterAvatar _avatar;
        private int _currentShownCharacter;

        private void Start()
        {
            _characterInfos = GameManager.Instance.CharacterInfos;

            if (_characterInfos.Count > 0)
            {
                _avatar = GameManager.Instance.Avatar;

                UMAHelper.SetAvatarString(_avatar, _characterInfos[0].Model);

                _currentShownCharacter = 0;

                SetCharacter(_currentShownCharacter);
            }

            _charactersCount = _characterInfos.Count - 1;
        }

        public void OnButtonPreviousCharacter()
        {
            if (_charactersCount >= 0)
            {
                if (_currentShownCharacter == 0)
                    _currentShownCharacter = _charactersCount;
                else
                    _currentShownCharacter--;

                SetCharacter(_currentShownCharacter);
            }
        }

        public void OnButtonNextCharacter()
        {
            if (_charactersCount >= 0)
            {
                if (_currentShownCharacter == _charactersCount)
                    _currentShownCharacter = 0;
                else
                    _currentShownCharacter++;

                SetCharacter(_currentShownCharacter);
            }
        }

        public void OnButtonBackToLogin()
        {
            NetworkManager.Instance.LoginState = LoginState.Lobby;
            GameManager.Instance.ChangeScene(Scenes.Login);
        }

        public void OnButtonPlay()
        {
            NetworkManager.Instance.PlayRequest(_characterInfos[_currentShownCharacter].Id);
        }

        public void OnButtonNewCharacter()
        {
            GameManager.Instance.ChangeScene(Scenes.CreateCharacter);
        }

        private void SetCharacter(int character)
        {
            EntityModel info = _characterInfos[character];
            _textCharacterName.text = info.Name;
            _textCharacterSex.text = info.Sex;
            _textCharacterRace.text = info.Race;
            _textCharacterLocation.text = info.Room;
            UMAHelper.SetAvatarString(_avatar, info.Model);
        }
    }
}
