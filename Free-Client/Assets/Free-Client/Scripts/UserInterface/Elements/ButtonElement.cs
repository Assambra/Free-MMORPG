using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface
{
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
}
