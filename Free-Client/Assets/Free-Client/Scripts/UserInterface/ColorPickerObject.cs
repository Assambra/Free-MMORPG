using UnityEngine;

public class ColorPickerObject : MonoBehaviour
{
    [field: SerializeField] public Color Color { get; private set; }
    
    //[SerializeField] ColorPicker colorPicker;

    private void OnEnable()
    {
        //colorPicker.onColorChanged += OnColorChanged;
    }

    private void OnDestroy()
    {
        //colorPicker.onColorChanged -= OnColorChanged;
    }

    public void OnColorChanged(Color c)
    {
        Color = c;
    }
}
