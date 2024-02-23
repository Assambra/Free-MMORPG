using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface
{
    public class ButtonColor : MonoBehaviour
    {
        [SerializeField] private Button buttonClose;
        [SerializeField] private Button buttonDecrease;
        [SerializeField] private Button buttonMaximize;
        [SerializeField] private Button buttonMinimize;
        [SerializeField] private Image buttonCloseSymbol;
    

        [SerializeField] private Color buttonCloseSymbolHoverColor;
        [SerializeField] private Color buttonCloseSymboldefaultColor;

        [SerializeField] private Color buttondefaultColor;

        [SerializeField] private Color buttonCloseHoverColor;
        [SerializeField] private Color buttonClosePressedColor;

        [SerializeField] private Color buttonNormalHoverColor;
        [SerializeField] private Color buttonNormalPressedColor;
    
        #region CLOSE BUTTON

        public void OnButtonClosePointerEnter()
        {
            buttonClose.gameObject.GetComponent<Image>().color = buttonCloseHoverColor;
            buttonCloseSymbol.color = buttonCloseSymbolHoverColor;
        }

        public void OnButtonClosePointerExit()
        {
            buttonClose.gameObject.GetComponent<Image>().color = buttondefaultColor;
            buttonCloseSymbol.color = buttonCloseSymboldefaultColor;
        }

        public void OnButtonClosePointerDown()
        {
            buttonClose.gameObject.GetComponent<Image>().color = buttonClosePressedColor;
        }

        public void OnButtonClosePointerUp()
        {
            OnButtonClosePointerEnter();
        }

        #endregion

        #region DECREASE BUTTON

        public void OnButtonDecreasePointerEnter()
        {
            buttonDecrease.gameObject.GetComponent<Image>().color = buttonNormalHoverColor;
        }

        public void OnButtonDecreasePointerExit()
        {
            buttonDecrease.gameObject.GetComponent<Image>().color = buttondefaultColor;
        }

        public void OnButtonDecreasePointerDown()
        {
            buttonDecrease.gameObject.GetComponent<Image>().color = buttonNormalPressedColor;
        }

        public void OnButtonDecreasePointerUp()
        {
            OnButtonDecreasePointerEnter();
        }

        #endregion

        #region MAXIMIZE BUTTON

        public void OnButtonMaximizePointerEnter()
        {
            buttonMaximize.gameObject.GetComponent<Image>().color = buttonNormalHoverColor;
        }

        public void OnButtonMaximizePointerExit()
        {
            buttonMaximize.gameObject.GetComponent<Image>().color = buttondefaultColor;
        }

        public void OnButtonMaximizePointerDown()
        {
            buttonMaximize.gameObject.GetComponent<Image>().color = buttonNormalPressedColor;
        }

        public void OnButtonMaximizePointerUp()
        {
            OnButtonMaximizePointerEnter();
        }

        #endregion

        #region MINIMIZE BUTTON

        public void OnButtonMinimizePointerEnter()
        {
            buttonMinimize.gameObject.GetComponent<Image>().color = buttonNormalHoverColor;
        }

        public void OnButtonMinimizePointerExit()
        {
            buttonMinimize.gameObject.GetComponent<Image>().color = buttondefaultColor;
        }

        public void OnButtonMinimizePointerDown()
        {
            buttonMinimize.gameObject.GetComponent<Image>().color = buttonNormalPressedColor;
        }

        public void OnButtonMinimizePointerUp()
        {
            OnButtonMinimizePointerEnter();
        }

        #endregion
    }
}
