using TMPro;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface
{
    public class ColorSelectorElement : MonoBehaviour
    {
        [SerializeField] private Button _buttonColor;
        [SerializeField] private Image _imageColor;
        [SerializeField] private GameObject _prefabColorPicker;
        [SerializeField] private TMP_Text _textColorName;

        private DynamicCharacterAvatar _avatar;

        private OverlayColorData _colorType;
        private Transform _colorPickerHome;
        private string _name;
        private GameObject _colorPicker;

        private void Update()
        {
            _imageColor.color = _colorType.color;
        }

        private void OnDestroy()
        {
            RemoveColorPicker();
        }

        public void Initialize(DynamicCharacterAvatar avatar, OverlayColorData colorType, string name, Transform colorPickerHome)
        {
            this._avatar = avatar;
            this._colorType = colorType;

            this._colorPickerHome = colorPickerHome;
            this._name = name;
            _textColorName.text = name;
        }

        public void RemoveColorPicker()
        {
            if (_colorPicker != null)
                Destroy(_colorPicker);
        }

        public void OnButtonColor()
        {
            if (_colorPicker == null)
            {
                _colorPicker = Instantiate(_prefabColorPicker, _colorPickerHome);
                ColorPicker cpo = _colorPicker.GetComponent<ColorPicker>();
                cpo.Initalize(_avatar, _colorType, _name);
            }
            else
            {
                Destroy(_colorPicker);
            }
        }
    }
}
