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

        [SerializeField] private List<PopupTypeData> _popupTypeData;
        
        private Dictionary<Type, IPopupFactory> _popupFactories;
        private Dictionary<PopupType, PopupTypeData> _popupTypeDataDictionary;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
            
            _popupFactories = new Dictionary<Type, IPopupFactory>();
            _popupTypeDataDictionary = new Dictionary<PopupType, PopupTypeData>();

            RegisterPopupFactory<InfoPopup>();
            RegisterPopupFactory<YesNoPopup>();
            RegisterPopupFactory<InputPopup>();

            InitializePopupTypeDataDictionary();
        }

        private void RegisterPopupFactory<T>() where T : BasePopup, new()
        {
            _popupFactories[typeof(T)] = new PopupFactory<T>();
        }

        private void InitializePopupTypeDataDictionary()
        {
            _popupTypeDataDictionary = new Dictionary<PopupType, PopupTypeData>();

            foreach (var popupData in _popupTypeData)
            {
                if (!_popupTypeDataDictionary.ContainsKey(popupData.Type))
                {
                    _popupTypeDataDictionary.Add(popupData.Type, popupData);
                }
                else
                {
                    CustomLogger.LogWarning($"Duplicate PopupType found: {popupData.Type}");
                }
            }
        }

        public Color GetTitleColor(PopupType type)
        {
            if (_popupTypeDataDictionary.TryGetValue(type, out var data))
            {
                return data.TitleColor;
            }
            CustomLogger.LogWarning($"No PopupTypeData found for PopupType: {type}");
            return Color.white;
        }

        public Sprite GetIcon(PopupType type)
        {
            if (_popupTypeDataDictionary.TryGetValue(type, out var data))
            {
                return data.Icon;
            }
            CustomLogger.LogWarning($"No PopupTypeData found for PopupType: {type}");
            return null;
        }

        public Color GetIconColor(PopupType type)
        {
            if (_popupTypeDataDictionary.TryGetValue(type, out var data))
            {
                return data.IconColor;
            }
            else
            {
                CustomLogger.LogWarning($"No PopupTypeData found for PopupType: {type}");
                return Color.white;
            }
        }

        public AudioClip GetPlaySound(PopupType type)
        {
            if (_popupTypeDataDictionary.TryGetValue(type, out var data))
            {
                return data.PlaySound;
            }
            CustomLogger.LogWarning($"No PopupTypeData found for PopupType: {type}");
            return null;
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
            return ShowPopup<T>(PopupType.Info, title, information, primaryCallback);
        }

        public T ShowInputPopup<T>(string title, string information, Action<string> primaryCallback = null, Action secondaryCallback = null) where T : InputPopup
        {
            return ShowPopup<T>(PopupType.Info, title, information, primaryCallback, secondaryCallback);
        }
    }
}
