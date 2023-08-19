using UnityEngine;
using TMPro;

public class ColorPicker : MonoBehaviour
{
    [field: SerializeField] public Color Color { get; private set; }
    
    [SerializeField] ColorPickerHandler colorPicker;

    [SerializeField] private TMP_Text headerText;

    private void OnEnable()
    {
        colorPicker.onColorChanged += OnColorChanged;
    }

    private void OnDestroy()
    {
        colorPicker.onColorChanged -= OnColorChanged;
    }

    private void Update()
    {
        
        Color = colorPicker.color;
    }

    public void SetColorPickerName(string name)
    {
        headerText.text = name;
    }

    public void OnColorChanged(Color c)
    {
        Color = c;
    }
}
