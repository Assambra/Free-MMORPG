using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UMA;
using UMA.CharacterSystem;

public class ColorPicker : MonoBehaviour
{
    [SerializeField] HSVPicker.ColorPicker _colorPicker;
    [SerializeField] TMP_Text _textPickerName;
    [SerializeField] Slider _sliderGloss;
    [SerializeField] TMP_Text _textGlossValue;
    [SerializeField] Slider _sliderMetallic;
    [SerializeField] TMP_Text _textMetallicValue;
    [SerializeField] Button _buttonClose;

    private DynamicCharacterAvatar _avatar;
    private OverlayColorData _colorType;
    private string _pickerName;

    public void Initalize(DynamicCharacterAvatar avatar, OverlayColorData colorType, string name)
    {
        this._avatar = avatar;
        this._colorType = colorType;
        this._pickerName = name;

        _textPickerName.text = _pickerName;

        _colorPicker.CurrentColor = colorType.color;
        _sliderGloss.value = _colorType.channelAdditiveMask[2].a;
        _textGlossValue.text = ((float)Math.Round((double)_sliderGloss.value, 2)).ToString();
        _sliderMetallic.value = _colorType.channelAdditiveMask[2].r;
        _textMetallicValue.text = ((float)Math.Round((double)_sliderMetallic.value, 2)).ToString();

        _sliderGloss.onValueChanged.AddListener(delegate { OnSliderGlossValueChanged(); });
        _sliderMetallic.onValueChanged.AddListener(delegate { OnSliderMetallicValueChanged(); });
        
        _colorPicker.onValueChanged.AddListener(color =>
        {
            OnColorChanged(color);
        });
    }

    public void OnColorChanged(Color color)
    {
        SetColor(_colorType.name, color, new Color(_colorType.channelAdditiveMask[2].r, 0, 0), _colorType.channelAdditiveMask[2].a);
    }

    private void OnSliderGlossValueChanged()
    {
        float gloss = _sliderGloss.value;
        _textGlossValue.text = ((float)Math.Round((double)gloss, 2)).ToString();

        SetColor(_colorType.name, _colorType.color, new Color(_colorType.channelAdditiveMask[2].r, 0, 0), gloss);
    }

    private void OnSliderMetallicValueChanged()
    {
        float metallic = _sliderMetallic.value;
        _textMetallicValue.text = ((float)Math.Round((double)metallic, 2)).ToString();
        Color metallicColor = new Color(metallic, 0, 0);
        SetColor(_colorType.name, _colorType.color, metallicColor, _colorType.channelAdditiveMask[2].a);
    }

    private void SetColor(string colorName, Color color, Color metallic, float gloss)
    {
        _avatar.SetColor(colorName, color, metallic, gloss, true);
    }

    public void OnButtonClose()
    {
        Destroy(this.gameObject);
    }
}
