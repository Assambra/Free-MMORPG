
using Assambra.FreeClient.UserInterface.PopupSystem.Interface;
using Assambra.FreeClient.UserInterface.PopupSystem.Manager;
using Assambra.FreeClient.UserInterface.PopupSystem.Popup;
using UnityEngine;

namespace Assambra.FreeClient.UserInterface.PopupSystem.Factory
{
    public class PopupFactory<T> : IPopupFactory where T : BasePopup
    {
        private T popupInstance;

        public BasePopup CreatePopup()
        {
            if (popupInstance == null || popupInstance.gameObject == null)
            {
                var prefab = Resources.Load<T>("Popups/" + typeof(T).Name);
                if (prefab == null)
                {
                    Debug.LogError("Can't load: " + "Popups/" + typeof(T).Name);
                    return null;
                }
                popupInstance = GameObject.Instantiate(prefab, PopupManager.Instance.UIHandler.Canvas);
            }

            return popupInstance;
        }
    }
}