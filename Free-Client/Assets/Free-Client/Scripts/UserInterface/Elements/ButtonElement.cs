using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonElement : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TMP_Text text;

    public void Initialize(string buttonName, Action onButtonClickAction)
    {
        text.text = buttonName;
        button.onClick.AddListener(() => onButtonClickAction());
    }
}
