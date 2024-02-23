using Assambra.GameFramework.GameManager;
using CharacterInfo = Assambra.FreeClient.Entities.CharacterInfo;
using Assambra.FreeClient.Helper;
using Assambra.FreeClient.Managers;
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
        [SerializeField] private Button _buttonPreviousCharacter;
        [SerializeField] private Button _buttonNextCharacter;

        private List<CharacterInfo> _characterInfos;
        private int _charactersCount;
        private DynamicCharacterAvatar _avatar;
        private int _currentShownCharacter;

        private void OnEnable()
        {
            _characterInfos = GameManager.Instance.CharacterInfos;

            if (_characterInfos.Count > 0)
            {
                _avatar = GameManager.Instance.Avatar;

                UMAHelper.SetAvatarString(_avatar, _characterInfos[0].model);

                _currentShownCharacter = 0;

                SetCharacter(_currentShownCharacter);
            }
            else
            {
                // Todo inform the player that no characters available
                // actually obsolete we can't go back from create character scene if we have zero characters
                // and also after login we check if zero characters go straight to create character scene
                // But we need to look into again if we create a option to delete the character.
                Debug.Log("Todo inform the player that no characters available");
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
            GameManager.Instance.ChangeScene(Scenes.Login);
            NetworkManagerGame.Instance.Disconnect();
        }

        public void OnButtonPlay()
        {
            NetworkManagerGame.Instance.PlayRequest(_characterInfos[_currentShownCharacter].id);
        }

        public void OnButtonNewCharacter()
        {
            GameManager.Instance.ChangeScene(Scenes.CreateCharacter);
        }

        private void SetCharacter(int character)
        {
            CharacterInfo info = _characterInfos[character];
            _textCharacterName.text = info.name;
            _textCharacterSex.text = info.sex;
            _textCharacterRace.text = info.race;
            UMAHelper.SetAvatarString(_avatar, info.model);
        }
    }
}
