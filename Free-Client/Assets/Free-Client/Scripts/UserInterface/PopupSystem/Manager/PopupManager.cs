using Assambra.FreeClient.UserInterface.PopupSystem;
using Assambra.FreeClient.Utilities;
using Assambra.GameFramework.GameManager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assambra.FreeClient.UserInterface.PopupSystem.Manager
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
            RegisterPopupFactory<InformationPopup>();
            RegisterPopupFactory<ErrorPopup>();
            RegisterPopupFactory<YesNoPopup>();
            RegisterPopupFactory<InputPopup>();
        }

        private void RegisterPopupFactory<T>() where T : BasePopup, new()
        {
            popupFactories[typeof(T)] = new PopupFactory<T>();
        }

        public T ShowPopup<T>(string title, string information, Delegate primaryCallback = null, Delegate secondaryCallback = null) where T : BasePopup
        {
            if (popupFactories.TryGetValue(typeof(T), out var factory))
            {
                T popup = factory.CreatePopup() as T;
                if (popup == null)
                {
                    CustomLogger.LogError($"Failed to create Popup of type {typeof(T)}.");
                    return null;
                }

                popup.Setup(title, information, primaryCallback, secondaryCallback);
                return popup;
            }
            else
            {
                CustomLogger.LogError($"No factory for Popup-Typ {typeof(T)} found.");
                return null;
            }
        }

        public T ShowInformationPopup<T>(string title, string information, Action primaryCallback = null) where T : InformationPopup
        {
            return ShowPopup<T>(title, information, primaryCallback);
        }

        public T ShowErrorPopup<T>(string title, string information, Action primaryCallback = null) where T : ErrorPopup
        {
            return ShowPopup<T>(title, information, primaryCallback);
        }

        public T ShowYesNoPopup<T>(string title, string information, Action<bool> primaryCallback = null) where T : YesNoPopup
        {
            return ShowPopup<T>(title, information, primaryCallback);
        }

        public T ShowInputPopup<T>(string title, string information, Action<string> primaryCallback = null, Action secondaryCallback = null) where T : InputPopup
        {
            return ShowPopup<T>(title, information, primaryCallback, secondaryCallback);
        }
    }
}
