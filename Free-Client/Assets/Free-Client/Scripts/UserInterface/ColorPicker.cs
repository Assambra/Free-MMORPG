using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    [field: SerializeField] public Color Color { get; private set; }
    
    [SerializeField] ColorPickerHandler colorPickerHandler;

    [SerializeField] private TMP_Text headerText;
    [SerializeField] private Button headerButton;
    [SerializeField] private Sprite rightArrow;
    [SerializeField] private Sprite downArrow;

    private bool isOpen = false;

    private void OnEnable()
    {
        colorPickerHandler.onColorChanged += OnColorChanged;
        colorPickerHandler.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        colorPickerHandler.onColorChanged -= OnColorChanged;
    }

    private void Update()
    {
        
        Color = colorPickerHandler.color;
    }

    public void SetColorPickerName(string name)
    {
        headerText.text = name;
    }

    public void OnColorChanged(Color c)
    {
        Color = c;
    }

    public void OnHeaderButton()
    {
        if(!isOpen)
        {
            isOpen = true;
            headerButton.image.sprite = downArrow;
            colorPickerHandler.gameObject.SetActive(true);
        }
        else
        {
            isOpen = false;
            headerButton.image.sprite = rightArrow;
            colorPickerHandler.gameObject.SetActive(false);
        }
    }
}
