using Assambra.GameFramework.GameManager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assambra.FreeClient.UserInterface
{
    public class PopupManager : MonoBehaviour
    {
        public static PopupManager Instance { get; private set; }

        [field: SerializeField] public UIHandler UIHandler { get; private set; }

        private Dictionary<Type, IPopupFactory> popupFactories;

        public void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;

            popupFactories = new Dictionary<Type, IPopupFactory>();

            popupFactories[typeof(InformationPopup)] = new PopupFactory<InformationPopup>();
            popupFactories[typeof(ErrorPopup)] = new PopupFactory<ErrorPopup>();
        }

        public T ShowInformationPopup<T>(string title, string information, Action onOK) where T : InformationPopup
        {
            if (popupFactories.TryGetValue(typeof(T), out var factory))
            {
                T popup = factory.CreatePopup() as T;
                popup.Setup(title, information, onOK);
                return popup;
            }
            else
            {
                Debug.LogError($"No factory for Popup-Typ {typeof(T)} found.");
                return null;
            }
        }

        public T ShowErrorPopup<T>(string title, string information, Action onOK) where T : ErrorPopup
        {
            if (popupFactories.TryGetValue(typeof(T), out var factory))
            {
                T popup = factory.CreatePopup() as T;
                popup.Setup(title, information, onOK);
                return popup;
            }
            else
            {
                Debug.LogError($"No factory for Popup-Typ {typeof(T)} found.");
                return null;
            }
        }
    }
}
