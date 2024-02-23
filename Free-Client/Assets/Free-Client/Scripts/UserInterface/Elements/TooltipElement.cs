using TMPro;
using UnityEngine;

namespace Assambra.FreeClient.UserInterface
{
    public class TooltipElement : MonoBehaviour
    {
        [SerializeField] TMP_Text textTooltip;

        public void SetTooltipText(string text)
        {
            textTooltip.text = text;
        }

        public Vector2 GetTooltipSize()
        {
            return textTooltip.gameObject.GetComponent<RectTransform>().sizeDelta;
        }
    }
}
