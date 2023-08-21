using System;
using UnityEngine;
using HSVPicker;
using UMA;
using UnityEngine.UI;
using TMPro;

public class ColorPickerObject : MonoBehaviour
{
    [field: SerializeField] public Color Color { get; private set; }
    
    [SerializeField] ColorPicker colorPicker;
    [SerializeField] TMP_Text textGlossValue;
    [SerializeField] Slider sliderGloss;

    public OverlayColorData ColorData;

    private UICreateCharacter uICreateCharacter;

    private float lastGloss = 0;
    private Color lastBaseColor;
    private Color lastMetallicColor = Color.black;


    private void OnEnable()
    {
        lastBaseColor = ColorData.color;
        colorPicker.CurrentColor = lastBaseColor;

        uICreateCharacter = GameObject.FindObjectOfType<UICreateCharacter>();

        colorPicker.onValueChanged.AddListener(color =>
        {
            OnColorChanged(color);
        });
    }

    public void OnColorChanged(Color baseColor)
    {
        lastBaseColor = baseColor;

        uICreateCharacter.SetColor(ColorData.name, baseColor, lastMetallicColor, lastGloss);
    }

    public void OnSliderValueChanged(float gloss)
    {
        lastGloss = gloss;
        textGlossValue.text = ((float)Math.Round((double)gloss, 2)).ToString();
        uICreateCharacter.SetColor(ColorData.name, lastBaseColor, lastMetallicColor, gloss);
    }
}
