using UMA;
using UMA.CharacterSystem;
using UnityEngine;
using UnityEngine.UI;

public class ColorElement : MonoBehaviour
{
    [SerializeField] Button _buttonColor;
    [SerializeField] Image _imageColor;
    [SerializeField] GameObject _prefabColorPicker;

    private DynamicCharacterAvatar _avatar;

    private OverlayColorData _colorType;
    private Transform _colorPickerHome;
    private string _name;
    private GameObject _colorPicker;

    void Update()
    {
        _imageColor.color = _colorType.color; 
    }

    public void Initialize(DynamicCharacterAvatar avatar, OverlayColorData colorType, string name, Transform colorPickerHome)
    {
        this._avatar = avatar;
        this._colorType = colorType;

        this._colorPickerHome = colorPickerHome;
        this._name = name;
    }

    public void RemoveColorPicker()
    {
        Destroy(_colorPicker);
    }

    public void OnButtonColor()
    {
        if(_colorPicker == null)
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
