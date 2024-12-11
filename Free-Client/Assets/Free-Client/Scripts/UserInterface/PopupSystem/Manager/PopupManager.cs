using Assambra.FreeClient.UserInterface.PopupSystem.Data;
using Assambra.FreeClient.UserInterface.PopupSystem.Enum;
using Assambra.FreeClient.UserInterface.PopupSystem.Factory;
using Assambra.FreeClient.UserInterface.PopupSystem.Interface;
using Assambra.FreeClient.UserInterface.PopupSystem.Popup;
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

        [SerializeField] private List<PopupIcon> _popupIcons;
        
        private Dictionary<Type, IPopupFactory> _popupFactories;
        private Dictionary<PopupType, Sprite> _iconDictionary;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;

            _popupFactories = new Dictionary<Type, IPopupFactory>();
            _iconDictionary = new Dictionary<PopupType, Sprite>();

            RegisterPopupFactory<InfoPopup>();
            RegisterPopupFactory<YesNoPopup>();
            RegisterPopupFactory<InputPopup>();

            InitializeIconDictionary();
        }

        private void RegisterPopupFactory<T>() where T : BasePopup, new()
        {
            _popupFactories[typeof(T)] = new PopupFactory<T>();
        }

        private void InitializeIconDictionary()
        {
            foreach (var popupIcon in _popupIcons)
            {
                if (!_iconDictionary.ContainsKey(popupIcon.Type))
                {
                    _iconDictionary.Add(popupIcon.Type, popupIcon.Icon);
                }
                else
                {
                    CustomLogger.LogWarning($"Duplicate PopupType found: {popupIcon.Type}");
                }
            }
        }

        public Sprite GetIcon(PopupType type)
        {
            if (_iconDictionary.TryGetValue(type, out var icon))
            {
                return icon;
            }
            else
            {
                CustomLogger.LogWarning($"No icon found for PopupType: {type}");
                return null;
            }
        }

        public T ShowPopup<T>(PopupType type, string title, string information, Delegate primaryCallback = null, Delegate secondaryCallback = null) where T : BasePopup
        {
            if (_popupFactories.TryGetValue(typeof(T), out var factory))
            {
                T popup = factory.CreatePopup() as T;
                if (popup == null)
                {
                    CustomLogger.LogError($"Failed to create Popup of type {typeof(T)}.");
                    return null;
                }

                popup.Setup(type, title, information, primaryCallback, secondaryCallback);
                return popup;
            }
            else
            {
                CustomLogger.LogError($"No factory for Popup-Typ {typeof(T)} found.");
                return null;
            }
        }

        public T ShowInfoPopup<T>(PopupType type, string title, string information, Action primaryCallback = null) where T : InfoPopup
        {
            return ShowPopup<T>(type, title, information, primaryCallback);
        }

        public T ShowYesNoPopup<T>(string title, string information, Action<bool> primaryCallback = null) where T : YesNoPopup
        {
            return ShowPopup<T>(PopupType.Information, title, information, primaryCallback);
        }

        public T ShowInputPopup<T>(string title, string information, Action<string> primaryCallback = null, Action secondaryCallback = null) where T : InputPopup
        {
            return ShowPopup<T>(PopupType.Information, title, information, primaryCallback, secondaryCallback);
        }
    }
}
