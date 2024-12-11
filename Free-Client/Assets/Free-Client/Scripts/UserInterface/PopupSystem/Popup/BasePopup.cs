using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assambra.FreeClient.Utilities;
using Assambra.FreeClient.UserInterface.PopupSystem.Enum;
using Assambra.FreeClient.UserInterface.PopupSystem.Manager;

namespace Assambra.FreeClient.UserInterface.PopupSystem.Popup
{
    public abstract class BasePopup : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _informationText;
        [SerializeField] private Button _buttonClose;

        public virtual void Setup(PopupType type, string title, string information, Delegate primaryCallback = null, Delegate secondaryCallback = null)
        {
            ApplyPopupType(type);

            _titleText.text = title;
            _informationText.text = information;

            if (!ValidateComponents(new (UnityEngine.Object, string)[]
            {
                (_buttonClose, "Close button")
            }))
                return;

            _buttonClose.onClick.RemoveAllListeners();
            _buttonClose.onClick.AddListener(() => { OnButtonClose(); });
        }

        private void ApplyPopupType(PopupType type)
        {
            var icon = PopupManager.Instance.GetIcon(type);

            if (_iconImage != null && icon != null)
            {
                _iconImage.sprite = icon;
            }

            switch (type)
            {
                case PopupType.Information:
                    _titleText.color = Color.blue;
                    break;
                case PopupType.Error:
                    _titleText.color = Color.red;
                    break;
                case PopupType.Warning:
                    _titleText.color = Color.yellow;
                    break;
            }
        }

        public virtual bool ValidateComponents((UnityEngine.Object component, string name)[] components)
        {
            bool hasErrors = false;

            foreach (var (component, name) in components)
            {
                if (component == null)
                {
                    CustomLogger.LogWarning($"{GetType().Name}: The {name} is not assigned.");
                    hasErrors = true;
                }
            }

            return !hasErrors;
        }

        public abstract void OnButtonClose();

        public virtual void Destroy()
        {
            if (_buttonClose != null)
                _buttonClose.onClick.RemoveAllListeners();

            Destroy(gameObject);
        }
    }
}
